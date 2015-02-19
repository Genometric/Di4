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

        public ExecutionReport Add(Dictionary<string, Dictionary<char, List<I>>> peaks, IndexingMode indexingMode, int nThreads)
        {
            return genome.Add(peaks, '*', indexingMode, nThreads);
        }
        public ExecutionReport Add2ndPass()
        {
            return genome.Add2ndPass();
        }
        public ExecutionReport Cover(CoverVariation coverVariation, char strand, byte minAcc, byte maxAcc, Aggregate aggregate, out FunctionOutput<Output<C, I, M>> result, int nThreads)
        {
            return genome.Cover(coverVariation, strand, minAcc, maxAcc, aggregate, out result, nThreads);
        }
        public ExecutionReport Map(char strand, Dictionary<string, Dictionary<char, List<I>>> references, Aggregate aggregate, out FunctionOutput<Output<C, I, M>> result, int nThreads)
        {
            return genome.Map(references, strand, aggregate, out result, nThreads);
        }
        public ExecutionReport AccumulationHistogram(out Dictionary<string, Dictionary<char, List<AccEntry<C>>>> result, int nThreads)
        {
            return genome.AccumulationHistogram(out result, nThreads);
        }
        public ExecutionReport AccumulationDistribution(out Dictionary<string, Dictionary<char, SortedDictionary<int, int>>> result, int nThreads)
        {
            return genome.AccumulationDistribution(out result, nThreads);
        }
        public ExecutionReport SecondResolutionIndex(int nThreads)
        {
            return genome.SecondResolutionIndex(nThreads);
        }
        public ExecutionReport Merge(out Dictionary<string, Dictionary<char, SortedDictionary<BlockKey<C>,int>>> result, int nThreads)
        {
            return genome.Merge(out result, nThreads);
        }
        public ExecutionReport Complement(out Dictionary<string, Dictionary<char, SortedDictionary<BlockKey<C>, int>>> result, int nThreads)
        {
            return genome.Complement(out result, nThreads);
        }
    }
}
