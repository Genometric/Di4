using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3Bioinformatics
{
    public class FunctionOutput<O>
    {
        public Dictionary<string, Chromosome> chrs { set; get; }

        public FunctionOutput(int chrCount)
        {
            chrs = new Dictionary<string, Chromosome>();
        }

        public class Chromosome
        {
            public List<O> outputPositiveStrand { set; get; }
            public List<O> outputNegativeStrand { set; get; }
            public List<O> outputUnstranded { set; get; }
        }

        public void addChromosome(string chr)
        {
            chrs.Add(chr, new Chromosome());
        }
    }
}
