using System;
using IGenomics;

namespace Di3BCLI
{
    /// <summary>
    /// Representing ChIP-seq Peak Metadata.
    /// </summary>
    public class PeakData : IChIPSeqPeak, IFormattable
    {
        /// <summary>
        /// Sets and gets peak name.
        /// </summary>
        public string name { set; get; }

        /// <summary>
        /// Gets the left-end of the interval.
        /// </summary>
        //public int left { set; get; }

        /// <summary>
        /// Gets the right-end of the interval.
        /// </summary>
        //public int right { set; get; }

        /// <summary>
        /// Sets and gets peak value.
        /// </summary>
        public double value { set; get; }

        /// <summary>
        /// Gets hash key generated using
        /// One-at-a-Time method based on 
        /// Dr. Dobb's left method.
        /// </summary>
        public UInt32 hashKey { set; get; }

        public string ToString(string separator = "\t")
        {
            return
                //left.ToString() + separator +
                //right.ToString() + separator +
                name + separator +
                value.ToString();
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "null";
        }
    }
}
