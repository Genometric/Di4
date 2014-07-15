using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3
{
    static internal class Modifiers<C, I>
    {
        public static void Insert(List<B<C,I>>Bi, I interval)
        {

        }

        /// <summary>
        /// Represents the coordinate (right or left-end) of interval being
        /// inserted to Di3.
        /// </summary>
        private static C e { set; get; }

        /// <summary>
        /// Represents the index of Bi
        /// </summary>
        private static int b { set; get; }
    }
}
