using System;

namespace DI3
{
    /// <summary>
    /// Represents the _interval intersecting with 
    /// the c of corresponding block. 
    /// <para>For _intervals of possibly different types,
    /// it is recommended to define this generic type
    /// parameter in terms of Lowest Common Denominator.
    /// </para>
    /// </summary>
    /// <typeparam name="C">Represents the c/domain
    /// type (e.g,. int, double, Time).</typeparam>
    /// <typeparam name="M">Represents generic
    /// type of pointer to descriptive hashKey cooresponding
    /// to the _interval.</typeparam>
    public struct Lambda
    {
        /// <summary>
        /// Represents the _interval intersecting with 
        /// the c of corresponding block. 
        /// <para>For _intervals of possibly different types,
        /// it is recommended to define this generic type
        /// parameter in terms of Lowest Common Denominator.
        /// </para>
        /// </summary>
        //internal Lambda()
        //{// it seems that the protobuf-net needs this constructor.
        //}

        /// <summary>
        /// Represents the _interval intersecting with 
        /// the c of corresponding block. 
        /// <para>For _intervals of possibly different types,
        /// it is recommended to define this generic type
        /// parameter in terms of Lowest Common Denominator.
        /// </para>
        /// </summary>
        /// <param name="tau">The intersection type of _interval
        /// wtih c of corresponding block.</param>
        /// <param name="atI">Descriptive hashKey of the intereval.</param>
        internal Lambda(char tau, UInt32 atI)
            : this()
        {
            this.tau = tau;
            this.atI = atI;
        }



        /// <summary>
        /// Gets the intersection type of _interval
        /// wtih c of corresponding block.
        /// <para>[currentValue] = L  ::>  Left-end  intersecting the coordiante.</para>
        /// <para>[currentValue] = M  ::>  Middle    intersecting the coordiante.</para>
        /// <para>[currentValue] = R  ::>  Right-end intersecting the coordiante.</para>
        /// </summary>
        internal char tau { private set; get; }

        /// <summary>
        /// Gets descriptive hashKey of the intereval
        /// represented by generic type M.
        /// </summary>
        internal UInt32 atI { private set; get; }
    }
}
