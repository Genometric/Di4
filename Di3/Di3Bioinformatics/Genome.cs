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
        where I : IInterval<int, M>
        where M : ICPMetadata<int>, IMetaData<int>
    {
        internal Genome(byte chrCount)
        {
            int cpuCount = Environment.ProcessorCount;
        }

        

        internal void Add(Dictionary<string, List<I>> peaks)
        {
            // TEST 1 : OK
            //Di3</*C,*/ I, M> di3 = new Di3<I, M>();
            //for (int i = 0; i < 1000000; i++)
            //{
              //  di3.Add();
            //}


            // TEST 2 : OK  
            //Dictionary<string, Di3<I, M>> TESTDIC = new Dictionary<string, Di3<I, M>>();
            //TESTDIC.Add("chr", new Di3<I, M>());
            //for (int i = 0; i < 1000000; i++)
            //{
                //TESTDIC["chr"].Add();
            //}


            // TEST 3 : OK
            //Dictionary<string, TESTCLASS> TESTDIC = new Dictionary<string, TESTCLASS>();
            //TESTDIC.Add("chr", new TESTCLASS());
            //for (int i = 0; i < 100000000; i++)
            //{
            //    TESTDIC["chr"].di3.Add();
            //}


            // TEST 4 : OK
            //Dictionary<string, TESTCLASS> TESTDIC = new Dictionary<string, TESTCLASS>();
            //TESTDIC.Add("chr", new TESTCLASS());
            //for (int i = 0; i < 100000000; i++)
            //{
            //    TESTDIC["chr"].di3A.Add();
            //}


            // TEST 5 : OK
            //AddChromosome("chr");
            //for (int i = 0; i < 100000000; i++)
            //{
            //    chrs["chr"].di3NegativeStrand.Add();
            //}


            // TEST 6 : OK
            //foreach (var chrKey in peaks)
            //{
            //    AddChromosome(chrKey.Key);
            //    for (int p = 0; p < 100000000; p++)
            //    {
            //        chrs[chrKey.Key].di3Unstranded.Add();//chrPeaks[p]);
            //    }
            //}

            // TEST 7 : NO NO NO
            /*foreach (var chrKey in peaks)
            {
                var chrPeaks = chrKey.Value;
                AddChromosome(chrKey.Key);

                for (int p = 0; p < chrPeaks.Count; p++)                
                    chrs[chrKey.Key].di3Unstranded.Add();//chrPeaks[p]);                
            }*/

            // TEST 8 : NO NO NO 
            /*foreach (var chrKey in peaks)
            {
                var chrPeaks = chrKey.Value;
                AddChromosome(chrKey.Key);

                foreach (var p in chrPeaks)
                    chrs[chrKey.Key].di3Unstranded.Add();//chrPeaks[p]);
            }*/

            // TEST 9 : NO NO NO 
            /*foreach (var chrKey in peaks)
            {
                AddChromosome(chrKey.Key);

                foreach (var p in chrKey.Value)
                    chrs[chrKey.Key].di3Unstranded.Add();//chrPeaks[p]);
            }*/

            // TEST 10 : NO NO NO ---------- OK OK OK 
            // This test and most of the above tests are working fine only
            // when peaks is not initialized with lots of PeakClasses from 
            // Orchestrator class. 
            // Peaks is tested with different items size. 
            // Actually there is not problem, only that when peaks contains
            // lots of items, the object occupies a lot of memory and this 
            // raises issues with BPlusTree making it slow and trying to 
            // make more memory occupancy requests. 
            // Peaks is tested with 2,000 & 20,000 & 200,000 & 2,000,000 & 
            // 20,000,000 & 25,000,000 objects in it. more than 25,000,000
            // raises insufficient memory exception.
            foreach (var chrKey in peaks)
            {
                AddChromosome(chrKey.Key);

                for (int i = 0; i < 100000000; i++)
                    chrs[chrKey.Key].di3Unstranded.Add();//chrPeaks[p]);
            }


            foreach (var chrKey in peaks)
            {
                var chrPeaks = chrKey.Value;
                AddChromosome(chrKey.Key);

                for (int p = 0; p < chrPeaks.Count; p++)
                {
                    switch (chrPeaks[p].metadata.strand)
                    {
                        case '+':
                            chrs[chrKey.Key].di3PositiveStrand.Add();//chrPeaks[p]);
                            break;

                        case '-':
                            chrs[chrKey.Key].di3NegativeStrand.Add();//chrPeaks[p]);
                            break;

                        case '*':
                            chrs[chrKey.Key].di3Unstranded.Add();//chrPeaks[p]);
                            break;
                    }
                }
            }
        }

        internal class TESTCLASS
        {
            internal Di3<I, M> di3A { set; get; }
            internal Di3<I, M> di3B { set; get; }

            internal TESTCLASS()
            {
                di3A = new Di3<I, M>();
                di3B = new Di3<I, M>();
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
                                    /*output.chrs[chrKey.Key].outputPositiveStrand =
                                        chrs[chrKey.Key].di3PositiveStrand.Cover<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);*/
                                    break;

                                case "summit":
                                    /*output.chrs[chrKey.Key].outputPositiveStrand =
                                        chrs[chrKey.Key].di3PositiveStrand.Summit<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);*/
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
                                    /*output.chrs[chrKey.Key].outputNegativeStrand =
                                        chrs[chrKey.Key].di3NegativeStrand.Cover<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);*/
                                    break;

                                case "summit":
                                    /*output.chrs[chrKey.Key].outputNegativeStrand =
                                        chrs[chrKey.Key].di3NegativeStrand.Summit<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);*/
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
                                    /*output.chrs[chrKey.Key].outputUnstranded =
                                        chrs[chrKey.Key].di3Unstranded.Cover<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);*/
                                    break;

                                case "summit":
                                    /*output.chrs[chrKey.Key].outputUnstranded =
                                        chrs[chrKey.Key].di3Unstranded.Summit<Output<C>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);*/
                                    break;
                            }
                        }
                        break;
                }

            }

            return output;
        }
    }


    public class TESTPeakClass<C> : IInterval<C, TESTPeakDataClass<C>>
    {
        public C left { set; get; }

        public C right { set; get; }

        public TESTPeakDataClass<C> metadata { set; get; }
    }

    public class TESTPeakDataClass<C> : ICPMetadata<C>
    {
        public byte chrNo { set; get; }

        public string name { set; get; }

        public double value { set; get; }

        public char strand { set; get; }

        public C left { set; get; }

        public C right { set; get; }

        public ulong hashKey { set; get; }
    }
}

