using CSharpTest.Net.Collections;
using Interfaces;
using System;
using System.Collections.Generic;


namespace DI3
{
    /// <summary>
    /// Provides efficient means of inserting an 
    /// _interval to DI3; i.e., _di3 indexding.
    /// </summary>
    /// <typeparam name="C">Represents the c/domain
    /// type (e.g,. int, double, Time).</typeparam>
    /// <typeparam name="I">Represents generic type of the _interval.
    /// (e.g., time span, _interval on natural numbers)
    /// <para>For _intervals of possibly different types,
    /// it is recommended to define this generic type
    /// parameter in terms of Lowest Common Denominator.
    /// </para></typeparam>
    /// <typeparam name="M">Represents generic
    /// type of pointer to descriptive hashKey cooresponding
    /// to the _interval.</typeparam>
    class INDEX<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        /// <summary>
        /// Provides efficient means of inserting an 
        /// _interval to DI3; i.e., _di3 indexding.
        /// </summary>
        /// <param name="_di3">The reference _di3 to be 
        /// manipulated.</param>
        internal INDEX(BPlusTree<C, B> di3)
        {
            _di3 = di3;
        }
        internal INDEX(BPlusTree<C, B> di3, List<I> Intervals, int Start, int Stop, Mode mode)
        {
            _di3 = di3;
            _intervals = Intervals;
            _start = Start;
            _stop = Stop;
            _mode = mode;            
        }

        

        /// <summary>
        /// Sets and gets the _di3 data structure
        /// to be manipulated. This data structure
        /// is in common between all classes of 
        /// namespace.
        /// </summary>
        private BPlusTree<C, B> _di3 { set; get; }
        /// <summary>
        /// Sets and Gets the _interval to 
        /// be added to _di3. 
        /// </summary>
        private I _interval { set; get; }
        private Mode _mode { set; get; }
        private List<I> _intervals { set; get; }
        private int _start { set; get; }
        private int _stop { set; get; }
        private AddUpdateValue update = new AddUpdateValue();


        /// <summary>
        /// Indexes the provided _interval.
        /// </summary>
        /// <param name="interval">The _interval to be index.</param>
        public void Index(I interval)
        {
            _interval = interval;
            bool isLeftEnd = true;
            bool enumerated = false;
            update.tau = 'L';
            update.hashKey = _interval.hashKey;

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
                    if (_interval.right.Equals(item.Key))
                    {
                        update.tau = 'R';
                        break;
                    }
                    else if (_interval.right.CompareTo(item.Key) == 1) // interval.right is bigger than block.Key
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

                    if (lambda.tau == 'R') keysToRemove.Add(lambda.atI);
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
                    case 1: // _interval.right is bigger than block.key
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
