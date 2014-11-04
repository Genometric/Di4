using Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3WithCustomBlockSerializer
{
    public class B
       // where C : IComparable<C>
        //where M : IMetaData/*<C>*/
    {
        /// <summary>
        /// A Block representing relative information of intervals
        /// intersecting with a particular c (e) on domain.</summary>
        internal B()
        {// it seems that the protobuf net needs this constructor. 
            omega = 0;
            ////_lambda = new Lambda[0];//new List<Lambda<C, M>>();
        }

        /// <summary>
        /// A Block representing relative information of intervals
        /// intersecting with a particular c (e) on domain.</summary>
        /// <param name="coordinate">Represents the c on domain.</param>
        /*internal B(C coordinate) // this constructor is redundant, because there is no Coordinate anymore. 
        {
            //e = coordinate;
            omega = 0;
            _lambda = new Lambda<M>[0];//new List<Lambda<C, M>>();
        }*/

        internal B(char tau, UInt32 metadata)
        { // initializes a block and adds one Lambda to lambda according to tau and metadata.
            if (tau == 'R') omega = 1;
            else omega = 0;
            ////_lambda = new Lambda[] { new Lambda(tau, metadata) };//new List<Lambda<C, M>>();
            //_lambda.Add(new Lambda<C, M>(tau: tau, atI: metadata));
        }

        internal B(char tau, UInt32 metadata, B nextBlock) // check whether it is faster with ref or without ref for nextBlock. 
        {
            if (tau == 'R') omega = 1;//nextBlock.omega + 1;
            //else omega = nextBlock.omega;




            ////_lambda = new Lambda[nextBlock._lambda.Length + 1];
            ////_lambda[0] = new Lambda(tau, metadata);
            ////for (int i = 1; i <= nextBlock._lambda.Length; i++)
                ////_lambda[i] = new Lambda('M', nextBlock._lambda[i - 1].atI);
            /*
                _lambda = new List<Lambda<C, M>>();
            _lambda.Add(new Lambda<C, M>(tau: tau, atI: metadata));
            foreach (var item in nextBlock.lambda)
                if (item.tau != 'L')
                    _lambda.Add(new Lambda<C, M>(tau: 'M', atI: item.atI));*/
        }

        ////internal B(int omega, Lambda[] lambda)
        internal B(int omega)
        {
            this.omega = omega;
            ////this._lambda = lambda;
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
        internal int omega { private set; get; }

        /// <summary>
        /// Represents the interval intersecting with 
        /// the c of corresponding block.
        /// </summary>
        ////private Lambda[] _lambda { set; get; }

        // think better how I can make this line, because it is necessary. 
        //internal ReadOnlyCollection<Lambda<M>> lambda { get { return _lambda.AsReadOnly(); } }
        // temporary solution:
        ////internal Lambda[] lambda { get { return _lambda; } }



        internal B Update(int Omega)
        {
            B newBlock = new B();

            /// update it with new Omega.
            newBlock.omega = Omega;

            /// deep copy the current one.
            /**///newBlock._lambda = new List<Lambda<C, M>>(this._lambda);

            return newBlock;
        }

        internal B Update(char tau, UInt32 metadata)
        {
            B newBlock = new B();

            /// deep copy the current one.
            /**///newBlock._lambda = new List<Lambda<C, M>>(this._lambda);

            /// update it with new Lambda.
            /**///newBlock._lambda.Add(new Lambda<C, M>(tau: tau, atI: metadata));

            return newBlock;
        }

        internal B Update(int Omega, char tau, UInt32 metadata)
        {
            B newBlock = new B();

            /// update it with new Omega.
            newBlock.omega = Omega;

            /// deep copy the current one.
            /**///newBlock._lambda = new List<Lambda<C, M>>(this._lambda);

            /// update it with new Lambda.
            /**///newBlock._lambda.Add(new Lambda<C, M>(tau: tau, atI: metadata));

            return newBlock;
        }

        internal B Update(Dictionary<uint, Lambda> lambdas)
        {
            B newBlock = new B();
            newBlock.omega = this.omega;
            /**///newBlock._lambda = new List<Lambda<C, M>>();
            /**///foreach (var l in lambdas)
                /**///newBlock._lambda.Add(new Lambda<C, M>(tau: l.Value.tau, atI: l.Value.atI));

            return newBlock;
        }
    }
}
