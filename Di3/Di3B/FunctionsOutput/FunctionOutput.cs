using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Di3B
{
    public class FunctionOutput<O>
    {
        //public Dictionary<string, Chromosome> chrs { set; get; }
        public Dictionary<string, Dictionary<char, ConcurrentBag<O>>> Chrs { set; get; }

        public FunctionOutput()
        {
            //chrs = new Dictionary<string, Chromosome>();
            Chrs = new Dictionary<string, Dictionary<char, ConcurrentBag<O>>>();
        }
        /*
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
        }*/

        public void addChromosome(string chr)
        {
            //chrs.Add(chr, new Chromosome());
            Chrs.Add(chr, new Dictionary<char, ConcurrentBag<O>>());
        }
    }
}
