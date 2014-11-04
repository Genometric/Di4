using System;
//using ProtoBuf;
using Interfaces;

namespace Di3BMain
{
    //[ProtoContract]
    public class LightPeakData : IMetaData
    {
        public LightPeakData()
        { }
        //[ProtoMember(1)]
        public UInt32 hashKey { set; get; }
    }
}
