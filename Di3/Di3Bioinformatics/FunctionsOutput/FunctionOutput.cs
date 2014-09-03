using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3Bioinformatics
{
    public class FunctionOutput<O>
    {
        public FunctionOutput(int chrCount)
        {
            chrs = new List<chr>();
            for (int chr = 0; chr < chrCount; chr++)
                chrs.Add(new chr());
        }

        public List<chr> chrs { set; get; }

        public class chr
        {
            public List<O> outputPositiveStrand { set; get; }
            public List<O> outputNegativeStrand { set; get; }
            public List<O> outputUnstranded { set; get; }

        }
    }
}
