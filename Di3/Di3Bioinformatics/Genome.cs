using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IInterval;
using ICPMD;
using DI3;
using DI3.Interfaces;
using CSharpTest.Net.Serialization;

namespace Di3Bioinformatics
{
    public class Genome<C, I, M> : BaseGenome<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>, new()
        where M : ICPMetadata<C>, IMetaData<C>, new()
    {
        /*internal Genome(byte chrCount, ISerializer<C> CoordinateSerializer)
        {
            int cpuCount = Environment.ProcessorCount;
            coordinateSerializer = CoordinateSerializer;
        }*/



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
            /*foreach (var chrKey in peaks)
            {
                AddChromosome(chrKey.Key);
                for (int p = 0; p < 100000000; p++)
                {
                    chrs[chrKey.Key].di3Unstranded.Add();//chrPeaks[p]);
                }
            }*/

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
            /*foreach (var chrKey in peaks)
            {
                AddChromosome(chrKey.Key);

                for (int i = 0; i < 100000000; i++)
                    chrs[chrKey.Key].di3Unstranded.Add();//chrPeaks[p]);
            }*/



            // TEST 11: NO NO NO
            //di3Unstranded is not owned by this function rather it is owned by the class.
            /*AddChromosome("chr1");
            for (int i = 0; i < 10000000; i++)
            {
                chrs["chr1"].di3Unstranded.Add(
                    new I()
                            {
                                left = default(C),//chrPeaks[p].left,
                                right = default(C),//chrPeaks[p].right,
                                metadata = default(M)
                            });
            }*/

            // TEST 12: OK ------- WORKED GREAT
            // use a proprietary instance of Di3. 
            // Occupies memory up to 100MB. 
            /*ISerializer<Int32> MycoordinateSerializer = PrimitiveSerializer.Int32;

            var myDi3 = new Di3<int, PeakClass, PeakDataClass>(MycoordinateSerializer);
            for (int i = 0; i < 10000000; i++)
            {
                myDi3.Add(new PeakClass()
                {
                    left = i,
                    right = i + 2,
                    metadata = new PeakDataClass()
                    {
                        chrNo = 1,
                        left = i,
                        right = i + 2,
                        name = "Hamed",
                        strand = '*',
                        hashKey = 100000,
                        value = 100
                    }
                });

                Console.Write("\rAdded : {0:N0}", i);
            }*/





            // TEST 13: NO NO NO
            /*var myDi313 = new Di3<C, I, M>(coordinateSerializer);
            for (int i = 0; i < 10000000; i++)
            {
                myDi313.Add(new I()
                {
                    left = default(C),
                    right = default(C),
                    metadata = new M()
                    {
                        chrNo = 1,
                        left = default(C),
                        right = default(C),
                        name = "ksdjhfkshf",
                        strand = '*',
                        value = 23984293,
                        hashKey = 9328578329
                    }
                });

                Console.Write("\rAdded : {0:N0}", i);
            }*/





            // Test 14: OK, worked fine. 
            // Here I have my proprietary classes and serializer
            // and data are generate in place. 
            /*AddChromosome("chr1");
            int tst13Counter = -1;

            int loopCounter = 0;
            foreach (var chrKey in peaks)
            {
                loopCounter = (50 * chrKey.Value.Count);

                ISerializer<Int32> MycoordinateSerializer14 = PrimitiveSerializer.Int32;
                var myDi314 = new Di3<int, PeakClass, PeakDataClass>(MycoordinateSerializer14);

                for (int p = 0; p < loopCounter; p++)
                {
                    myDi314.Add(new PeakClass()
                    {
                        left = p,
                        right = p + 2,
                        metadata = new PeakDataClass()
                        {
                            chrNo = 1,
                            left = p,
                            right = p + 10,
                            name = "Hamed",
                            strand = '*',
                            hashKey = 100000,
                            value = 100
                        }
                    });

                    Console.Write("\rAdded : {0:N0}", ++tst13Counter);
                }
            }*/


            // Test 15: OK, worked fine.
            /*AddChromosome("chr1");
            int tst13Counter = -1;

            foreach (var chrKey in peaks)
            {
                ISerializer<Int32> MycoordinateSerializer14 = PrimitiveSerializer.Int32;
                var myDi314 = new Di3<int, PeakClass, PeakDataClass>(MycoordinateSerializer14);

                for (int p = 0; p < 50 * chrKey.Value.Count; p++)
                {
                    myDi314.Add(new PeakClass()
                    {
                        left = p,
                        right = p + 2,
                        metadata = new PeakDataClass()
                        {
                            chrNo = 1,
                            left = p,
                            right = p + 10,
                            name = "Hamed",
                            strand = '*',
                            hashKey = 100000,
                            value = 100
                        }
                    });

                    Console.Write("\rAdded : {0:N0}", ++tst13Counter);
                }
            }*/


            // TEST 16: OK, worked perfect. Memory fixed at 140MB
            // This test adds Di3 to dictionary.
            /*AddChromosome("chr1");
            int tst16Counter = -1;

            var myDi316 = new Dictionary<string, Di3<int, PeakClass, PeakDataClass>>();
            ISerializer<Int32> MycoordinateSerializer16 = PrimitiveSerializer.Int32;            

            foreach (var chrKey in peaks)
            {
                myDi316.Add(chrKey.Key, new Di3<int, PeakClass, PeakDataClass>(MycoordinateSerializer16));

                for (int p = 0; p < 10 * chrKey.Value.Count; p++)
                {
                    myDi316[chrKey.Key].Add(new PeakClass()
                    {
                        left = p,
                        right = p + 2,
                        metadata = new PeakDataClass()
                        {
                            chrNo = 1,
                            left = p,
                            right = p + 10,
                            name = "Hamed",
                            strand = '*',
                            hashKey = 100000,
                            value = 100
                        }
                    });

                    Console.Write("\rAdded : {0:N0}", ++tst16Counter);
                }
            }*/




            // not working example : 

            // TEST 17: 
            AddChromosome("chr1");
            int tst13Counter = -1;

            var myDi317 = new Dictionary<string, Di3<C, I, M>>();
            myDi317.Add("chr", new Di3<C, I, M>(coordinateSerializer, coordinateComparer));

            foreach (var chrKey in peaks)
            {
                //myDi317.Add(chrKey.Key, new Di3<C, I, M>(coordinateSerializer));

                for (int p = 0; p < chrKey.Value.Count; p++)
                {
                    myDi317["chr"].Add(new I()
                    {
                        left = chrKey.Value[p].left,
                        right = chrKey.Value[p].right,
                        metadata = new M()
                        {
                            chrNo = chrKey.Value[p].metadata.chrNo,
                            left = chrKey.Value[p].metadata.left,
                            right = chrKey.Value[p].metadata.right,
                            name = chrKey.Value[p].metadata.name,
                            strand = chrKey.Value[p].metadata.strand,
                            hashKey = chrKey.Value[p].metadata.hashKey,
                            value = chrKey.Value[p].metadata.value
                        }
                    });

                    Console.Write("\rAdded : {0:N0}", ++tst13Counter);
                }
            }











            int i2 = -1;
            /*foreach (var chrKey in peaks)
            {
                var chrPeaks = chrKey.Value;
                AddChromosome(chrKey.Key);

                for (int p = 0; p < chrPeaks.Count; p++)
                {
                    switch (chrPeaks[p].metadata.strand)
                    {
                        case '+':
                            //chrs[chrKey.Key].di3PositiveStrand.Add(chrPeaks[p]);
                            break;

                        case '-':
                            //chrs[chrKey.Key].di3NegativeStrand.Add(chrPeaks[p]);
                            break;

                        case '*':
                            chrs[chrKey.Key].di3Unstranded.Add(chrPeaks[p]);
                            chrs[chrKey.Key].di3Unstranded.Add(new I()
                            {
                                left = default(C),//chrPeaks[p].left,
                                right = default(C),//chrPeaks[p].right,
                                metadata = default(M)/* new M()
                                {
                                    chrNo = chrPeaks[p].metadata.chrNo,
                                    left = chrPeaks[p].metadata.left,
                                    right = chrPeaks[p].metadata.right,
                                    name = chrPeaks[p].metadata.name,
                                    value = chrPeaks[p].metadata.value,
                                    strand = chrPeaks[p].metadata.strand,
                                    hashKey = chrPeaks[p].metadata.hashKey
                                }*/
            //});

            //break;
            //}

            // Console.Write("\rAdded : {0:N0}", ++i2);

            /*if (++gcCounter >= 10000)
            {
                chrs[chrKey.Key].di3PositiveStrand.Clean();
                chrs[chrKey.Key].di3NegativeStrand.Clean();
                chrs[chrKey.Key].di3Unstranded.Clean();
                GC.Collect();
                gcCounter = -1;
            }*/
            //}

            //chrs[chrKey.Key].di3PositiveStrand.Clean();
            //chrs[chrKey.Key].di3NegativeStrand.Clean();
            //chrs[chrKey.Key].di3Unstranded.Clean();
            //}
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
}

