using System;

namespace Polimi.DEIB.VahidJalili.DI3
{
    /// <summary>
    /// Represents the interval intersecting with 
    /// the e of corresponding bookmark. 
    /// <para>For intervals of possibly different types,
    /// it is recommended to define this generic type
    /// parameter in terms of Lowest Common Denominator.
    /// </para>
    /// </summary>
    public struct Lambda
    {
        /// <summary>
        /// Represents the interval intersecting with 
        /// the e of corresponding bookmark. 
        /// <para>For intervals of possibly different types,
        /// it is recommended to define this generic type
        /// parameter in terms of Lowest Common Denominator.
        /// </para>
        /// </summary>
        /// <param name="phi">The intersection type of interval
        /// wtih c of corresponding bookmark.</param>
        /// <param name="atI">Descriptive hashKey of the intereval.</param>
        internal Lambda(char tau, UInt32 atI)
            : this()
        {
            this.phi = tau;
            this.atI = atI;
        }


        /// <summary>
        /// Gets the intersection type of _interval
        /// wtih c of corresponding bookmark.
        /// <para>[currentValue] = L  ::>  Left-end  intersecting the coordiante.</para>
        /// <para>[currentValue] = M  ::>  Middle    intersecting the coordiante.</para>
        /// <para>[currentValue] = R  ::>  Right-end intersecting the coordiante.</para>
        /// </summary>
        internal char phi { private set; get; }

        /// <summary>
        /// Gets descriptive hashKey of the intereval
        /// represented by generic type M.
        /// </summary>
        internal UInt32 atI { private set; get; }
    }
}
