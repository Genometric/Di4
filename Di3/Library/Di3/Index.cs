using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI3
{
    /// <summary>
    /// Provides efficient means of inserting an 
    /// interval to Di3; i.e., di3 indexding.
    /// </summary>
    /// <typeparam name="C">Represents the c/domain
    /// type (e.g,. int, double, Time.</typeparam>
    /// <typeparam name="I">Represents generic type of the interval.
    /// (e.g., time span, interval on natural numbers)
    /// <para>For intervals of possibly different types,
    /// it is recommended to define this generic type
    /// parameter in terms of Lowest Common Denominator.
    /// </para></typeparam>
    /// <typeparam name="M">Represents generic
    /// type of pointer to descriptive metadata cooresponding
    /// to the interval.</typeparam>
    internal class INDEX<C, I, M> : Di3DataStructure<C, I, M>
        where C : ICoordinate<C>
        where I : IInterval<C, M>
    {
        /// <summary>
        /// Represents the coordinate (right or left-end) of interval being
        /// inserted to di3.
        /// </summary>
        private C c { set; get; }


        /// <summary>
        /// Represents the index of di3.
        /// </summary>
        private int b { set; get; }


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
        /// Indexes the provided interval. 
        /// </summary>
        /// <param name="interval">The interval to be index.</param>
        public void Index(I interval)
        {
            this.interval = interval;

            c = interval.left;

            b = FindLeftendInsertCoordinate();
            Insert_into_di3('L');
            b++;

            c = interval.right;
            FindRightendInsertCoordinate();
            Insert_into_di3('R');
        }


        /// <summary>
        /// Finds the proper coordiante on B for insertion of 
        /// Left-end of the interval through a dichotomic search. 
        /// </summary>
        private int FindLeftendInsertCoordinate()
        {
            di3Cardinality = di3.Count;

            int left = 0;
            int mid = 0;
            int right = di3Cardinality;

            while (left < right)
            {
                mid = (int)Math.Floor((left + right) / 2.0);

                if (mid == right)
                {
                    return mid;
                }
                else if (c.CompareTo(di3[mid].e) == 2) // e is less than Di3DataStructure[mid].e
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
                else if (c.CompareTo(di3[mid].e) == 1) // e is equal to Di3DataStructure[mid].e
                {
                    return mid;
                }
                else
                {
                    left = mid;

                    if (left == right - 1)
                    {
                        if (c.CompareTo(di3[mid].e) == 2) // e is less than Di3DataStructure[mid].e
                        {
                            return left;
                        }
                        else
                        {
                            if (right < di3Cardinality)
                            {
                                if (c.CompareTo(di3[mid].e) == 2) // e is less than Di3DataStructure[mid].e
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
            }

            return di3Cardinality;
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
            // Shall new Block be added to the end of list ?
            if (b < di3Cardinality) // No
            {
                // Does the same index already available ?
                if (c.CompareTo(di3[b].e) != 1) // No, add new index then
                {
                    di3.Insert(b, GetNewBlock(tau));
                    di3Cardinality++;
                }
                else // Yes, then update the available index with new region
                {
                    UpdateBlock(tau);
                }
            }
            else // Yes, then add new index
            {
                di3.Insert(b, GetNewBlock(tau));
                di3Cardinality++;
            }
        }


        /// <summary>
        /// Initializes and returns a new block
        /// to be added to di3.
        /// </summary>
        /// <param name="tau">The intersection type of interval
        /// wtih c of corresponding block.</param>
        /// <returns>A new block to be added to di3.</returns>
        private B<C, I, M> GetNewBlock(char tau)
        {
            B<C, I, M> newB = new B<C, I, M>(c) { };

            // Will new Block be added to the end of di3 list ?
            if (b < di3Cardinality) // No, then copy data from subsequent Block
            {
                foreach (var d in di3[b].lambda)
                {
                    if (d.tau != 'L')
                    {
                        newB.lambda.Add(new Lambda<C, I, M>('M', d.atI) { });
                    }
                }
            }

            return newB;
        }


        /// <summary>
        /// Linearly searches di3 starting from left-end
        /// insertion index of the interval toward di3 end for 
        /// proper postion for the interval's right-end insertion.
        /// </summary>
        private void FindRightendInsertCoordinate()
        {
            for (int i = b + 1; i < di3Cardinality; i++)
            {
                if (c.CompareTo(di3[i].e) == 'L')
                {
                    b = i;
                    break;
                }
                else if (c.CompareTo(di3[i].e) == 'G')
                {
                    UpdateBlock('M');

                    b++;
                }
                else
                {
                    break;
                }
            }
        }


        /// <summary>
        /// Updates the di3 block specified by 'b' 
        /// to represent the interval being indexed as well. 
        /// </summary>
        /// <param name="tau">The intersection type of interval
        /// wtih c of corresponding block.</param>
        private void UpdateBlock(char tau)
        {
            di3[b].lambda.Add(new Lambda<C, I, M>(tau, interval.metadata) { });

            if (tau == 'R')
            {
                di3[b].omega++;
            }
        }
    }
}
