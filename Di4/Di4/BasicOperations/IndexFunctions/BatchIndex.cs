using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Genometric.Di4
{
    /// <summary>
    /// Provides efficient means of inserting an 
    /// interval to Di4; i.e., Di4 indexding.
    /// </summary>
    /// <typeparam name="C">Represents the c/domain
    /// type (e.g,. int, double, Time).</typeparam>
    /// <typeparam name="I">Represents generic type of the interval.
    /// (e.g., time span, interval on natural numbers)
    /// <para>For intervals of possibly different types,
    /// it is recommended to define this generic type
    /// parameter in terms of Lowest Common Denominator.
    /// </para></typeparam>
    /// <typeparam name="M">Represents generic
    /// type of pointer to descriptive hashKey cooresponding
    /// to the interval.</typeparam>
    internal class BatchIndex<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        /// <summary>
        /// Provides efficient means of inserting an 
        /// interval to Genometric.Di4; i.e., _di4_1R indexding.
        /// </summary>
        /// <param name="_di4_1R">The reference _di4_1R to be 
        /// manipulated.</param>
        internal BatchIndex(BPlusTree<C, B> di4)
        {
            _di4 = di4;
        }
        internal BatchIndex(BPlusTree<C, B> di4, uint collectionID, List<I> intervals, int start, int stop, IndexingMode mode, ConcurrentDictionary<int, int> addedBookmarks)
        {
            _di4 = di4;
            _collectionID = collectionID;
            _intervals = intervals;
            _start = start;
            _stop = stop;
            _mode = mode;
            _addedBookmarks = addedBookmarks;
            _bCounter = new BookmarkCounter();
            update.bookmarkCounter = _bCounter;
        }



        private BPlusTree<C, B> _di4;
        private uint _collectionID;
        private IndexingMode _mode;
        private int _start;
        private int _stop;
        private List<I> _intervals;
        private ConcurrentDictionary<int, int> _addedBookmarks;
        private BookmarkCounter _bCounter;
        private AddUpdateValue update = new AddUpdateValue();


        public void Run()
        {
            int i;
            switch (_mode)
            {
                case IndexingMode.SinglePass:
                    for (i = _start; i < _stop; i++)
                        Index(_intervals[i]);
                    break;

                case IndexingMode.MultiPass:
                    for (i = _start; i < _stop; i++)
                    {
                        update.atI = _intervals[i].hashKey;
                        update.collectionID = _collectionID;

                        update.phi = Phi.LeftEnd;
                        _di4.AddOrUpdate(_intervals[i].left, ref update);

                        update.phi = Phi.RightEnd;
                        _di4.AddOrUpdate(_intervals[i].right, ref update);
                    }
                    break;
            }
            _addedBookmarks.TryAdd(_start, _bCounter.value);
        }
        private void Index(I interval)
        {
            update.phi = Phi.LeftEnd;
            update.atI = interval.hashKey;
            update.collectionID = _collectionID;
            update.NextBookmark = null;

            var enumerator = _di4.EnumerateFrom(interval.left).GetEnumerator();

            if (!enumerator.MoveNext())
            {
                _di4.AddOrUpdate(interval.left, ref update);

                update.phi = Phi.RightEnd;
                _di4.AddOrUpdate(interval.right, ref update);
                return;
            }

            if (interval.left.Equals(enumerator.Current.Key))
            {
                _di4.AddOrUpdate(interval.left, ref update);
                if (!enumerator.MoveNext())
                {
                    update.phi = Phi.RightEnd;
                    _di4.AddOrUpdate(interval.right, ref update);
                    return;
                }
            }
            else
            {
                update.NextBookmark = enumerator.Current.Value;
                _di4.AddOrUpdate(interval.left, ref update);
            }

            do
            {
                switch (interval.right.CompareTo(enumerator.Current.Key))
                {
                    case 0:
                        update.phi = Phi.RightEnd;
                        update.NextBookmark = null;
                        _di4.AddOrUpdate(interval.right, ref update);
                        return;

                    case 1:
                        update.phi = Phi.Middle;
                        update.NextBookmark = null;
                        _di4.AddOrUpdate(enumerator.Current.Key, ref update);
                        break;

                    case -1:
                        update.phi = Phi.RightEnd;
                        update.NextBookmark = enumerator.Current.Value;
                        _di4.AddOrUpdate(interval.right, ref update);
                        return;
                }
            }
            while (enumerator.MoveNext());

            update.phi = Phi.RightEnd;
            _di4.AddOrUpdate(interval.right, ref update);
        }
        public void SecondPass()
        {
            KeyValuePair<C, B> firstItem;
            _di4.TryGetFirst(out firstItem);

            int mu = 0;
            ushort omega = 0;
            ReadOnlyCollection<Lambda> currentBookmarkLambda = null;
            var t = new Dictionary<uint, bool>();
            KeyValueUpdate<C, B> updateFunction = delegate (C k, B i) { return i.Update(ref mu, ref omega, currentBookmarkLambda); };

            foreach (var bookmark in _di4.EnumerateFrom(firstItem.Key))
            {
                // mu update option A:
                mu = t.Count - bookmark.Value.omega;

                foreach (var lambda in bookmark.Value.lambda)
                    if (!t.Remove(lambda.atI))
                        t.Add(lambda.atI, true);

                // mu update option B:
                // mu = t.Count - bookmark.Value.lambda.Count + bookmark.Value.omega; // ;-)
                if (bookmark.Value.mu != mu)
                {
                    omega = bookmark.Value.omega;
                    currentBookmarkLambda = bookmark.Value.lambda;
                    _di4.TryUpdate(bookmark.Key, updateFunction);
                }
            }
        }

        struct AddUpdateValue : ICreateOrUpdateValue<C, B>, IRemoveValue<C, B>
        {
            public B oldValue;
            public Phi phi { set; get; }
            public uint atI { set; get; }
            public uint collectionID { set; get; }
            public BookmarkCounter bookmarkCounter { set; get; }
            public B NextBookmark { set; get; }

            public bool CreateValue(C key, out B value)
            {
                oldValue = null;
                if (NextBookmark == null)
                    value = new B(phi: phi, atI: atI, collectionID: collectionID);
                else
                    value = new B(phi: phi, atI: atI, nextBookmark: NextBookmark, collectionID: collectionID);

                bookmarkCounter.value++;
                return true;
            }
            public bool UpdateValue(C key, ref B value)
            {
                oldValue = value;
                value = value.Update(atI: atI, condition: phi, collectionID: collectionID);
                return true;
            }
            public bool RemoveValue(C key, B value)
            {
                oldValue = value;
                if (value == value.Update(atI: atI, condition: phi, collectionID: collectionID))
                {
                    bookmarkCounter.value--;
                    return true;
                }
                return false;
            }
        }

        private class BookmarkCounter
        {
            public int value { set; get; }
        }
    }
}
