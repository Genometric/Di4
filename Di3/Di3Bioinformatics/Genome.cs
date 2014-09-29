using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IInterval;
using ICPMD;
using DI3;
using DI3.Interfaces;

namespace Di3Bioinformatics
{
    internal class Genome<C, I, M> : BaseGenome<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : ICPMetadata<C>, IMetaData<C>
    {
        internal Genome(byte chrCount)
        {
            int cpuCount = Environment.ProcessorCount;
        }

        internal void Add(Dictionary<string, List<I>> peaks)
        {
            foreach(var chrKey in peaks)
            {
                var chrPeaks = chrKey.Value;
                AddChromosome(chrKey.Key);

                for (int p = 0; p < chrPeaks.Count; p++)
                {
                    switch (chrPeaks[p].metadata.strand)
                    {
                        case '+':
                            chrs[chrKey.Key].di3PositiveStrand.Add(chrPeaks[p]);
                            break;

                        case '-':
                            chrs[chrKey.Key].di3NegativeStrand.Add(chrPeaks[p]);
                            break;

                        case '*':
                            chrs[chrKey.Key].di3Unstranded.Add(chrPeaks[p]);
                            break;
                    }
                }
            }
        }

        internal FunctionOutput<Output<C>> CoverSummit(string function, char strand, byte minAcc, byte maxAcc, string aggregate)
        {
            FunctionOutput<Output<C>> output = new FunctionOutput<Output<C>>(chrs.Count);

            AggregateFactory<C, I, M> aggFactory = new AggregateFactory<C, I, M>();

            foreach(var chrKey in chrs)
            {
                switch (strand)
                {
                    case '+':
                        if (chrKey.Value.di3PositiveStrand.blockCount > 0)
                        {
                            switch (function)
                            {
                                case "cover":
                                    output.chrs[chrKey.Key].outputPositiveStrand =
                                        chrs[chrKey.Key].di3PositiveStrand.Cover<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;

                                case "summit":
                                    output.chrs[chrKey.Key].outputPositiveStrand =
                                        chrs[chrKey.Key].di3PositiveStrand.Summit<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;
                            }
                        }
                        break;

                    case '-':
                        if (chrs[chrKey.Key].di3NegativeStrand.blockCount > 0)
                        {
                            switch (function)
                            {
                                case "cover":
                                    output.chrs[chrKey.Key].outputNegativeStrand =
                                        chrs[chrKey.Key].di3NegativeStrand.Cover<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;

                                case "summit":
                                    output.chrs[chrKey.Key].outputNegativeStrand =
                                        chrs[chrKey.Key].di3NegativeStrand.Summit<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;
                            }
                        }
                        break;

                    case '*':
                        if (chrs[chrKey.Key].di3Unstranded.blockCount > 0)
                        {
                            switch (function)
                            {
                                case "cover":
                                    output.chrs[chrKey.Key].outputUnstranded =
                                        chrs[chrKey.Key].di3Unstranded.Cover<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;

                                case "summit":
                                    output.chrs[chrKey.Key].outputUnstranded =
                                        chrs[chrKey.Key].di3Unstranded.Summit<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;
                            }
                        }
                        break;
                }

            }

            return output;
        }
    }
}
