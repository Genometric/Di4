using CSharpTest.Net.Serialization;
using Polimi.DEIB.VahidJalili.DI3.DI3B.Logging;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;
using Polimi.DEIB.VahidJalili.DI3;

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

        public ExecutionReport Add(Dictionary<string, Dictionary<char, List<I>>> peaks, IndexingMode indexingMode)
        {
            return genome.Add(peaks, '*', indexingMode);
        }
        public ExecutionReport Add2ndPass()
        {
            return genome.Add2ndPass();
        }
        public ExecutionReport Cover(CoverVariation coverVariation, char strand, byte minAcc, byte maxAcc, Aggregate aggregate, out FunctionOutput<Output<C, I, M>> result)
        {
            return genome.Cover(coverVariation, strand, minAcc, maxAcc, aggregate, out result);
        }
        public ExecutionReport Map(char strand, Dictionary<string, Dictionary<char, List<I>>> references, Aggregate aggregate, out FunctionOutput<Output<C, I, M>> result)
        {
            return genome.Map(references, strand, aggregate, out result);
        }
        public ExecutionReport SecondResolutionIndex()
        {
            return genome.SecondResolutionIndex();
        }
    }
}
