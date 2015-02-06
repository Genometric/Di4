using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Di3B
{
    public class FunctionOutput<O>
    {
        public Dictionary<string, Dictionary<char, ConcurrentBag<O>>> Chrs { set; get; }

        public FunctionOutput()
        {
            Chrs = new Dictionary<string, Dictionary<char, ConcurrentBag<O>>>();
        }

        public void addChromosome(string chr)
        {
            Chrs.Add(chr, new Dictionary<char, ConcurrentBag<O>>());
        }
    }
}
