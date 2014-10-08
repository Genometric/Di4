using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3B
{
    public class FunctionOutput<O>
    {
        public Dictionary<string, Chromosome> Chrs { set; get; }

        public FunctionOutput(int chrCount)
        {
            Chrs = new Dictionary<string, Chromosome>();
        }

        public class Chromosome
        {
            public Chromosome()
            {
                //outputPS = new List<O>();
                //outputNS = new List<O>();
                outputUS = new List<O>();
            }
            public List<O> outputPS { set; get; }
            public List<O> outputNS { set; get; }
            public List<O> outputUS { set; get; }
        }

        public void addChromosome(string chr)
        {
            Chrs.Add(chr, new Chromosome());
        }
    }
}
