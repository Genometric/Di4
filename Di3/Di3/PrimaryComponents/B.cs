using System;
using System.Collections.Generic;
//using ProtoBuf;
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
    //[ProtoContract]
    public class B//<C, M>
        //where C : IComparable<C>
        //where M : IMetaData/*<C>*/, new()
    {
        /// <summary>
        /// A Block representing relative information of intervals
        /// intersecting with a particular c (e) on domain.</summary>
        internal B()
        {// it seems that the protobuf net needs this constructor. 
            omega = 0;
            _lambda = new Lambda[0];//new List<Lambda>();
        }

        /// <summary>
        /// A Block representing relative information of intervals
        /// intersecting with a particular c (e) on domain.</summary>
        /// <param name="coordinate">Represents the c on domain.</param>
        /*internal B(C coordinate) // this constructor is redundant, because there is no Coordinate anymore. 
        {
            //e = coordinate;
            omega = 0;
            _lambda = new Lambda[0];//new List<Lambda>();
        }*/

        internal B(char tau, UInt32 metadata)
        { // initializes a block and adds one Lambda to lambda according to tau and metadata.
            if (tau == 'R') omega = 1;
            else omega = 0;
            _lambda = new Lambda[] { new Lambda(tau: tau, atI: metadata) };//new List<Lambda>();
            //_lambda.Add(new Lambda(tau: tau, atI: metadata));
        }

        internal B(int omega, ReadOnlyCollection<Lambda> lambda)
        {
            omega = 0;
            _lambda = new Lambda[lambda.Count];

            for (int i = 0; i < lambda.Count; i++)
            {
                _lambda[i] = lambda[i];
                if (lambda[i].tau == 'R')
                    omega++;
            }
        }

        internal B(char tau, UInt32 metadata, B nextBlock) // check whether it is faster with ref or without ref for nextBlock. 
        {
            if (tau == 'R') omega = 1;//nextBlock.omega + 1;
            //else omega = nextBlock.omega;


            /// Following method has 2 ints and one extra read on 
            /// nextBlock.lambda. I prefered this method over `copying
            /// all items satisfying the condition to a list and 
            /// converting a list to array` because this method has
            /// less footprints and copies lambda only once. 
            int i = 0;
            foreach (var item in nextBlock.lambda)
                if (item.tau != 'L')
                    i++;
            _lambda = new Lambda[i];
            i = 0;
            foreach (var item in nextBlock.lambda)
                if (item.tau != 'L')
                    _lambda[i++] = new Lambda(tau: 'M', atI: item.atI);


            /// Fix this, note lambda size can't be determined at begining because it depends on nextblock lambdas' satisfying the condition.
            //_lambda = new List<Lambda>();
            //_lambda.Add(new Lambda(tau: tau, atI: metadata));
            //foreach (var item in nextBlock.lambda)
            //    if (item.tau != 'L')
            //        _lambda.Add(new Lambda(tau: 'M', atI: item.atI));
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
        //[ProtoMember(1)]
        internal int omega { private set; get; }

        /// <summary>
        /// Represents the interval intersecting with 
        /// the c of corresponding block.
        /// </summary>
        //[ProtoMember(2)]
        //private List<Lambda> _lambda { set; get; }
        private Lambda[] _lambda { set; get; }

        internal ReadOnlyCollection<Lambda> lambda { get { return Array.AsReadOnly(_lambda); } }



        internal B Update(int Omega)
        {
            B newBlock = new B();

            /// update it with new Omega.
            newBlock.omega = Omega;

            /// deep copy the current one.
            //newBlock._lambda = new List<Lambda>(this._lambda);

            return newBlock;
        }

        internal B Update(char tau, UInt32 metadata)
        {
            B newBlock = new B();

            /// deep copy the current one.
            //newBlock._lambda = new List<Lambda>(this._lambda);

            /// update it with new Lambda.
            //newBlock._lambda.Add(new Lambda(tau: tau, atI: metadata));

            return newBlock;
        }

        internal B Update(int Omega, char tau, UInt32 metadata)
        {
            B newBlock = new B();

            /// update it with new Omega.
            newBlock.omega = Omega;

            /// deep copy the current one.
            //newBlock._lambda = new List<Lambda>(this._lambda);

            /// update it with new Lambda.
            //newBlock._lambda.Add(new Lambda(tau: tau, atI: metadata));

            return newBlock;
        }

        internal B Update(Dictionary<uint, Lambda> lambdas)
        {
            B newBlock = new B();
            newBlock.omega = this.omega;
            //newBlock._lambda = new List<Lambda>();
            //foreach (var l in lambdas)
                //newBlock._lambda.Add(new Lambda(tau: l.Value.tau, atI: l.Value.atI));

            return newBlock;
        }
    }
}
