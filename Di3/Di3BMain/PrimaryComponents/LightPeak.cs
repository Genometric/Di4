//using ProtoBuf;
using Interfaces;

namespace Di3BMain
{
    //[ProtoContract]
    public class LightPeak : IInterval<int, LightPeakData>
    {
        /// <summary>
        /// Sets and gets the left-end of the interval.
        /// </summary>
        //[ProtoMember(1)]
        public int left { set; get; }


        /// <summary>
        /// Sets and gets the right-end of the interval.
        /// </summary>
        //[ProtoMember(2)]
        public int right { set; get; }


        /// <summary>
        /// Sets and gets the descriptive metadata
        /// of the interval. It could be a reference
        /// to a memory object, or a pointer, or 
        /// an entry ID on database, or etc. 
        /// </summary>
        //[ProtoMember(3)]
        public LightPeakData metadata { set; get; }
    }
}
