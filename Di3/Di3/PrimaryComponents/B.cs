using System;
using System.Collections.Generic;
using ProtoBuf;
using Interfaces;
using System.Collections.ObjectModel;

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
        where M : IMetaData/*<C>*/
    {
        /// <summary>
        /// A Block representing relative information of intervals
        /// intersecting with a particular c (e) on domain.</summary>
        internal B()
        {// it seems that the protobuf net needs this constructor. 
            omega = 0;
            _lambda = new List<Lambda<C, M>>();
        }

        /// <summary>
        /// A Block representing relative information of intervals
        /// intersecting with a particular c (e) on domain.</summary>
        /// <param name="coordinate">Represents the c on domain.</param>
        internal B(C coordinate) // this constructor is redundant, because there is no Coordinate anymore. 
        {
            //e = coordinate;
            omega = 0;
            _lambda = new List<Lambda<C, M>>();
        }

        internal B(int omega, char tau, M metadata)
        { // initializes a block and adds one Lambda to lambda according to tau and metadata.
            this.omega = omega;
            _lambda = new List<Lambda<C, M>>();
            _lambda.Add(new Lambda<C, M>(tau, metadata));
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
        internal int omega { private set; get; }

        /// <summary>
        /// Represents the interval intersecting with 
        /// the c of corresponding block.
        /// </summary>
        [ProtoMember(2)]
        private List<Lambda<C, M>> _lambda { set; get; }

        internal ReadOnlyCollection<Lambda<C, M>> lambda { get { return _lambda.AsReadOnly(); } }



        internal B<C, M> Update(int Omega)
        {
            B<C, M> newBlock = new B<C, M>();

            /// update it with new Omega.
            newBlock.omega = Omega;

            /// deep copy the current one.
            newBlock._lambda = new List<Lambda<C, M>>(this._lambda);

            return newBlock;
        }

        internal B<C, M> Update(char tau, M metadata)
        {
            B<C, M> newBlock = new B<C, M>();

            /// deep copy the current one.
            newBlock._lambda = new List<Lambda<C, M>>(this._lambda);

            /// update it with new Lambda.
            newBlock._lambda.Add(new Lambda<C, M>(tau: tau, atI: metadata));

            return newBlock;
        }

        internal B<C, M> Update(int Omega, char tau, M metadata)
        {
            B<C, M> newBlock = new B<C, M>();

            /// update it with new Omega.
            newBlock.omega = Omega;

            /// deep copy the current one.
            newBlock._lambda = new List<Lambda<C, M>>(this._lambda);

            /// update it with new Lambda.
            newBlock._lambda.Add(new Lambda<C, M>(tau: tau, atI: metadata));

            return newBlock;
        }
    }
}
