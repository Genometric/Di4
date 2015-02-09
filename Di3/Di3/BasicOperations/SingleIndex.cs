using CSharpTest.Net.Collections;
using IGenomics;
using System;
using System.Collections.Generic;


namespace DI3
{
    /// <summary>
    /// Provides efficient means of inserting an 
    /// interval to Di3; i.e., Di3 indexding.
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
    internal class SingleIndex<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        /// <summary>
        /// Provides efficient means of inserting an 
        /// interval to DI3; i.e., _di3_1R indexding.
        /// </summary>
        /// <param name="_di3">The reference _di3_1R to be 
        /// manipulated.</param>
        internal SingleIndex(BPlusTree<C, B> di3)
        {
            _di3 = di3;
        }
        internal SingleIndex(BPlusTree<C, B> di3, List<I> Intervals, int Start, int Stop, Mode mode)
        {
            _di3 = di3;
            _intervals = Intervals;
            _start = Start;
            _stop = Stop;
            _mode = mode;
        }

        

        /// <summary>
        /// Sets and gets the _di3_1R data structure
        /// to be manipulated. This data structure
        /// is in common between all classes of 
        /// namespace.
        /// </summary>
        private BPlusTree<C, B> _di3 { set; get; }
        private Mode _mode { set; get; }
        private int _start { set; get; }
        private int _stop { set; get; }
        /// <summary>
        /// Sets and Gets the interval to 
        /// be added to _di3_1R. 
        /// </summary>
        private I _interval { set; get; }
        private List<I> _intervals { set; get; }
        private AddUpdateValue update = new AddUpdateValue();
        

        public void Index()
        {
            int i;
            switch (_mode)
            {
                case Mode.SinglePass:
                    for (i = _start; i < _stop; i++)
                        Index(_intervals[i]);
                    break;

                case Mode.MultiPass:
                    for (i = _start; i < _stop; i++)
                    {
                        update.hashKey = _intervals[i].hashKey;
                        update.tau = 'L';
                        _di3.AddOrUpdate(_intervals[i].left, ref update);

                        update.tau = 'R';
                        _di3.AddOrUpdate(_intervals[i].right, ref update);
                    }
                    break;
            }
        }

        /// <summary>
        /// Indexes the provided interval.
        /// </summary>
        /// <param name="interval">The interval to be index.</param>
        public void Index(I interval)
        {
            _interval = interval;
            bool isLeftEnd = true;
            bool enumerated = false;
            update.tau = 'L';
            update.hashKey = _interval.hashKey;
            int compareResult;

            foreach (var item in _di3.EnumerateFrom(_interval.left))
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
                        update.tau = 'R';
                        break;
                    }
                    else if (compareResult == 1)// interval.right is bigger than bookmark.Key
                    {
                        update.tau = 'M';
                        _di3.AddOrUpdate(item.Key, ref update);
                    }
                    else
                    {
                        update.tau = 'R';
                        update.NextBlock = item.Value;
                        break;
                    }
                }

                /// this will be useful when the iteration reaches the 
                /// end of collection while right-end is not handled yet. 
                update.tau = 'R';
            }

            if (enumerated)
            {
                _di3.AddOrUpdate(_interval.right, ref update);
            }
            else
            {
                update.NextBlock = null;
                update.tau = 'L';
                update.hashKey = _interval.hashKey;
                _di3.AddOrUpdate(_interval.left, ref update);

                update.tau = 'R';
                update.hashKey = _interval.hashKey;
                _di3.AddOrUpdate(_interval.right, ref update);
            }
        }

        public int SecondPass()
        {
            int blockCount = 0;
            KeyValuePair<C, B> firstItem;
            _di3.TryGetFirst(out firstItem);

            Dictionary<uint, Lambda> lambdaCarrier = new Dictionary<uint, Lambda>();
            KeyValueUpdate<C, B> updateFunction = delegate(C k, B i) { return i.Update(lambdaCarrier); };
            List<uint> keysToRemove = new List<uint>();
            List<uint> keys;

            foreach (var block in _di3.EnumerateFrom(firstItem.Key))
            {
                foreach (var lambda in block.Value.lambda)
                {
                    if (lambdaCarrier.ContainsKey(lambda.atI))
                        lambdaCarrier[lambda.atI] = lambda;
                    else
                        lambdaCarrier.Add(lambda.atI, lambda);

                    if (lambda.phi == 'R') keysToRemove.Add(lambda.atI);
                }

                _di3.TryUpdate(block.Key, updateFunction);

                foreach (uint item in keysToRemove)
                    lambdaCarrier.Remove(item);
                keysToRemove.Clear();

                keys = new List<uint>(lambdaCarrier.Keys);
                foreach (var key in keys)
                    lambdaCarrier[key] = new Lambda('M', lambdaCarrier[key].atI);

                blockCount++;
            }

            return blockCount;
        }

        private bool HandleFirstItem(KeyValuePair<C, B> item)
        {
            if (_interval.left.Equals(item.Key))
            {
                _di3.AddOrUpdate(_interval.left, ref update);
                return false;
            }
            else
            {
                update.NextBlock = item.Value;

                switch (_interval.right.CompareTo(item.Key))
                {
                    case 1: // _interval.right is bigger than item.newKey
                        _di3.AddOrUpdate(_interval.left, ref update);

                        update.tau = 'M';
                        update.NextBlock = null;
                        _di3.AddOrUpdate(item.Key, ref update);
                        return false;

                    case 0:
                    case -1:
                        _di3.AddOrUpdate(_interval.left, ref update);

                        update.tau = 'R';
                        return true;
                }
            }

            return true;
        }



        struct AddUpdateValue : ICreateOrUpdateValue<C, B>, IRemoveValue<C, B>
        {
            public B oldValue;
            public char tau { set; get; }
            public UInt32 hashKey { set; get; }

            public B NextBlock { set; get; }

            public bool CreateValue(C key, out B value)
            {
                oldValue = null;
                value = GetNewBlock();
                return hashKey != 0 && tau != default(char);
            }
            public bool UpdateValue(C key, ref B value)
            {
                oldValue = value;

                if (tau == 'R')
                    value = value.Update(omega: value.omega + 1, tau: tau, hashKey: hashKey);
                else
                    value = value.Update(omega: value.omega, tau: tau, hashKey: hashKey);

                return hashKey != 0 && tau != default(char);
            }
            public bool RemoveValue(C key, B value)
            {
                oldValue = value;

                if (tau == 'R')
                    return value == value.Update(omega: value.omega + 1, tau: tau, hashKey: hashKey);
                else
                    return value == value.Update(omega: value.omega, tau: tau, hashKey: hashKey);
            }

            private B GetNewBlock()
            {
                if (NextBlock == null)
                    return new B(tau: tau, hashKey: hashKey);
                else
                    return new B(tau: tau, metadata: hashKey, nextBlock: NextBlock);
            }
        }
    }
}
