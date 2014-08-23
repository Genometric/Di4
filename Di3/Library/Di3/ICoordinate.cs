using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI3
{
    public interface ICoordinate<C>
    {
        /// <summary>
        /// Compares this object with that both of type C and returns:
        /// <para>G : if this is greater than that.</para>
        /// <para>E : if two objects have equal values.</para>
        /// <para>L : if this is less than that.</para>
        /// </summary>
        /// <param name="that">The object this is compared to.</param>
        /// <returns></returns>
        char CompareTo(C that);


        /// <summary>
        /// Denotes the default value for the coordinate, 
        /// used when coordinate is not provided. 
        /// </summary>
        C defaultValue { set; get; }
    }
}
