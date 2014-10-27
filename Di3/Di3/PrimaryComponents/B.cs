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
            //_atI = new M[10];
            //_tau = new char[10];
            //_lambda = new List<Lambda<C, M>>();
        }

        /// <summary>
        /// A Block representing relative information of intervals
        /// intersecting with a particular c (e) on domain.</summary>
        /// <param name="coordinate">Represents the c on domain.</param>
        internal B(C coordinate) // this constructor is redundant, because there is no Coordinate anymore. 
        {
            //e = coordinate;
            omega = 0;
            //_tau = new char[10];
            //_atI = new M[10];
            //_lambda = new List<Lambda<C, M>>();
        }

        internal B(char tau, M metadata)
        { // initializes a block and adds one Lambda to lambda according to tau and metadata.
            if (tau == 'R') omega = 1;
            else omega = 0;
            //_tau = new char[10];
            //_atI = new M[10];
            //_tau[0]=tau;
            //_atI[0]=metadata;
            //_lambda = new List<Lambda<C, M>>();
            //_lambda.Add(new Lambda<C, M>(tau: tau, atI: metadata));
        }

        internal B(char tau, M metadata, B<C, M> nextBlock) // check whether it is faster with ref or without ref for nextBlock. 
        {
            if (tau == 'R') omega = 1;//nextBlock.omega + 1;
            //else omega = nextBlock.omega;
            //_tau = new char[10];
            //_atI = new M[10];
            //_tau[0]= tau;
            //_atI[0]= metadata;
            //_lambda.Add(new Lambda<C, M>(tau: tau, atI: metadata));
           /* foreach (var item in nextBlock.Tau)
                if (item != 'L')
                {
                    _tau.Add('M');
                }*/
                    //_lambda.Add(new Lambda<C, M>(tau: 'M', atI: item.atI));
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
        //[ProtoMember(2)]
        //private List<Lambda<C, M>> _lambda { set; get; }
        //internal ReadOnlyCollection<Lambda<C, M>> lambda { get { return _lambda.AsReadOnly(); } }

        //[ProtoMember(2)]
        //private char[] _tau { set; get; }
        //internal ReadOnlyCollection<char> Tau { get { return _tau.AsReadOnly(); } }

        //[ProtoMember(3)]
        //private M[] _atI { set; get; }
        //internal ReadOnlyCollection<M> atI { get { return _atI.AsReadOnly(); } }




        internal B<C, M> Update(int Omega)
        {
            B<C, M> newBlock = new B<C, M>();

            /// update it with new Omega.
            newBlock.omega = Omega;

            /// deep copy the current one.
            //newBlock._lambda = new List<Lambda<C, M>>(this._lambda);
            //newBlock._tau = new char[10];
            //newBlock._tau[0] = this._tau[0];
            //newBlock._atI = new M[10];
            //newBlock._atI[0] = this._atI[0];

            return newBlock;
        }

        internal B<C, M> Update(char tau, M metadata)
        {
            B<C, M> newBlock = new B<C, M>();

            /// deep copy the current one.
            //newBlock._lambda = new List<Lambda<C, M>>(this._lambda);
            //newBlock._tau = new char[10];
            //newBlock._tau[0] = this._tau[0];
            //newBlock._atI = new M[10];
            //newBlock._atI[0] = this._atI[0];

            /// update it with new Lambda.
            //newBlock._lambda.Add(new Lambda<C, M>(tau: tau, atI: metadata));
            //newBlock._tau[1] = tau;
            //newBlock._atI[1] = metadata;

            return newBlock;
        }

        internal B<C, M> Update(int Omega, char tau, M metadata)
        {
            B<C, M> newBlock = new B<C, M>();

            /// update it with new Omega.
            newBlock.omega = Omega;

            /// deep copy the current one.
            //newBlock._lambda = new List<Lambda<C, M>>(this._lambda);
            //newBlock._tau = new char[10];
            //newBlock._tau[0] = this._tau[0];
            //newBlock._atI = new M[10];
            //newBlock._atI[0] = this._atI[0];

            /// update it with new Lambda.
            //newBlock._lambda.Add(new Lambda<C, M>(tau: tau, atI: metadata));
            //newBlock._tau[1] = tau;
            //newBlock._atI[1] = metadata;

            return newBlock;
        }

        /*
        internal B<C, M> Update(Dictionary<uint, Lambda<C, M>> lambdas)
        {
            B<C, M> newBlock = new B<C, M>();
            newBlock.omega = this.omega;
            //newBlock._lambda = new List<Lambda<C, M>>();
            newBlock._tau = new List<char>();
            newBlock._atI = new List<M>();

            foreach (var l in lambdas)
            {
                //newBlock._lambda.Add(new Lambda<C, M>(tau: l.Value.tau, atI: l.Value.atI));
                newBlock._atI.Add(l.Value.atI);
                newBlock._tau.Add(l.Value.tau);
            }

            return newBlock;
        }*/
    }
}
