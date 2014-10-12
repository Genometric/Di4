using System;
using System.Collections.Generic;
using ProtoBuf;
using Interfaces;

namespace DI3
{
    /// <summary>
    /// A Block representing relative information of intervals
    /// intersecting with a particular c (i.e., e) on 
    /// domain.</summary>
    /// <typeparam name="C">Represents the c/domain
    /// type (e.g,. int, double, Time).</typeparam>
    /// <typeparam name="M">Represents generic
    /// type of pointer to descriptive metadata cooresponding
    /// to the interval.</typeparam>
    [ProtoContract]
    public class B<C, M>
        where C : IComparable<C>
        where M : IMetaData<C>
    {
        /// <summary>
        /// A Block representing relative information of intervals
        /// intersecting with a particular c (e) on domain.</summary>
        internal B()
        {// it seems that the protobuf net needs this constructor. 
            lambda = new List<Lambda<C, M>>();
        }

        /// <summary>
        /// A Block representing relative information of intervals
        /// intersecting with a particular c (e) on domain.</summary>
        /// <param name="coordinate">Represents the c on domain.</param>
        internal B(C coordinate)
        {
            //e = coordinate;
            omega = 0;
            lambda = new List<Lambda<C, M>>();
        }



        public object TESTGETTotalMemory()
        {
            return GC.GetTotalMemory(true);
        }



        /// <summary>
        /// Gets the element of domain which the block refers to.
        /// </summary>
        //[ProtoMember(1)]
        //internal C e { set; get; }

        /// <summary>
        /// Denotes the number of intervals whose
        /// right-end intersects with e. 
        /// </summary>
        [ProtoMember(1)]
        internal int omega { set; get; }

        /// <summary>
        /// Represents the interval intersecting with 
        /// the c of corresponding block.
        /// </summary>
        [ProtoMember(2)]
        internal List<Lambda<C, M>> lambda { set; get; }
    }
}
