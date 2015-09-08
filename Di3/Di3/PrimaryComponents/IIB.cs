using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Polimi.DEIB.VahidJalili.DI3
{
    /// <summary>
    /// A Bookmark representing relative information of intervals
    /// intersecting with a particular e on domain.</summary>
    public class IB
    {
        /// <summary>
        /// A Bookmark representing relative information of intervals
        /// intersecting with a particular e on domain.</summary>
        internal IB()
        {
            omega = 0;
            _lambda = new Lambda[0];
        }

        internal IB(ushort omega)
        {
            this.omega = omega;
            _lambda = new Lambda[0];
        }

        private IB(ushort omega, Lambda[] lambda, Phi phi, uint hashKey)
        {
            this.omega = phi == Phi.RightEnd ? (ushort)(omega + 1) : omega;

            _lambda = new Lambda[lambda.Length + 1];
            Array.Copy(lambda, _lambda, lambda.Length);
            _lambda[lambda.Length] = new Lambda(phi: phi, atI: hashKey);
        }
        private IB(ushort omega, Dictionary<uint, Lambda> lambdas)
        {
            this.omega = omega;
            _lambda = new Lambda[lambdas.Count];
            lambdas.Values.CopyTo(_lambda, 0);
        }

        internal IB(Phi phi, uint hashKey)
        {
            omega = phi == Phi.RightEnd ? (ushort)1 : (ushort)0;
            _lambda = new Lambda[] { new Lambda(phi: phi, atI: hashKey) };
        }

        internal IB(ushort omega, ReadOnlyCollection<Lambda> lambda)
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
                if (lambda[i].phi == 'R')
                    omega++;
            }*/
        }

        internal IB(Phi phi, uint metadata, IB nextBlock) // check whether it is faster with ref or without ref for nextBlock. 
        {
            if (phi == Phi.RightEnd) omega = 1;

            /// Following method has 2 ints and one extra read on 
            /// nextBlock.lambda. I prefered this method over `copying
            /// all items satisfying the condition to a list and 
            /// converting a list to array` because this method has
            /// less footprints and copies lambda only once. 
            int i = 1;
            foreach (var item in nextBlock.lambda)
                if (item.phi !=  Phi.LeftEnd)
                    i++;
            _lambda = new Lambda[i];
            _lambda[0] = new Lambda(phi: phi, atI: metadata);
            i = 1;
            foreach (var item in nextBlock.lambda)
                if (item.phi != Phi.LeftEnd)
                    _lambda[i++] = new Lambda(phi: Phi.Middle, atI: item.atI);
        }

        private IB(ushort omega, Lambda[] lambda, uint atI, Phi phi)
        {
            this.omega = omega;
            _lambda = new Lambda[lambda.Length + 1];
            Array.Copy(lambda, _lambda, lambda.Length);
            _lambda[lambda.Length] = new Lambda(phi: phi, atI: atI);

            if (phi == Phi.RightEnd)
                this.omega++;
        }


        /// <summary>
        /// Denotes the number of intervals whose
        /// right-end intersects with e. 
        /// </summary>
        internal ushort omega { private set; get; }

        /// <summary>
        /// Represents the _interval intersecting with 
        /// the c of corresponding bookmark.
        /// </summary>
        private Lambda[] _lambda { set; get; }

        internal ReadOnlyCollection<Lambda> lambda { get { return Array.AsReadOnly(_lambda); } }



        internal IB Update(ushort omega, Phi phi, uint hashKey)
        {
            return new IB(omega: omega, lambda: _lambda, phi: phi, hashKey: hashKey);
        }

        internal IB Update(uint atI, Phi condition)
        {
            return new IB(omega: omega, lambda: _lambda, atI: atI, phi: condition);
        }

        internal IB Update(Dictionary<uint, Lambda> lambdas)
        {
            return new IB(omega: this.omega, lambdas: lambdas);
        }
    }
}
