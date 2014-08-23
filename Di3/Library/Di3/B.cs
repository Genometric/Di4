using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI3
{
    /// <summary>
    /// A Block representing relative information of intervals
    /// intersecting with a particular c (i.e., e) on 
    /// domain.</summary>
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
    internal class B<C, I, M> : Lambda<C, I, M>
        where C : ICoordinate<C>
        where I : IInterval<C, M>
    {
        /// <summary>
        /// A Block representing relative information of intervals
        /// intersecting with a particular c (e) on 
        /// domain.
        /// </summary>
        internal B()
        {
            lambda = new List<Lambda<C, I, M>>();
            e = e.defaultValue;
            omega = 0;
        }


        /// <summary>
        /// A Block representing relative information of intervals
        /// intersecting with a particular c (e) on 
        /// domain.
        /// </summary>
        /// <param name="c">Represents the c on
        /// domain</param>
        internal B(C coordinate)
        {
            lambda = new List<Lambda<C, I, M>>();
            e = coordinate;
            omega = 0;
        }


        /// <summary>
        /// Gets the element on domain which the block refers to.
        /// </summary>
        internal C e { private set; get; }


        /// <summary>
        /// Represents the interval intersecting with 
        /// the c of corresponding block.
        /// </summary>
        internal List<Lambda<C, I, M>> lambda { private set; get; }


        /// <summary>
        /// Denotes the number of intervals whose
        /// right-end intersects with e. 
        /// </summary>
        internal int omega { set; get; }
    }
}
