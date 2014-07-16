using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="C">Represents the coordinate/domain
    /// type (e.g,. int, double, Time.</typeparam>
    /// <typeparam name="I">Represents interval type.
    /// <para>For intervals of possibly different types,
    /// it is recommended to define this generic type
    /// parameter in terms of Lowest Common Denominator.
    /// </para></typeparam>
    internal sealed class Di3<C, I> : B<C, I> where I : IInterval<C>
    {
        internal List<B<C, I>> Bi { private set; get; }

        internal Di3()
        {
            Bi = new List<B<C, I>>();
        }

        public void AddInterval(I interval)
        {

        }
    }
}
