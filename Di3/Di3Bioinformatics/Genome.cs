using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IInterval;
using ICPMD;
using DI3;

namespace Di3Bioinformatics
{
    internal class Genome<C, I, M> : BaseGenome<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : ICPMetadata<C>, IMetaData<C>
    {
        internal Genome(byte chrCount)
            : base(chrCount)
        {
            int cpuCount = Environment.ProcessorCount;
        }

        internal void Add(List<List<I>> peaks)
        {
            for (int chr = 0; chr < peaks.Count; chr++)
            {
                for (int p = 0; p < peaks[chr].Count; p++)
                {
                    switch (peaks[chr][p].metadata.strand)
                    {
                        case '+':
                            chrs[chr].di3PositiveStrand.Add(peaks[chr][p]);
                            break;

                        case '-':
                            chrs[chr].di3NegativeStrand.Add(peaks[chr][p]);
                            break;

                        case '*':
                            chrs[chr].di3Unstranded.Add(peaks[chr][p]);
                            break;
                    }
                }
            }
        }

        internal FunctionOutput<Output<C>> CoverSummit(string function, char strand, byte minAcc, byte maxAcc, string aggregate)
        {
            FunctionOutput<Output<C>> output = new FunctionOutput<Output<C>>(chrs.Count);

            AggregateFactory<C, I, M> aggFactory = new AggregateFactory<C, I, M>();

            for (int chr = 0; chr < chrs.Count; chr++)
            {
                switch (strand)
                {
                    case '+':
                        if (chrs[chr].di3PositiveStrand.blockCount > 0)
                        {
                            switch(function)
                            {
                                case "cover":
                                    output.chrs[chr].outputPositiveStrand = 
                                        chrs[chr].di3PositiveStrand.Cover<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;

                                case "summit":
                                    output.chrs[chr].outputPositiveStrand = 
                                        chrs[chr].di3PositiveStrand.Summit<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;
                            }
                        }
                        break;

                    case '-':
                        if (chrs[chr].di3NegativeStrand.blockCount > 0)
                        {
                            switch (function)
                            {
                                case "cover":
                                    output.chrs[chr].outputNegativeStrand = 
                                        chrs[chr].di3NegativeStrand.Cover<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;

                                case "summit":
                                    output.chrs[chr].outputNegativeStrand = 
                                        chrs[chr].di3NegativeStrand.Summit<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;
                            }
                        }   
                        break;

                    case '*':
                        if (chrs[chr].di3Unstranded.blockCount > 0)
                        {
                            switch(function)
                            {
                                case "cover":
                                    output.chrs[chr].outputUnstranded = 
                                        chrs[chr].di3Unstranded.Cover<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;

                                case "summit":
                                    output.chrs[chr].outputUnstranded = 
                                        chrs[chr].di3Unstranded.Summit<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
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
