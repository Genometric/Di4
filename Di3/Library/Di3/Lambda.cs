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
    /// </summary>
    /// <typeparam name="I">Denotes generic type of the interval.</typeparam>
    internal class Lambda<I>
    {
        /// <summary>
        /// Represents the interval intersecting with 
        /// the coordinate of corresponding block.
        /// </summary>
        /// <param name="tau">
        /// The type of intersection the interval has
        /// wtih coordinate of the block it corresponds to. 
        /// <para>value = 0  ::>  Left-end  intersects.</para>
        /// <para>value = 1  ::>  middle    intersects.</para>
        /// <para>value = 2  ::>  Right-end intersects.</para>
        /// </param>
        /// <param name="atI">
        /// Descriptive metadata of the intereval
        /// represented by generic type I.
        /// </param>
        public Lambda(byte tau, I atI)
        {
            this.tau = tau;
            this.atI = atI;
        }

        /// <summary>
        /// Gets the type of intersection the interval has
        /// wtih coordinate of the block it corresponds to. 
        /// <para>value = 0  ::>  Left-end  intersects.</para>
        /// <para>value = 1  ::>  middle    intersects.</para>
        /// <para>value = 2  ::>  Right-end intersects.</para>
        /// </summary>
        public byte tau { internal set; get; }

        /// <summary>
        /// Gets descriptive metadata of the intereval
        /// represented by generic type I.
        /// </summary>
        public I atI { internal set; get; }
    }
}
