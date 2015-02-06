using IGenomics;
using System;

namespace Di3BCLI
{
    public class Peak : IInterval<int, PeakData>, IFormattable
    {
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
        /// of the interval. It could be a refChr
        /// to a _memory object, or a pointer, or 
        /// an entry ID on database, or etc. 
        /// </summary>
        public PeakData metadata { set; get; }

        public uint hashKey { set; get; }

        public string ToString(string separator = "\t")
        {
            if (metadata == null)
                return
                    left.ToString() + separator +
                    right.ToString();
            return
                left.ToString() + separator +
                right.ToString() + separator +
                metadata.ToString();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "null";
        }
    }
}
