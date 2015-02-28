using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI3.DI3B
{
    public class FunctionOutput<O>
    {
        public ConcurrentDictionary<string, ConcurrentDictionary<char, List<O>>> Chrs { set; get; }

        public FunctionOutput()
        {
            Chrs = new ConcurrentDictionary<string, ConcurrentDictionary<char, List<O>>>();
        }

        public void addChromosome(string chr)
        {
            Chrs.TryAdd(chr, new ConcurrentDictionary<char, List<O>>());
        }
    }
}
