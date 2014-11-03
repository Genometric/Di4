using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3WithCustomBlockSerializer
{
    public class Lambda//<M>
        //where C : IComparable<C>
        //where M : IMetaData/*<C>*/
    {
        /// <summary>
        /// Represents the interval intersecting with 
        /// the c of corresponding block. 
        /// <para>For intervals of possibly different types,
        /// it is recommended to define this generic type
        /// parameter in terms of Lowest Common Denominator.
        /// </para>
        /// </summary>
        internal Lambda()
        {// it seems that the protobuf-net needs this constructor.
        }

        /// <summary>
        /// Represents the interval intersecting with 
        /// the c of corresponding block. 
        /// <para>For intervals of possibly different types,
        /// it is recommended to define this generic type
        /// parameter in terms of Lowest Common Denominator.
        /// </para>
        /// </summary>
        /// <param name="tau">The intersection type of interval
        /// wtih c of corresponding block.</param>
        /// <param name="atI">Descriptive metadata of the intereval.</param>
        internal Lambda(char tau, UInt32 atI)//M atI)
        {
            this.tau = tau;
            this.atI = atI;
        }



        /// <summary>
        /// Gets the intersection type of interval
        /// wtih c of corresponding block.
        /// <para>[currentValue] = L  ::>  Left-end  intersecting the coordiante.</para>
        /// <para>[currentValue] = M  ::>  Middle    intersecting the coordiante.</para>
        /// <para>[currentValue] = R  ::>  Right-end intersecting the coordiante.</para>
        /// </summary>
        internal char tau { private set; get; }

        /// <summary>
        /// Gets descriptive metadata of the intereval
        /// represented by generic type M.
        /// </summary>
        //internal M atI { private set; get; }
        internal UInt32 atI { private set; get; }
    }
}
