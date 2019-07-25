using CSharpTest.Net.Serialization;
using Genometric.Di4.Di4B.Logging;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Genometric.Di4.Di4B
{
    public class Di4B<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>, IFormattable, new()
        where M : IMetaData, IFormattable, new()
    {
        public Di4B(string workingDirectory, string sectionTitle, Memory Memory, HDDPerformance hddPerformance, CacheOptions cacheOptions, ISerializer<C> CSerializer, IComparer<C> CComparer)
        {
            this.CSerializer = CSerializer;
            this.CComparer = CComparer;

            genome = new Genome<C, I, M>(workingDirectory, sectionTitle, Memory, hddPerformance, cacheOptions, CSerializer, CComparer);
        }

        private ISerializer<C> CSerializer { set; get; }
        private IComparer<C> CComparer { set; get; }
        private Genome<C, I, M> genome { set; get; }

        public ExecutionReport Add(uint collectionID, Dictionary<string, Dictionary<char, List<I>>> peaks, IndexingMode indexingMode, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.Add(collectionID, peaks, '*', indexingMode, maxDegreeOfParallelism);
        }
        public ExecutionReport Add2ndPass(MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.Add2ndPass(maxDegreeOfParallelism);
        }
        public void CommitIndexedData(MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            genome.CommitIndexedData('*', maxDegreeOfParallelism);
        }

        public ExecutionReport Cover(CoverVariation coverVariation, char strand, int minAcc, int maxAcc, Aggregate aggregate, out FunctionOutput<Output<C, I, M>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.Cover(coverVariation, strand, minAcc, maxAcc, aggregate, out result, maxDegreeOfParallelism);
        }
        public ExecutionReport Map(char strand, Dictionary<string, Dictionary<char, List<I>>> references, Aggregate aggregate, out FunctionOutput<Output<C, I, M>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.Map(references, strand, aggregate, out result, maxDegreeOfParallelism);
        }
        public ExecutionReport VariantAnalysis(char strand, Dictionary<string, Dictionary<char, List<I>>> references, Aggregate aggregate, out FunctionOutput<Output<C, I, M>> result, MaxDegreeOfParallelism maxDegreeOfParallelism, out Dictionary<uint, int> newRes)
        {
            return genome.VariantAnalysis(references, strand, aggregate, out result, out newRes, maxDegreeOfParallelism);
        }
        public ExecutionReport LambdaSizeStats(out SortedDictionary<int, int> results)
        {
            return genome.LambdaSizeStats(out results);
        }
        public ExecutionReport AccumulationHistogram(out ConcurrentDictionary<string, ConcurrentDictionary<char, List<AccEntry<C>>>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.AccumulationHistogram(out result, maxDegreeOfParallelism);
        }
        public ExecutionReport AccumulationDistribution(out ConcurrentDictionary<string, ConcurrentDictionary<char, SortedDictionary<int, int>>> result, out SortedDictionary<int, int> mergedResult, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.AccumulationDistribution(out result, out mergedResult, maxDegreeOfParallelism);
        }
        public ExecutionReport SecondResolutionIndex(CuttingMethod cuttingMethod, int binCount, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.SecondResolutionIndex(cuttingMethod, binCount, maxDegreeOfParallelism);
        }
        public ExecutionReport Merge(out ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<C>>>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.Merge(out result, maxDegreeOfParallelism);
        }
        public ExecutionReport Complement(out ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<C>>>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.Complement(out result, maxDegreeOfParallelism);
        }
        public ExecutionReport Dichotomies(out ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<C>>>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.Dichotomies(out result, maxDegreeOfParallelism);
        }
        public ExecutionReport BlocksInfoDistribution(out BlockInfoDis result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            return genome.BlocksInfoDistribution(out result, maxDegreeOfParallelism);
        }
        public ExecutionReport Statistics(out SortedDictionary<string, SortedDictionary<char, Stats>> result)
        {
            return genome.Statistics(out result);
        }
    }
}
