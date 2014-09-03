using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEDParser
{
    public sealed class ChrStatistics
    {
        public string chrTitle { set; get; }
        public int count { set; get; }
        public string percentage { set; get; }

        public uint peakWidthMax { set; get; }
        public uint peakWidthMin { set; get; }
        public double peakWidthMean { set; get; }
        public double peakWidth_STDV { set; get; }


        public double pValueMax { set; get; }
        public double pValueMin { set; get; }
        public double pValueMean { set; get; }
        public double pValueSTDV { set; get; }

        public float coverage { set; get; }
    }
}
