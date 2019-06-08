using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Genometric.Di4.Inv
{
    /// <summary>
    /// A Bookmark representing relative information of intervals
    /// intersecting with a particular e on domain.</summary>
    public class B
    {
        /// <summary>
        /// A Bookmark representing relative information of intervals
        /// intersecting with a particular e on domain.</summary>
        internal B()
        {
            omega = 0;
            _lambda = new Lambda[0];
        }

        internal B(ushort omega)
        {
            this.omega = omega;
            _lambda = new Lambda[0];
        }

        private B(ushort omega, Lambda[] lambda, Phi phi, uint hashKey, uint collectionID)
        {
            this.omega = phi == Phi.RightEnd ? (ushort)(omega + 1) : omega;

            _lambda = new Lambda[lambda.Length + 1];
            Array.Copy(lambda, _lambda, lambda.Length);
            _lambda[lambda.Length] = new Lambda(phi: phi, atI: hashKey, collectionID: collectionID);
        }
        private B(ushort omega, ICollection<Lambda> lambdas)
        {
            this.omega = omega;
            _lambda = new Lambda[lambdas.Count];
            lambdas.CopyTo(_lambda, 0);
        }
        

        internal B(Phi phi, uint hashKey, uint collectionID)
        {
            omega = phi == Phi.RightEnd ? (ushort)1 : (ushort)0;
            _lambda = new Lambda[] { new Lambda(phi: phi, atI: hashKey, collectionID: collectionID) };
        }

        internal B(ushort omega, ReadOnlyCollection<Lambda> lambda)
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

        internal B(Phi phi, uint metadata, uint collectionID, B nextBlock) // check whether it is faster with ref or without ref for nextBlock. 
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
            _lambda[0] = new Lambda(phi: phi, atI: metadata, collectionID: collectionID);
            i = 1;
            foreach (var item in nextBlock.lambda)
                if (item.phi != Phi.LeftEnd)
                    _lambda[i++] = new Lambda(phi: Phi.Middle, atI: item.atI, collectionID: collectionID);
        }

        private B(ushort omega, Lambda[] lambda, uint atI, Phi phi, uint collectionID)
        {
            this.omega = omega;
            _lambda = new Lambda[lambda.Length + 1];
            Array.Copy(lambda, _lambda, lambda.Length);
            _lambda[lambda.Length] = new Lambda(phi: phi, atI: atI, collectionID: collectionID);

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



        internal B Update(ushort omega, Phi phi, uint hashKey, uint collectionID)
        {
            return new B(omega: omega, lambda: _lambda, phi: phi, hashKey: hashKey, collectionID: collectionID);
        }

        internal B Update(uint atI, uint collectionID, Phi condition)
        {
            return new B(omega: omega, lambda: _lambda, atI: atI, phi: condition, collectionID: collectionID);
        }

        internal B Update(ICollection<Lambda> lambdas)
        {
            return new B(omega: omega, lambdas: lambdas);
        }
    }
}
