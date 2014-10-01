using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IInterval;
using ICPMD;
using Di3Interfaces;

namespace BEDParser
{
    public class ParsedBED<C, I, M>
        //where C : ICoordinate
        where I : IInterval<C, M>, new()
        where M : ICPMetadata<C>
    {
        public string fileName { set; get; }
        public string filePath { set; get; }
        public UInt32 fileHashKey { set; get; }
        public byte chrCount { set; get; }
        public int peaksCount { set; get; }
        public Dictionary<string, ChrStatistics> chrStatistics { set; get; }
        public I pValueMax { set; get; }
        public I pValueMin { set; get; }
        public double pValueMean { set; get; }
        public List<string> messages { set; get; }
        public Dictionary<string, List<I>> peaks { set; get; }
        public string species { set; get; }


        public ParsedBED()
        {
            chrStatistics = new Dictionary<string, ChrStatistics>();

            messages = new List<string>();

            peaks = new Dictionary<string, List<I>>();

            pValueMax = new I() { left = default(C), right = default(C), metadata = { chrNo = 0, name = null, value = 0, strand = '*' } };

            pValueMin = new I() { left = default(C), right = default(C), metadata = { chrNo = 0, name = null, value = 1, strand = '*' } };
        }
    }
}
