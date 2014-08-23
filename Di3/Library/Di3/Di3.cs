using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI3
{
    /// <summary>
    /// Dynamic intervals inverted index (Di3) 
    /// is an indexing system aimed at providing
    /// efficient means of processing the intervals
    /// it indexes for common information retrieval 
    /// tasks.
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
    internal sealed class Di3<C, I, M> : Di3DataStructure<C, I, M>
        where C : ICoordinate<C>
        where I : IInterval<C, M>
    {
        /// <summary>
        /// Dynamic intervals inverted index (Di3) 
        /// is an indexing system aimed at providing
        /// efficient means of processing the intervals
        /// it indexes for common information retrieval 
        /// tasks.
        /// </summary>
        internal Di3()
        {
            di3 = new List<B<C, I, M>>();
            insertNewInterval = new INDEX<C, I, M>();
        }


        /// <summary>
        /// This instance of the INDEX class is used
        /// for inserting a new interval into di3
        /// </summary>
        private INDEX<C, I, M> insertNewInterval { set; get; }


        /// <summary>
        /// Adds the provided interval to di3.
        /// </summary>
        /// <param name="interval">The interval
        /// to be added to the di3.</param>
        public void AddInterval(I interval)
        {
            insertNewInterval.Index(interval);
        }
    }
}
