using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI3
{
    /// <summary>
    /// Represents main di3 data structure. 
    /// </summary>
    /// <typeparam name="C">Represents the c/domain
    /// type (e.g,. int, double, Time.</typeparam>
    /// <typeparam name="I">Represents generic type of the interval.
    /// (e.g., time span, interval on natural numbers)
    /// <para>For intervals of possibly different types,
    /// it is recommended to define this generic type
    /// parameter in terms of Lowest Common Denominator.
    /// </para></typeparam>
    /// <typeparam name="M">Represents generic
    /// type of pointer to descriptive metadata cooresponding
    /// to the interval.</typeparam>
    internal class Di3DataStructure<C, I, M> : B<C, I, M>
        where C : ICoordinate<C>
        where I : IInterval<C, M>
    {
        /// <summary>
        /// Di3 primary data structure; a list of Di3 blocks.
        /// </summary>
        static internal List<B<C, I, M>> di3 { set; get; }
    }
}
