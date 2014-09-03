using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEDParser;
using IInterval;
using ICPMD;

namespace Di3BMain
{
    internal class Peak : IInterval<int, PeakData>
    {
        public Peak()
        {
            metadata = new PeakData();
        }

        /// <summary>
        /// Sets and gets the left-end of the interval.
        /// </summary>
        public int left { set; get; }

        /// <summary>
        /// Sets and gets the right-end of the interval.
        /// </summary>
        public int right { set; get; }

        /// <summary>
        /// Sets and gets the descriptive metadata
        /// of the interval. It could be a reference
        /// to a memory object, or a pointer, or 
        /// an entry ID on database, or etc. 
        /// </summary>
        public PeakData metadata { set; get; }
    }
}
