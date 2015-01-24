using System;
using Interfaces;

namespace Di3BCLI
{
    /// <summary>
    /// Representing ChIP-seq Peak Metadata.
    /// </summary>
    public class PeakData : IExtMetaData<int>
    {
        /// <summary>
        /// Sets and gets peak name.
        /// </summary>
        public string name { set; get; }

        /// <summary>
        /// Gets the left-end of the interval.
        /// </summary>
        public int left { set; get; }

        /// <summary>
        /// Gets the right-end of the interval.
        /// </summary>
        public int right { set; get; }

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
    }
}
