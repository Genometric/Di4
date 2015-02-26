using CSharpTest.Net.Serialization;
using Polimi.DEIB.VahidJalili.DI3.DI3B.Logging;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI3.DI3B
{
    public class Di3B<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>, IFormattable, new()
        where M : IMetaData, IFormattable, new()
    {
        public Di3B(string workingDirectory, string sectionTitle, Memory Memory, HDDPerformance hddPerformance, ISerializer<C> CSerializer, IComparer<C> CComparer)
        {
            this.CSerializer = CSerializer;
            this.CComparer = CComparer;

            genome = new Genome<C, I, M>(workingDirectory, sectionTitle, Memory, hddPerformance, CSerializer, CComparer);
        }

        private ISerializer<C> CSerializer { set; get; }
        private IComparer<C> CComparer { set; get; }
        private Genome<C, I, M> genome { set; get; }

        public ExecutionReport Add(Dictionary<string, Dictionary<char, List<I>>> peaks, IndexingMode indexingMode, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.Add(peaks, '*', indexingMode, maxDegreeOfParallelism);
        }
        public ExecutionReport Add2ndPass(MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.Add2ndPass(maxDegreeOfParallelism);
        }
        public ExecutionReport Cover(CoverVariation coverVariation, char strand, int minAcc, int maxAcc, Aggregate aggregate, out FunctionOutput<Output<C, I, M>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.Cover(coverVariation, strand, minAcc, maxAcc, aggregate, out result, maxDegreeOfParallelism);
        }
        public ExecutionReport Map(char strand, Dictionary<string, Dictionary<char, List<I>>> references, Aggregate aggregate, out FunctionOutput<Output<C, I, M>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.Map(references, strand, aggregate, out result, maxDegreeOfParallelism);
        }
        public ExecutionReport AccumulationHistogram(out Dictionary<string, Dictionary<char, List<AccEntry<C>>>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.AccumulationHistogram(out result, maxDegreeOfParallelism);
        }
        public ExecutionReport AccumulationDistribution(out Dictionary<string, Dictionary<char, SortedDictionary<int, int>>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.AccumulationDistribution(out result, maxDegreeOfParallelism);
        }
        public ExecutionReport SecondResolutionIndex(MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.SecondResolutionIndex(maxDegreeOfParallelism);
        }
        public ExecutionReport Merge(out Dictionary<string, Dictionary<char, ICollection<BlockKey<C>>>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.Merge(out result, maxDegreeOfParallelism);
        }
        public ExecutionReport Complement(out Dictionary<string, Dictionary<char, ICollection<BlockKey<C>>>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.Complement(out result, maxDegreeOfParallelism);
        }
    }
}
