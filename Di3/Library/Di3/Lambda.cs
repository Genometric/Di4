using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3
{
    /// <summary>
    /// Represents the interval intersecting with 
    /// the coordinate of corresponding block. 
    /// <para>For intervals of possibly different types,
    /// it is recommended to define this generic type
    /// parameter in terms of Lowest Common Denominator.
    /// </para>
    /// </summary>
    /// <typeparam name="I">Denotes generic type of the interval.</typeparam>
    internal class Lambda<C, I> where I : IInterval<C>
    {
        /// <summary>
        /// Gets the type of intersection the interval has
        /// wtih coordinate of the block it corresponds to. 
        /// <para>value = 0  ::>  Left-end  intersects.</para>
        /// <para>value = 1  ::>  middle    intersects.</para>
        /// <para>value = 2  ::>  Right-end intersects.</para>
        /// </summary>
        public byte Tau { internal set; get; }

        /// <summary>
        /// Gets descriptive metadata of the intereval
        /// represented by generic type I.
        /// </summary>
        public I AtI { internal set; get; }
    }
}
