using Polimi.DEIB.VahidJalili.IGenomics;
using System;

namespace Polimi.DEIB.VahidJalili.DI3.CLI
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
        /// Sets and gets peak value.
        /// </summary>
        public double value { set; get; }

        /// <summary>
        /// Gets hash key generated using
        /// One-at-a-Time method based on 
        /// Dr. Dobb's left method.
        /// </summary>
        public UInt32 hashKey { set; get; }

        /// <summary>
        /// Sets and gets the summit of the interval.
        /// </summary>
        public int summit { set; get; }

        public string ToString(string separator = "\t")
        {
            return
                name + separator +
                value.ToString();
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "null";
        }
    }
}
