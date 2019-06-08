using System;
using System.Collections.ObjectModel;

namespace Genometric.Di4.Inc
{
    /// <summary>
    /// A Bookmark representing relative information of intervals
    /// intersecting with a particular e on domain.</summary>
    public class B
    {
        /// <summary>
        /// A Bookmark representing relative information of intervals
        /// intersecting with a particular e on domain.</summary>
        internal B() // this initializer may be redundant, however, it might have performance impact.
        {
            mu = 0;
            omega = 0;
            _lambda = new Lambda[0];
        }
        internal B(Phi phi, uint atI, uint collectionID)
        {
            mu = 0;
            omega = 0;
            switch(phi)
            {
                case Phi.LeftEnd:
                    _lambda = new Lambda[] { new Lambda(phi: Phi.LeftEnd, atI: atI, collectionID: collectionID) };
                    break;

                case Phi.Middle:
                    mu = 1;
                    break;

                case Phi.RightEnd:
                    omega = 1;
                    _lambda = new Lambda[] { new Lambda(phi: Phi.RightEnd, atI: atI, collectionID: collectionID) };
                    break;
            }
        }
        internal B(int mu, ushort omega, ReadOnlyCollection<Lambda> lambda)
        {
            this.mu = mu;
            this.omega = omega;
            _lambda = new Lambda[lambda.Count];
            lambda.CopyTo(_lambda, 0);
        }
        internal B(Phi phi, uint atI, uint collectionID, B nextBookmark)
        {
            mu = nextBookmark.mu;
            omega = 0;
            switch(phi)
            {
                case Phi.LeftEnd:                    
                    _lambda = new Lambda[] { new Lambda(phi: Phi.LeftEnd, atI: atI, collectionID: collectionID) };
                    break;

                // I don't think this condition would be possibly ever met :)
                case Phi.Middle:
                    mu++;
                    break;

                case Phi.RightEnd:
                    omega++;
                    _lambda = new Lambda[] { new Lambda(phi: Phi.RightEnd, atI: atI, collectionID: collectionID) };
                    break;
            }
            
            /*foreach (var item in nextBookmark.lambda)
                if (item.phi == false)
                    mu++;*/
            mu += nextBookmark.omega;
        }
        private B(int mu, ushort omega, Lambda[] lambda, bool phi, uint atI)
        {
            // TODO: 
            /// Do I have to check if I need to update omega as omega++ based on phi ?
            this.omega = omega;
            _lambda = new Lambda[lambda.Length + 1];
            Array.Copy(lambda, _lambda, lambda.Length);

            // The following line should not be commented-out
            //_lambda[lambda.Length] = new Lambda(phi: phi, atI: atI);
        }
        private B(int mu, ushort omega, Lambda[] lambda, uint atI, uint collectionID, Phi phi)
        {
            this.mu = mu;
            this.omega = omega;
            switch (phi)
            {
                case Phi.LeftEnd:
                    _lambda = new Lambda[lambda.Length + 1];
                    Array.Copy(lambda, _lambda, lambda.Length);
                    _lambda[lambda.Length] = new Lambda(phi: Phi.LeftEnd, atI: atI, collectionID: collectionID);
                    break;

                case Phi.Middle:
                    this.mu++;
                    _lambda = new Lambda[lambda.Length];
                    Array.Copy(lambda, _lambda, lambda.Length);
                    break;

                case Phi.RightEnd:
                    this.omega++;
                    _lambda = new Lambda[lambda.Length + 1];
                    Array.Copy(lambda, _lambda, lambda.Length);
                    _lambda[lambda.Length] = new Lambda(phi: Phi.RightEnd, atI: atI, collectionID: collectionID);
                    break;
            }
        }

        /// <summary>
        /// Denotes the number of intervals whose
        /// middle intersects with e.
        /// </summary>
        internal int mu { private set; get; }

        /// <summary>
        /// Denotes the number of intervals whose
        /// right-end intersects with e. 
        /// </summary>
        internal ushort omega { private set; get; }

        /// <summary>
        /// Represents the intervals intersecting with
        /// the e of corresponding keyBookmark.
        /// </summary>
        private Lambda[] _lambda { set; get; }

        /// <summary>
        /// Represents the intervals intersecting with
        /// the e of corresponding keyBookmark.
        /// </summary>
        internal ReadOnlyCollection<Lambda> lambda { get { return Array.AsReadOnly(_lambda); } }

        internal int accumulation { get { return lambda.Count - omega + mu; } }



        internal B Update(int mu, ushort omega, bool phi, uint atI) // keep the bool for phi cos until make sure who is calling/using it !!
        {
            return new B(mu: mu, omega: omega, lambda: _lambda, phi: phi, atI: atI);
        }

        internal B Update(uint atI, uint collectionID, Phi condition)
        {
            return new B(mu: mu, omega: omega, lambda: _lambda, atI: atI, collectionID: collectionID, phi: condition);
        }

        internal B Update(ref int mu, ref ushort omega, ReadOnlyCollection<Lambda> lambda)
        {
            return new B(mu: mu, omega: omega, lambda: lambda);
        }
    }
}
