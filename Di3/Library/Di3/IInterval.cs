using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3
{
    public interface IInterval<C>
    {
        /// <summary>
        /// Gets the left-end of the interval.
        /// </summary>
        C left { set; get; }

        /// <summary>
        /// Gets the right-end of the interval.
        /// </summary>
        C right { set; get; }
    }
}
