using System;
using System.Collections.Generic;
using Interfaces;
using CSharpTest.Net.Serialization;
using Di3B.Logging;


namespace Di3B
{
    public class Di3B<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>, new()
        where M : IExtMetaData<C>, new()
    {
        private ISerializer<C> CSerializer { set; get; }
        private IComparer<C> CComparer { set; get; }

        private Genome<C, I, M> genome { set; get; }
        public Di3B(string Di3Path, Memory Memory, HDDPerformance hddPerformance, ISerializer<C> CSerializer, IComparer<C> CComparer)
        {
            this.CSerializer = CSerializer;
            this.CComparer = CComparer;
            
            /// TODO: Two constructors are needed, with (for Memory = RAM) & without (for Memory = HDD) HDDPerformance.
            genome = new Genome<C, I, M>(Di3Path, Memory, hddPerformance, CSerializer, CComparer);
        }

        public ExecutionReport Add(Dictionary<string, List<I>> peaks)
        {
            return genome.Add(peaks, '*');
        }


        public FunctionOutput<Output<C, I, M>> Cover(char strand, byte minAcc, byte maxAcc, string aggregate)
        {
            return genome.CoverSummit("cover", strand, minAcc, maxAcc, aggregate);
        }

        public FunctionOutput<Output<C, I, M>> Summit(char strand, byte minAcc, byte maxAcc, string aggregate)
        {
            return genome.CoverSummit("summit", strand, minAcc, maxAcc, aggregate);
        }

        public FunctionOutput<Output<C, I, M>> Map(char strand, Dictionary<string, List<I>> references, string aggregate)
        {
            return genome.Map(references, strand, aggregate);
        }
    }
}
