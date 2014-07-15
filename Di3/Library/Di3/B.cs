using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3
{
    /// <summary>
    /// A Block representing relative information of intervals
    /// intersecting with a particular coordinate (e) on 
    /// domain.
    /// </summary>
    /// <typeparam name="C">Represents the coordinate/domain type (e.g,. int, double, Time</typeparam>
    /// <typeparam name="I">Represents interval type.</typeparam>
    internal class B<C, I> : Lambda<I>
    {
        /// <summary>
        /// A Block representing relative information of intervals
        /// intersecting with a particular coordinate (e) on 
        /// domain.
        /// </summary>
        /// <param name="coordinate">Represents the coordinate on
        /// domain</param>
        internal B(C coordinate)
        {
            lambda = new List<Lambda<I>>();
            e = coordinate;
        }

        /// <summary>
        /// A Block representing relative information of intervals
        /// intersecting with a particular coordinate (e) on 
        /// domain.
        /// </summary>
        internal B() { }

        /// <summary>
        /// Gets the element on domain which the block refers to.
        /// </summary>
        internal C e { private set; get; }

        /// <summary>
        /// Represents the interval intersecting with 
        /// the coordinate of corresponding block.
        /// </summary>
        internal List<Lambda<I>> lambda { private set; get; }

        /// <summary>
        /// Denotes the number of intervals whose
        /// right-end intersects with e. 
        /// </summary>
        internal int omega { private set; get; }
    }
}
