    using System;
    using System.Linq;
    using Interfaces;
    using CSharpTest.Net.Collections;
    using System.Collections.Generic;
    using CSharpTest.Net.Threading;

namespace Di3WithCustomBlockSerializer
{


    /// <summary>
    /// Provides efficient means of inserting an 
    /// interval to DI3; i.e., di3 indexding.
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
    /// type of pointer to descriptive metadata cooresponding
    /// to the interval.</typeparam>
    class INDEX
    {
        /// <summary>
        /// Provides efficient means of inserting an 
        /// interval to DI3; i.e., di3 indexding.
        /// </summary>
        /// <param name="di3">The reference di3 to be 
        /// manipulated.</param>
        internal INDEX(BPlusTree<int, B> di3)
        {
            this.di3 = di3;
            //update.di3 = di3;
        }


        internal INDEX(BPlusTree<int, B> di3, List<Intervals> Intervals, int Start, int Stop)
        {
            this.di3 = di3;
            this.intervals = Intervals;
            this.Start = Start;
            this.Stop = Stop;
        }

        /// <summary>
        /// Sets and gets the di3 data structure
        /// to be manipulated. This data structure
        /// is in common between all classes of 
        /// namespace.
        /// </summary>
        private BPlusTree<int, B> di3 { set; get; }

        /// <summary>
        /// Represents the coordinate (right or left-end)
        /// of interval being inserted to di3.
        /// </summary>
        private int marshalPoint { set; get; }

        /// <summary>
        /// Sets and Gets the interval to 
        /// be added to di3. 
        /// </summary>
        private Intervals interval { set; get; }

        private List<Intervals> intervals { set; get; }

        private int Start { set; get; }
        private int Stop { set; get; }

        /// <summary>
        /// Sets and Gets the cardinality of di3.
        /// </summary>
        private int di3Cardinality { set; get; }

        public int TEST_Sample_Number;
        public int TEST_Region_Number;

        private AddUpdateValue update = new AddUpdateValue();



        private int test_Maximum_Lambda_Lenght { set; get; }


        /// <summary>
        /// Indexes the provided interval.
        /// </summary>
        /// <param name="Intval">The interval to be index.</param>
        public int Index(Intervals Intval)
        {

            interval = Intval;

            /*
            marshalPoint = interval.left;
            Insert('L');

            marshalPoint = interval.right;
            //MarchForRightEnd();
            Insert('R');
            */

            bool isLeftEnd = true;
            bool enumerated = false;
            update.Tau = 'L';
            update.Metadata = Intval.hashKey;

            foreach (var item in di3.EnumerateFrom(Intval.left))
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
                    if (Intval.right.Equals(item.Key))
                    {
                        update.Tau = 'R';
                        break;
                    }
                    else if (Intval.right.CompareTo(item.Key) == 1) // Intval.right is bigger than block.Key
                    {
                        update.Tau = 'M';
                        di3.AddOrUpdate(item.Key, ref update);
                    }
                    else
                    {
                        update.Tau = 'R';
                        update.NextBlock = item.Value;
                        break;
                    }
                }

                /// this will be useful when the iteration reaches the 
                /// end of collection while right-end is not handled yet. 
                update.Tau = 'R';
            }

            if (enumerated)
            {
                di3.AddOrUpdate(Intval.right, ref update);
            }
            else
            {
                update.NextBlock = null;
                update.Tau = 'L';
                update.Metadata = Intval.hashKey;
                di3.AddOrUpdate(Intval.left, ref update);

                update.Tau = 'R';
                update.Metadata = Intval.hashKey;
                di3.AddOrUpdate(Intval.right, ref update);
            }






