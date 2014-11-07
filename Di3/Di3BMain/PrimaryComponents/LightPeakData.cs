using System;
using Interfaces;

namespace Di3BMain
{
    public class LightPeakData : IMetaData
    {
        public LightPeakData()
        { }
        public UInt32 hashKey { set; get; }
    }
}
