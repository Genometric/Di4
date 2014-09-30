using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IInterval;
using ICPMD;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System.IO;

namespace DI3
{
    /// <summary>
    /// Provides efficient means of inserting an 
    /// interval to Di3; i.e., di3 indexding.
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
    internal class INDEX<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : IMetaData<C>
    {
        /// <summary>
        /// Provides efficient means of inserting an 
        /// interval to Di3; i.e., di3 indexding.
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
        /// Represents the coordinate (right or left-end) of interval being
        /// inserted to di3.
        /// </summary>
        private C c { set; get; }


        /// <summary>
        /// Represents the index of di3.
        /// </summary>
        private int b { set; get; }


        private C marshalPoint { set; get; }


        /// <summary>
        /// Sets and Gets the interval to 
        /// be added to di3. 
        /// </summary>
        private I interval { set; get; }


        /// <summary>
        /// Sets and Gets the cardinality of 
        /// di3.
        /// </summary>
        private int di3Cardinality { set; get; }

        /// <summary>
        /// The left and right ends of an indexed 
        /// interval update at least two blocks. 
        /// This variable holds the index of left
        /// and right end indexes of such updated 
        /// blocks cause by the "NEW" interval index.
        /// <para>This information is used to improve
        /// indexing performance for sorted input.</para>
        /// </summary>
        //private int[] newIndexes { set; get; }

        /// <summary>
        /// The left and right ends of an indexed 
        /// interval update at least two blocks. 
        /// This variable holds the index of left
        /// and right end indexes of such updated 
        /// blocks cause by the "PREVIOUS" interval index.
        /// <para>This information is used to improve
        /// indexing performance for sorted input.</para>
        /// </summary>
        //private int[] preIndexes { set; get; }


        /// <summary>
        /// Indexes the provided interval. 
        /// </summary>
        /// <param name="interval">The interval to be index.</param>
        public void Index(I Interval)//, int[] previousIndexes)
        {
            //preIndexes = previousIndexes;

            interval = Interval;

            //c = interval.left;
            marshalPoint = interval.left;

            //b = FindLeftendInsertCoordinate();
            Insert_into_di3('L');
            //newIndexes[0] = b;
            //b++;

            marshalPoint = interval.right;
            MarchForRightEnd();
            Insert_into_di3('R');
            //newIndexes[1] = b;

            //return newIndexes;
        }


        /// <summary>
        /// Finds the proper coordiante on B for insertion of 
        /// Left-end of the interval through a dichotomic search. 
        /// </summary>
        private int FindLeftendInsertCoordinate()
        {
            //di3Cardinality = di3.Count;

            /*if (di3Cardinality == 0) return 0;

            int left = 0;
            int mid = 0;
            int right = di3Cardinality;*/
            
            /*if (c.CompareTo(di3[preIndexes[0]].e) == -1)
            {
                right = preIndexes[0];
            }
            else if (c.CompareTo(di3[preIndexes[1]].e) == 1)
            {
                left = preIndexes[1];
            }
            else
            {
                left = preIndexes[0];
                right = preIndexes[1];
            }*/
            /*
            while (left < right)
            {
                mid = (int)Math.Floor((left + right) / 2.0);

                if (mid == right)
                {
                    return mid;
                }
                else if (c.CompareTo(di3[mid].e) == -1) // c is less than di3[mid].e
                {
                    if (mid == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        right = mid;
                    }
                }
                else if (c.CompareTo(di3[mid].e) == 0) // c is equal to di3[mid].e
                {
                    return mid;
                }
                else
                {
                    left = mid;

                    if (left == right - 1)
                    {
                        if (c.CompareTo(di3[left].e) == -1) // c is less than di3[mid].e
                        {
                            return left;
                        }
                        else
                        {
                            if (right < di3Cardinality)
                            {
                                if (c.CompareTo(di3[right].e) == -1) // c is less than di3[mid].e
                                {
                                    return right;
                                }
                                else
                                {
                                    return right + 1;
                                }
                            }
                            else
                            {
                                return right;
                            }
                        }
                    }
                }
            }*/

            // This value will be returned when di3 is empty. 
            return di3Cardinality;
        }


