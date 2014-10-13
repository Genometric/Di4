using System;
using System.Linq;
using Interfaces;
using CSharpTest.Net.Collections;


namespace DI3
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
    class INDEX<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : IMetaData<C>
    {
        /// <summary>
        /// Provides efficient means of inserting an 
        /// interval to DI3; i.e., di3 indexding.
        /// </summary>
        /// <param name="di3">The reference di3 to be 
        /// manipulated.</param>
        internal INDEX(BPlusTree<C, B<C, M>> di3)
        {
            this.di3 = di3;
        }

        /// <summary>
        /// Sets and gets the di3 data structure
        /// to be manipulated. This data structure
        /// is in common between all classes of 
        /// namespace.
        /// </summary>
        private BPlusTree<C, B<C, M>> di3 { set; get; }

        /// <summary>
        /// Represents the coordinate (right or left-end)
        /// of interval being inserted to di3.
        /// </summary>
        private C marshalPoint { set; get; }

        /// <summary>
        /// Sets and Gets the interval to 
        /// be added to di3. 
        /// </summary>
        private I interval { set; get; }

        /// <summary>
        /// Sets and Gets the cardinality of di3.
        /// </summary>
        private int di3Cardinality { set; get; }

        /// <summary>
        /// Indexes the provided interval.
        /// </summary>
        /// <param name="Interval">The interval to be index.</param>
        public void Index(I Interval)
        {
            interval = Interval;

            marshalPoint = interval.left;
            Insert('L');

            marshalPoint = interval.right;
            MarchForRightEnd();
            Insert('R');
        }


        /// <summary>
        /// Linearly searches di3 starting from left-end
        /// insertion index of the interval toward di3 end for 
        /// proper postion for the interval's right-end insertion.
        /// </summary>
        private void MarchForRightEnd()
        {
            foreach (var block in di3.EnumerateFrom(interval.left).Skip(1))
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
            }
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
            // Shall new Block be added to the end of list ? OR: Does the same index already available ?
            if (di3.ContainsKey(marshalPoint)) // Condition satisfied: add new index
            {
                UpdateBlock(marshalPoint, tau);
            }
            else // update the available index with new region
            {
                di3.Add(marshalPoint, GetNewBlock(tau));
            }
        }


        /// <summary>
        /// Initializes and returns a new block
        /// to be added to di3.
        /// </summary>
        /// <param name="tau">The intersection type of interval
        /// wtih c of corresponding block.</param>
        /// <returns>A new block to be added to di3.</returns>
        private B<C, M> GetNewBlock(char tau)
        {
            B<C, M> newB = new B<C, M>(marshalPoint) { };
            newB.lambda.Add(new Lambda<C, M>(tau, interval.metadata));

            if (tau == 'R') newB.omega = 1;

            // I need to access one item ahead, but since I could not 
            // find any proper way to do so with BPlusTree, I'm using 
            // following ramblings :)
            foreach (var block in di3.EnumerateFrom(marshalPoint))//.Skip(0))
            {
                foreach (var d in block.Value.lambda)
                {
                    if (d.tau != 'L')                    
                        newB.lambda.Add(new Lambda<C, M>('M', d.atI) { });                    
                }

                // only one object :) 
                break;
            }

            return newB;
        }

        /// <summary>
        /// Updates the di3 block specified by 'b' 
        /// to represent the interval being indexed as well.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="tau">The intersection type of interval
        /// wtih c of corresponding block.</param>
        private void UpdateBlock(C key, char tau)
        {
            /// 1st Strategy: is the old version; it cause errors because it's not actually updateing. 
            //di3[key].lambda.Add(new Lambda<C, M>(tau, interval.metadata) { });

            /// 2nd strategy: this works well ... but a bit slow and dirty !
            /// ------------------------------------------------------------------------------------
            /// KEEP GOING WITH THIS DIRTY ONE BECAUSE THE LATER RAISES SOME ISSUES IN SERIALIZATION
            /// ------------------------------------------------------------------------------------
            B<C, M> removingValue;
            di3.TryRemove(key, out removingValue);
            removingValue.lambda.Add(new Lambda<C, M>(tau, interval.metadata));
            di3.Add(key, removingValue);

            /// 3rd strategy : based on test fuctions.
            /*KeyValueUpdate<C, B<C, M>> updateFunction = delegate(C Key, B<C, M> Value)
            {
                B<C, M> block = Value;
                block.lambda.Add(new Lambda<C, M>(tau, interval.metadata));
                return block;
            };
            di3.TryUpdate(key, updateFunction);*/

            if (tau == 'R') di3[key].omega++;
        }
    }
}
