using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Genometric.Di4.AuxiliaryComponents;


namespace Genometric.Di4.Inv
{
    /// <summary>
    /// Provides efficient means of inserting an 
    /// interval to Di4.
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
        internal BatchIndex(BPlusTree<C, B> di4, uint collectionID, List<I> Intervals, int Start, int Stop, IndexingMode mode, ConcurrentDictionary<int, int> addedBookmarks)
        {
            _di4 = di4;
            _collectionID = collectionID;
            _intervals = Intervals;
            _start = Start;
            _stop = Stop;
            _mode = mode;
            _addedBookmarks = addedBookmarks;
            _bCounter = new BookmarkCounter();
            update.bookmarkCounter = _bCounter;
        }


        private int _start;
        private int _stop;
        private uint _collectionID;
        private IndexingMode _mode;
        private List<I> _intervals;
        private BPlusTree<C, B> _di4;
        private ConcurrentDictionary<int, int> _addedBookmarks;
        private BookmarkCounter _bCounter;
        private AddUpdateValue update = new AddUpdateValue();


        public void Run()
        {
            switch (_mode)
            {
                case IndexingMode.SinglePass:
                    for (int i = _start; i < _stop; i++)
                        Index(_intervals[i]);
                    break;

                case IndexingMode.MultiPass:
                    for (int i = _start; i < _stop; i++)
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

        public void Index(I interval)
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

            var lambdaCarrier = new AtomicDictionary<uint, Lambda>();
            KeyValueUpdate<C, B> updateFunction = delegate (C k, B i) { return i.Update(lambdaCarrier.Values); };
            List<uint> keysToRemove = new List<uint>();
            List<uint> keys;

            foreach (var bookmark in _di4.EnumerateFrom(firstItem.Key))
            {
                foreach (var lambda in bookmark.Value.lambda)
                {
                    lambdaCarrier.AddOrUpdate(lambda.atI, lambda);
                    if (lambda.phi == Phi.RightEnd) keysToRemove.Add(lambda.atI);
                }

                if (UpdateRequired(bookmark.Value.lambda, lambdaCarrier))
                    _di4.TryUpdate(bookmark.Key, updateFunction);

                foreach (uint item in keysToRemove)
                    lambdaCarrier.Remove(item);
                keysToRemove.Clear();

                keys = new List<uint>(lambdaCarrier.Keys);
                foreach (var key in keys)
                    lambdaCarrier[key] = new Lambda(Phi.Middle, lambdaCarrier[key].atI, _collectionID);
            }
        }
        private bool UpdateRequired(ReadOnlyCollection<Lambda> lambda, AtomicDictionary<uint, Lambda> lambdaCarrier)
        {
            if (lambda.Count != lambdaCarrier.Count) return true;
            foreach (var item in lambda)
                if (!lambdaCarrier.ContainsKey(item.atI))
                    return true;
            return false;
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
                    value = new B(phi: phi, hashKey: atI, collectionID: collectionID);
                else
                    value = new B(phi: phi, metadata: atI, nextBlock: NextBookmark, collectionID: collectionID);

                bookmarkCounter.value++;
                return true;
            }
            public bool UpdateValue(C key, ref B value)
            {
                oldValue = value;
                value = value.Update(omega: value.omega, phi: phi, hashKey: atI, collectionID: collectionID);
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
                return value == value.Update(omega: value.omega, phi: phi, hashKey: atI, collectionID: collectionID);
            }
        }

        private class BookmarkCounter
        {
            public int value { set; get; }
        }
    }
}