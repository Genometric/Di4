using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Polimi.DEIB.VahidJalili.DI4.Inv
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
        /// interval to Polimi.DEIB.VahidJalili.DI4; i.e., _di4_1R indexding.
        /// </summary>
        /// <param name="_di4_1R">The reference _di4_1R to be 
        /// manipulated.</param>
        internal BatchIndex(BPlusTree<C, B> di4)
        {
            _di4 = di4;
        }
        internal BatchIndex(BPlusTree<C, B> di4, List<I> Intervals, int Start, int Stop, IndexingMode mode, ConcurrentDictionary<int, int> addedBookmarks)
        {
            _di4 = di4;
            _intervals = Intervals;
            _start = Start;
            _stop = Stop;
            _mode = mode;
            _addedBookmarks = addedBookmarks;
            _bCounter = new BookmarkCounter();
            update.bookmarkCounter = _bCounter;
        }



        /// <summary>
        /// Sets and gets the _di4_1R data structure
        /// to be manipulated. This data structure
        /// is in common between all classes of 
        /// namespace.
        /// </summary>
        private BPlusTree<C, B> _di4 { set; get; }
        private IndexingMode _mode { set; get; }
        private int _start { set; get; }
        private int _stop { set; get; }
        private I _interval { set; get; }
        private List<I> _intervals { set; get; }
        private ConcurrentDictionary<int, int> _addedBookmarks { set; get; }
        private BookmarkCounter _bCounter { set; get; }
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
                        update.hashKey = _intervals[i].hashKey;
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
            _interval = interval;
            bool isLeftEnd = true;
            bool enumerated = false;
            update.phi = Phi.LeftEnd;
            update.hashKey = _interval.hashKey;
            int compareResult;

            foreach (var item in _di4.EnumerateFrom(_interval.left))
            {
                enumerated = true;
                update.NextBlock = null;

                if (isLeftEnd)
                {
                    if (HandleFirstItem(item)) break;
                    isLeftEnd = false;
                }
                else
                {
                    compareResult = _interval.right.CompareTo(item.Key);

                    if (compareResult == 0)
                    {
                        update.phi = Phi.RightEnd;
                        break;
                    }
                    else if (compareResult == 1)// interval.right is bigger than bookmark.Key
                    {
                        update.phi = Phi.Middle;
                        _di4.AddOrUpdate(item.Key, ref update);
                    }
                    else
                    {
                        update.phi = Phi.RightEnd;
                        update.NextBlock = item.Value;
                        break;
                    }
                }

                /// this will be useful when the iteration reaches the 
                /// end of collection while right-end is not handled yet. 
                update.phi = Phi.RightEnd;
            }

            if (enumerated)
            {
                _di4.AddOrUpdate(_interval.right, ref update);
            }
            else
            {
                update.NextBlock = null;
                update.phi = Phi.LeftEnd;
                update.hashKey = _interval.hashKey;
                _di4.AddOrUpdate(_interval.left, ref update);

                update.phi = Phi.RightEnd;
                update.hashKey = _interval.hashKey;
                _di4.AddOrUpdate(_interval.right, ref update);
            }
        }

        public void SecondPass()
        {
            KeyValuePair<C, B> firstItem;
            _di4.TryGetFirst(out firstItem);

            Dictionary<uint, Lambda> lambdaCarrier = new Dictionary<uint, Lambda>();
            KeyValueUpdate<C, B> updateFunction = delegate (C k, B i) { return i.Update(lambdaCarrier); };
            List<uint> keysToRemove = new List<uint>();
            List<uint> keys;

            foreach (var bookmark in _di4.EnumerateFrom(firstItem.Key))
            {
                foreach (var lambda in bookmark.Value.lambda)
                {
                    if (lambdaCarrier.ContainsKey(lambda.atI))
                        lambdaCarrier[lambda.atI] = lambda;
                    else
                        lambdaCarrier.Add(lambda.atI, lambda);

                    if (lambda.phi == Phi.RightEnd) keysToRemove.Add(lambda.atI);
                }

                if (UpdateRequired(bookmark.Value.lambda, lambdaCarrier))
                    _di4.TryUpdate(bookmark.Key, updateFunction);

                foreach (uint item in keysToRemove)
                    lambdaCarrier.Remove(item);
                keysToRemove.Clear();

                keys = new List<uint>(lambdaCarrier.Keys);
                foreach (var key in keys)
                    lambdaCarrier[key] = new Lambda(Phi.Middle, lambdaCarrier[key].atI);
            }
        }
        private bool UpdateRequired(ReadOnlyCollection<Lambda> lambda, Dictionary<uint, Lambda> lambdaCarrier)
        {
            if (lambda.Count != lambdaCarrier.Count) return true;
            foreach (var item in lambda)
                if (!lambdaCarrier.ContainsKey(item.atI))
                    return true;
            return false;
        }

        private bool HandleFirstItem(KeyValuePair<C, B> item)
        {
            if (_interval.left.Equals(item.Key))
            {
                _di4.AddOrUpdate(_interval.left, ref update);
                return false;
            }
            else
            {
                update.NextBlock = item.Value;

                switch (_interval.right.CompareTo(item.Key))
                {
                    case 1: // _interval.right is bigger than lambda.newKey
                        _di4.AddOrUpdate(_interval.left, ref update);

                        update.phi = Phi.Middle;
                        update.NextBlock = null;
                        _di4.AddOrUpdate(item.Key, ref update);
                        return false;

                    case 0:
                    case -1:
                        _di4.AddOrUpdate(_interval.left, ref update);

                        update.phi = Phi.RightEnd;
                        return true;
                }
            }

            return true;
        }



        struct AddUpdateValue : ICreateOrUpdateValue<C, B>, IRemoveValue<C, B>
        {
            public B oldValue;
            public Phi phi { set; get; }
            public uint hashKey { set; get; }
            public BookmarkCounter bookmarkCounter { set; get; }
            public B NextBlock { set; get; }

            public bool CreateValue(C key, out B value)
            {
                oldValue = null;

                if (NextBlock == null)
                    value = new B(phi: phi, hashKey: hashKey);
                else
                    value = new B(phi: phi, metadata: hashKey, nextBlock: NextBlock);

                bookmarkCounter.value++;
                return true;
            }
            public bool UpdateValue(C key, ref B value)
            {
                oldValue = value;
                value = value.Update(omega: value.omega, phi: phi, hashKey: hashKey);
                return true;
            }
            public bool RemoveValue(C key, B value)
            {
                oldValue = value;
                if (value == value.Update(atI: hashKey, condition: phi))
                {
                    bookmarkCounter.value--;
                    return true;
                }
                return value == value.Update(omega: value.omega, phi: phi, hashKey: hashKey);
            }
        }

        private class BookmarkCounter
        {
            public int value { set; get; }
        }
    }
}