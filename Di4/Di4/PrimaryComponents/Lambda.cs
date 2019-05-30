namespace Polimi.DEIB.VahidJalili.DI4
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
        internal Lambda(Phi phi, uint atI, uint collectionID)
            : this()
        {
            this.phi = phi;
            this.atI = atI;
            this.collectionID = collectionID;
        }


        /// <summary>
        /// Gets the intersection type of interval with e of corresponding keyBookmark.
        /// </summary>
        internal Phi phi { private set; get; }

        /// <summary>
        /// Gets hashKey of the intereval pointing to descriptive atI of 
        /// the interval. The hashkey is represented by generic type M.
        /// </summary>
        internal uint atI { private set; get; }

        internal uint collectionID { private set; get; }
    }
}
