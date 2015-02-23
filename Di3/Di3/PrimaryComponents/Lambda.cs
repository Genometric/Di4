using System;

namespace Polimi.DEIB.VahidJalili.DI3
{
    /// <summary>
    /// Represents the interval intersecting with 
    /// the e of corresponding keyBookmark. 
    /// <para>For intervals of possibly different types,
    /// it is recommended to define this generic type
    /// parameter in terms of Lowest Common Denominator.
    /// </para>
    /// </summary>
    public struct Lambda
    {
        /// <summary>
        /// Represents the interval intersecting with 
        /// the e of corresponding keyBookmark. 
        /// <para>For intervals of possibly different types,
        /// it is recommended to define this generic type
        /// parameter in terms of Lowest Common Denominator.
        /// </para>
        /// </summary>
        /// <param name="phi">The intersection type of interval
        /// wtih e of corresponding keyBookmark.</param>
        /// <param name="atI">Descriptive hashKey of the intereval.</param>
        internal Lambda(bool phi, UInt32 atI)
            : this()
        {
            this.phi = phi;
            this.atI = atI;
        }


        /// <summary>
        /// Gets the intersection type of interval wtih e of corresponding keyBookmark.
        /// <para>[currentValue] = true   ::>  Left-end  intersecting the coordiante.</para>
        /// <para>[currentValue] = false  ::>  Right-end intersecting the coordiante.</para>
        /// </summary>
        internal bool phi { private set; get; }

        /// <summary>
        /// Gets hashKey of the intereval pointing to descriptive atI of 
        /// the interval. The hashkey is represented by generic type M.
        /// </summary>
        internal UInt32 atI { private set; get; }
    }
}
