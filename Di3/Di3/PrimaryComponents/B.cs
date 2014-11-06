using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DI3
{
    /// <summary>
    /// A Block representing relative information of _intervals
    /// intersecting with a particular c (i.e., e) on 
    /// domain.</summary>
    /// <typeparam name="C">Represents the c/domain
    /// type (e.g,. int, double, Time).</typeparam>
    /// <typeparam name="M">Represents generic
    /// type of pointer to descriptive hashKey cooresponding
    /// to the _interval.</typeparam>
    //[ProtoContract]
    public class B
    {
        /// <summary>
        /// A Block representing relative information of _intervals
        /// intersecting with a particular c (e) on domain.</summary>
        internal B()
        {// it seems that the protobuf net needs this constructor. 
            omega = 0;
            _lambda = new Lambda[0];
        }

        internal B(int omega)
        {
            this.omega = omega;
            _lambda = new Lambda[0];
        }

        private B(int omega, Lambda[] lambda, char tau, UInt32 hashKey)
        {
            this.omega = omega;
            _lambda = new Lambda[lambda.Length + 1];
            Array.Copy(lambda, _lambda, lambda.Length);
            _lambda[lambda.Length] = new Lambda(tau: tau, atI: hashKey);
        }
        private B(int omega, Dictionary<uint, Lambda> lambdas)
        {
            this.omega = omega;
            _lambda = new Lambda[lambdas.Count]; 
            lambdas.Values.CopyTo(_lambda, 0);
        }

        internal B(char tau, UInt32 hashKey)
        { // initializes a block and adds one Lambda to lambda according to tau and hashKey.
            if (tau == 'R') omega = 1;
            else omega = 0;
            _lambda = new Lambda[] { new Lambda(tau: tau, atI: hashKey) };
        }

        internal B(int omega, ReadOnlyCollection<Lambda> lambda)
        {
            this.omega = omega;
            _lambda = new Lambda[lambda.Count];
            lambda.CopyTo(_lambda, 0);
            /*
            omega = 0;
            _lambda = new Lambda[lambda.Count];

            for (int i = 0; i < lambda.Count; i++)
            {
                _lambda[i] = lambda[i];
                if (lambda[i].tau == 'R')
                    omega++;
            }*/
        }

        internal B(char tau, UInt32 metadata, B nextBlock) // check whether it is faster with ref or without ref for nextBlock. 
        {
            if (tau == 'R') omega = 1;

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
        }


        /// <summary>
        /// Denotes the number of _intervals whose
        /// right-end intersects with e. 
        /// </summary>
        internal int omega { private set; get; }

        /// <summary>
        /// Represents the _interval intersecting with 
        /// the c of corresponding block.
        /// </summary>
        private Lambda[] _lambda { set; get; }

        internal ReadOnlyCollection<Lambda> lambda { get { return Array.AsReadOnly(_lambda); } }



        internal B Update(int omega, char tau, UInt32 hashKey)
        {
            return new B(omega: omega, lambda: _lambda, tau: tau, hashKey: hashKey);
        }

        internal B Update(Dictionary<uint, Lambda> lambdas)
        {
            return new B(omega: this.omega, lambdas: lambdas);
        }
    }
}
