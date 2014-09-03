using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IInterval;
using ICPMD;

namespace BEDParser
{
    public class ParsedBED<I, M>
        where I : IInterval<int, M>, new()
        where M : ICPMetadata<int>
    {
        public string fileName { set; get; }
        public string filePath { set; get; }
        public byte chrCount { set; get; }
        public int peaksCount { set; get; }
        public List<ChrStatistics> chrStatistics { set; get; }
        public I pValueMax { set; get; }
        public I pValueMin { set; get; }
        public double pValueMean { set; get; }
        public List<string> messages { set; get; }
        public List<List<I>> peaks { set; get; }
        public string species { set; get; }


        public ParsedBED()
        {
            chrStatistics = new List<ChrStatistics>();

            messages = new List<string>();

            peaks = new List<List<I>>();

            pValueMax = new I() { left = 0, right = 0, metadata = { chrNo = 0, name = null, value = 0, strand = '*' } };

            pValueMin = new I() { left = 0, right = 0, metadata = { chrNo = 0, name = null, value = 1, strand = '*' } };
        }
    }
}
