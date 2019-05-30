using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polimi.DEIB.VahidJalili.IGenomics;

namespace Polimi.DEIB.VahidJalili.DI4.CLI
{
    public class VariantData : IVCF, IFormattable
    {
        public BasePair[] altBase { set; get; }

        public string filter { set; get; }

        public uint hashKey { set; get; }

        public string ID { set; get; }

        public string info { set; get; }

        public double quality { set; get; }

        public BasePair[] refBase { set; get; }

        public double value { set; get; }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "null";
        }
    }
}
