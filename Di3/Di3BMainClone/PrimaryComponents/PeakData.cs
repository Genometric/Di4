using System;
using ProtoBuf;
using Interfaces;

namespace Di3BMain
{
    /// <summary>
    /// Representing ChIP-seq Peak Metadata.
    /// </summary>
    [ProtoContract]
    public class PeakData : IExtMetaData<int>
    {
        /// <summary>
        /// Sets and gets peak name.
        /// </summary>
        [ProtoMember(1)]
        public string name { set; get; }

        /// <summary>
        /// Gets the left-end of the interval.
        /// </summary>
        [ProtoMember(2)]
        public int left { set; get; }

        /// <summary>
        /// Gets the right-end of the interval.
        /// </summary>
        [ProtoMember(3)]
        public int right { set; get; }

        /// <summary>
        /// Sets and gets peak value.
        /// </summary>
        [ProtoMember(4)]
        public double value { set; get; }

        /// <summary>
        /// Gets hash key generated using
        /// One-at-a-Time method based on 
        /// Dr. Dobb's left method.
        /// </summary>
        [ProtoMember(5)]
        public UInt32 hashKey { set; get; }
    }
}
