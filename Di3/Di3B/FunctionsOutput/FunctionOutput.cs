using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI3.DI3B
{
    public class FunctionOutput<O>
    {
        public Dictionary<string, Dictionary<char, List<O>>> Chrs { set; get; }

        public FunctionOutput()
        {
            Chrs = new Dictionary<string, Dictionary<char, List<O>>>();
        }

        public void addChromosome(string chr)
        {
            Chrs.Add(chr, new Dictionary<char, List<O>>());
        }
    }
}
