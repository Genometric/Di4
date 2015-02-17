using System;
using Polimi.DEIB.VahidJalili.IGenomics;

namespace Polimi.DEIB.VahidJalili.DI3.CLI
{
    public class LightPeakData : IMetaData
    {
        public LightPeakData()
        { }
        public UInt32 hashKey { set; get; }
    }
}