            return test_Maximum_Lambda_Lenght;
        }

        public void Index()
        {
            int i;
            /*switch (_mode)
            {
                case Mode.SinglePass:*/
                    /*for (i = Start; i < Stop; i++)
                        Index(intervals[i]);*/
                    /*break;

                case Mode.MultiPass:*/
                    for (i = Start; i < Stop; i++)
                    {
                        update.Metadata = intervals[i].hashKey;
                        update.Tau = 'L';
                        di3.AddOrUpdate(intervals[i].left, ref update);

                        update.Tau = 'R';
                        di3.AddOrUpdate(intervals[i].right, ref update);
                    }
                    /*break;
            }*/
        }

        public int SecondPass()
        {/*
            KeyValuePair<C, B> firstItem;
            di3.TryGetFirst(out firstItem);

            Dictionary<uint, Lambda<C, M>> lambdaCarrier = new Dictionary<uint, Lambda<C, M>>();
            KeyValueUpdate<C, B<C, M>> updateFunction = delegate(C k, B<C, M> i) { return i.Update(lambdaCarrier); };
            List<uint> keysToRemove = new List<uint>();
            List<uint> keys;


            int TESTBlockCount = 0;

            foreach (var block in di3.EnumerateFrom(firstItem.Key))
            {
                TESTBlockCount++;

                foreach (var lambda in block.Value.lambda)
                {
                    if (lambdaCarrier.ContainsKey(lambda.atI.hashKey))
                        lambdaCarrier[lambda.atI.hashKey] = lambda;
                    else
                        lambdaCarrier.Add(lambda.atI.hashKey, lambda);

                    if (lambda.tau == 'R') keysToRemove.Add(lambda.atI.hashKey);
                }

                di3.TryUpdate(block.Key, updateFunction);

                foreach (uint item in keysToRemove)
                    lambdaCarrier.Remove(item);
                keysToRemove.Clear();

                keys = new List<uint>(lambdaCarrier.Keys);
                foreach (var key in keys)
                    lambdaCarrier[key] = new Lambda<C, M>('M', lambdaCarrier[key].atI);
            }

            return TESTBlockCount;*/
            return 0;
        }

        private bool HandleFirstItem(KeyValuePair<int, B> item)
        {
            if (interval.left.Equals(item.Key))
            {
                di3.AddOrUpdate(interval.left, ref update);
                return false;
            }
            else
            {
                update.NextBlock = item.Value;

                switch (interval.right.CompareTo(item.Key))
                {
                    case 1: // interval.right is bigger than block.key
                        di3.AddOrUpdate(interval.left, ref update);

                        update.Tau = 'M';
                        update.NextBlock = null;
                        di3.AddOrUpdate(item.Key, ref update);
                        return false;

                    case 0:
                    case -1:
                        di3.AddOrUpdate(interval.left, ref update);

                        update.Tau = 'R';
                        return true;
                }
            }

            return true;
        }


        /// <summary>
        /// Linearly searches di3 starting from left-end
        /// insertion index of the interval toward di3 end for 
        /// proper postion for the interval's right-end insertion.
        /// </summary>
        private void MarchForRightEnd()
        {
            /*foreach (var block in di3.EnumerateFrom(interval.left).Skip(1))
            {
                switch (marshalPoint.CompareTo(block.Key))
                {
                    case 0:  // equal
                    case -1: // greater than
                        return;

                    case 1:  // less than
                        UpdateBlock(block.Key, 'M');
                        break;
                }
            }*/
        }


        /// <summary>
        /// Inserts right/left -end of the interval to 
        /// determined position on di3 by either updating
        /// present block or creating a new block.
        /// </summary>
        /// <param name="tau">The intersection type of interval
        /// wtih marshalPoint of corresponding block.</param>
        private void Insert(char tau)
        {
            /*
            // Shall new Block be added to the end of list ? OR: Does the same index already available ?
            if (di3.ContainsKey(marshalPoint)) // Condition satisfied: add new index
            {
                UpdateBlock(marshalPoint, tau);
            }
            else // update the available index with new region
            { // Test with TryAdd
                //di3.Add(marshalPoint, GetNewBlock(tau));
                di3.TryAdd(marshalPoint, GetNewBlock(tau));
            }*/

            update.Tau = tau;
            //update.Metadata = interval.metadata;
            di3.AddOrUpdate(marshalPoint, ref update);
        }


        /// <summary>
        /// Initializes and returns a new block
        /// to be added to di3.
        /// </summary>
        /// <param name="tau">The intersection type of interval
        /// wtih c of corresponding block.</param>
        /// <returns>A new block to be added to di3.</returns>
        private B GetNewBlock(char tau)
        {
            //B newB = new B(marshalPoint) { };
            //newB.lambda.Add(new Lambda<C, M>(tau, interval.metadata));

            //if (tau == 'R') newB.omega = 1;

            // I need to access one block ahead, but since I could not 
            // find any proper way to do so with BPlusTree, I'm using 
            // following ramblings :)
            /*foreach (var block in di3.EnumerateFrom(marshalPoint))//.Skip(0))
            {
                foreach (var d in block.newValue.lambda)
                {
                    if (d.tau != 'L')                    
                        newB.lambda.Add(new Lambda<C, M>('M', d.atI) { });                    
                }

                // only one object :) 
                break;
            }*/

            //test_Maximum_Lambda_Lenght = Math.Max(test_Maximum_Lambda_Lenght, newB.lambda.Count);

            //return newB;
            return null;
        }

        /// <summary>
        /// Updates the di3 block specified by 'b' 
        /// to represent the interval being indexed as well.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="tau">The intersection type of interval
        /// wtih c of corresponding block.</param>
        private void UpdateBlock(int key, char tau)
        {
            /*using(var sw = new System.IO.StreamWriter("D:\\UpdateCalls.txt", true))
            {
                sw.WriteLine("Updating at sample #{0:N0}   and Region #{1:N0}", TEST_Sample_Number, TEST_Region_Number);
            }*/


            /// 1st Strategy: is the old version; it cause errors because it's not actually updateing. 
            //di3[key].lambda.Add(new Lambda<C, M>(tau, interval.metadata) { });

            /// 2nd strategy: this works well ... but a bit slow and dirty !
            /// ------------------------------------------------------------------------------------
            /// KEEP GOING WITH THIS DIRTY ONE BECAUSE THE LATER RAISES SOME ISSUES IN SERIALIZATION
            /// ------------------------------------------------------------------------------------
            /*B<C, M> removingValue;
            di3.TryRemove(key, out removingValue);
            removingValue.lambda.Add(new Lambda<C, M>(tau, interval.metadata));
            di3.Add(key, removingValue);*/

            /// 3rd strategy : based on test fuctions.
            /*KeyValueUpdate<C, B<C, M>> updateFunction = delegate(C Key, B<C, M> newValue)
            {
                B<C, M> block = newValue;
                block.lambda.Add(new Lambda<C, M>(tau, interval.metadata));
                return block;
            };
            di3.TryUpdate(key, updateFunction);
            */
            //if (tau == 'R') di3[key].omega++;

            //test_Maximum_Lambda_Lenght = Math.Max(test_Maximum_Lambda_Lenght, removingValue.lambda.Count);
        }


        struct AddUpdateValue : ICreateOrUpdateValue<int, B>, IRemoveValue<int, B>
        {
            public B OldValue;
            //public string Value;
            public char Tau { set; get; }
            public UInt32 Metadata { set; get; }

            public B NextBlock { set; get; }

            public bool CreateValue(int key, out B value)
            {
                OldValue = null;

                //value = Value;
                value = GetNewBlock();

                return !Metadata.Equals(default(UInt32)) && Tau != default(char); // Value != null;
            }
            public bool UpdateValue(int key, ref B value)
            {
                OldValue = value;

                //value = Value;
                /*if (Tau == 'R')
                    value = value.Update(Omega: value.omega + 1, tau: Tau, metadata: Metadata);
                else
                    value = value.Update(Omega: value.omega, tau: Tau, metadata: Metadata);*/


                //return !Metadata.Equals(default(M)) && Tau != default(char); // Value != null;
                return false;
            }
            public bool RemoveValue(int key, B value)
            {
                OldValue = value;

                //return value == Value;
                if (Tau == 'R')
                    return value == value.Update(Omega: value.omega + 1, tau: Tau, metadata: Metadata);
                else
                    return value == value.Update(Omega: value.omega, tau: Tau, metadata: Metadata);
            }


            private B GetNewBlock()
            {
                //B<C, M> newB;
                if (NextBlock == null)
                    //newB = new B<C, M>(tau: Tau, metadata: Metadata);
                    return new B(tau: Tau, metadata: Metadata);
                else
                    //newB = new B<C, M>(tau: Tau, metadata: Metadata, nextBlock: NextBlock);
                    return new B(tau: Tau, metadata: Metadata, nextBlock: NextBlock);


                // I need to access one block ahead, but since I could not 
                // find any proper way to do so with BPlusTree, I'm using 
                // following ramblings :)
                /*foreach (var block in di3.EnumerateFrom(marshalPoint))//.Skip(0))
                {
                    foreach (var d in block.newValue.lambda)
                    {
                        if (d.tau != 'L')                    
                            newB.lambda.Add(new Lambda<C, M>('M', d.atI) { });                    
                    }

                    // only one object :) 
                    break;
                }*/


                //return newB;
            }


        }
    }










    public class Intervals
    {
        public int left;
        public int right;
        public UInt32 hashKey;
    }
}