        /// <summary>
        /// Linearly searches di3 starting from left-end
        /// insertion index of the interval toward di3 end for 
        /// proper postion for the interval's right-end insertion.
        /// </summary>
        private void MarchForRightEnd()
        {
            // Enumerator starts enumeration from given coordinate (i.e., e); 
            // however, this modification does not apply to the block corresponding
            // coordinate "c". Hence, this boolean value is used to  "exclusively" 
            // enumerate from coordinate "c". 
            bool skip = true;

            foreach (var block in di3.EnumerateFrom(interval.left))
            {
                if (!skip)
                {
                    switch(marshalPoint.CompareTo(block.Key))
                    {
                        case 0:  // equal
                        case -1: // greater than
                            return;

                        case 1:  // less than
                            UpdateBlock(block.Key, 'M');
                            break;

                    }
                    /*if (marshalPoint.CompareTo(block.Key) == -1)
                    {
                        //marshalPoint = block.Key;
                        //b = i;
                        break;
                    }
                    else if (marshalPoint.CompareTo(block.Key) == 1)
                    {
                        UpdateBlock(block.Key, 'M');
                        //marshalPoint = block.Key;
                        //b++;
                    }
                    else
                    {
                        break;
                    }*/
                }
                else { skip = false; }
            }
        }


        /// <summary>
        /// Inserts right/left -end of the interval to 
        /// determined position on di3 by either updating
        /// present block or creating a new block.
        /// </summary>
        /// <param name="tau">The intersection type of interval
        /// wtih c of corresponding block.</param>
        private void Insert_into_di3(char tau)
        {
            // Shall new Block be added to the end of list ? OR: Does the same index already available ?
            if (di3.ContainsKey(marshalPoint))// b >= di3Cardinality || c.CompareTo(di3[b].e) != 0) // Condition satisfied: add new index
            {
                UpdateBlock(marshalPoint, tau);
            }
            else // update the available index with new region
            {
                di3.Add(marshalPoint, GetNewBlock(tau));
                //di3.Insert(b, GetNewBlock(tau));
                //di3Cardinality++;
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
            foreach (var block in di3.EnumerateFrom(marshalPoint))
            {
                foreach (var d in block.Value.lambda)
                {
                    if (d.tau != 'L')
                    {
                        newB.lambda.Add(new Lambda<C, M>('M', d.atI) { });
                    }
                }

                // only one object :) 
                break;
            }

            
            /*
            // Will new Block be added to the end of di3 list ?
            if (b < di3Cardinality) // No, then copy data from subsequent Block
            {
                foreach (var d in di3[c].lambda)
                {
                    if (d.tau != 'L')
                    {
                        newB.lambda.Add(new Lambda<C, M>('M', d.atI) { });
                    }
                }
            }*/

            return newB;
        }


        /// <summary>
        /// Updates the di3 block specified by 'b' 
        /// to represent the interval being indexed as well. 
        /// </summary>
        /// <param name="tau">The intersection type of interval
        /// wtih c of corresponding block.</param>
        private void UpdateBlock(C key, char tau)
        {
            // Following line seems to be updating the "value" of "this.key = the key"
            // and based on my tests it works fine both with in-memory and 
            // out-of-memory policies; however, still it need further assessment.
            di3[key].lambda.Add(new Lambda<C, M>(tau, interval.metadata) { });

            //2nd strategy (not recommended):
            //List<Lambda<C, M>> tempLambda = di3[c].lambda;
            //tempLambda.Add(new Lambda<C, M>(tau, interval.metadata) { });
            //di3.TryUpdate(c, tempLambda);

            if (tau == 'R')
            {
                di3[key].omega++;
            }
        }
    }
}
