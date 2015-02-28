using System;
using System.Collections.ObjectModel;

namespace Polimi.DEIB.VahidJalili.DI3
{
    /// <summary>
    /// A Bookmark representing relative information of intervals
    /// intersecting with a particular e on domain.</summary>
    public class B
    {
        internal B(IntersectionCondition condition, UInt32 atI)
        {
            mu = 0;
            omega = 0;
            switch(condition)
            {
                case IntersectionCondition.LeftEnd:
                    _lambda = new Lambda[] { new Lambda(phi: true, atI: atI) };
                    break;

                case IntersectionCondition.Middle:
                    mu = 1;
                    break;

                case IntersectionCondition.RightEnd:
                    omega = 1;
                    _lambda = new Lambda[] { new Lambda(phi: false, atI: atI) };
                    break;
            }
        }
        internal B(int mu, UInt16 omega, ReadOnlyCollection<Lambda> lambda)
        {
            this.mu = mu;
            this.omega = omega;
            _lambda = new Lambda[lambda.Count];
            lambda.CopyTo(_lambda, 0);
        }
        internal B(IntersectionCondition condition, UInt32 atI, B nextBookmark)
        {
            mu = nextBookmark.mu;
            omega = 0;
            switch(condition)
            {
                case IntersectionCondition.LeftEnd:                    
                    _lambda = new Lambda[] { new Lambda(phi: true, atI: atI) };
                    break;

                // I don't think this condition would be possibly seen ever :)
                case IntersectionCondition.Middle:
                    mu++;
                    break;

                case IntersectionCondition.RightEnd:
                    omega++;
                    _lambda = new Lambda[] { new Lambda(phi: false, atI: atI) };
                    break;
            }
            
            foreach (var item in nextBookmark.lambda)
                if (item.phi == false)
                    mu++;
        }
        private B(int mu, UInt16 omega, Lambda[] lambda, bool phi, UInt32 atI)
        {
            this.omega = omega;
            _lambda = new Lambda[lambda.Length + 1];
            Array.Copy(lambda, _lambda, lambda.Length);
            _lambda[lambda.Length] = new Lambda(phi: phi, atI: atI);
        }
        private B(int mu, UInt16 omega, Lambda[] lambda, UInt32 atI, IntersectionCondition condition)
        {
            this.mu = mu;
            this.omega = omega;
            switch (condition)
            {
                case IntersectionCondition.LeftEnd:
                    _lambda = new Lambda[lambda.Length + 1];
                    Array.Copy(lambda, _lambda, lambda.Length);
                    _lambda[lambda.Length] = new Lambda(phi: true, atI: atI);
                    break;

                case IntersectionCondition.Middle:
                    this.mu++;
                    _lambda = new Lambda[lambda.Length];
                    Array.Copy(lambda, _lambda, lambda.Length);
                    break;

                case IntersectionCondition.RightEnd:
                    this.omega++;
                    _lambda = new Lambda[lambda.Length + 1];
                    Array.Copy(lambda, _lambda, lambda.Length);
                    _lambda[lambda.Length] = new Lambda(phi: false, atI: atI);
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
        internal UInt16 omega { private set; get; }

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



        internal B Update(int mu, UInt16 omega, bool phi, UInt32 atI)
        {
            return new B(mu: mu, omega: omega, lambda: _lambda, phi: phi, atI: atI);
        }

        internal B Update(UInt32 atI, IntersectionCondition condition)
        {
            return new B(mu: this.mu, omega: this.omega, lambda: _lambda, atI: atI, condition: condition);
        }

        internal B Update(ref int mu, ref UInt16 omega, ReadOnlyCollection<Lambda> lambda)
        {
            return new B(mu: mu, omega: omega, lambda: lambda);
        }
    }
}
