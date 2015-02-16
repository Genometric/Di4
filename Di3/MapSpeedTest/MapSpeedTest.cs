using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using DI3;
using Di3B;
using Di3BCLI;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MapSpeedTest
{
    class MapSpeedTest
    {
        Int32Comparer int32Comparer = new Int32Comparer();
        int cpuCount = Environment.ProcessorCount;

        public void Run(string IndexFile,
            int RegionCount,
            int MinLenght,
            int MaxLenght)
        {
            /// the following were using LightPeak.
            List<Peak> Peaks = new List<Peak>();
            Random rnd = new Random();
            int rndLeft = 0;
            int rndRight = 0;
            Stopwatch stopWatch = new Stopwatch();

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                IndexFile);

            options.MinimumChildNodes = 2;
            options.MaximumChildNodes = 256;
            options.MinimumValueNodes = 2;
            options.MaximumValueNodes = 256;

            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.None;

            options.StoragePerformance = StoragePerformance.Fastest;

            using (var di3 = new Di3<int, Peak, PeakData>(options))
            {
                for (int i = 0; i < 10; i++)
                {
                    Peaks.Clear();

                    /// Creating some random regions. 
                    //Console.WriteLine("");
                    //Console.Write("Creating Random Files ...  ");

                    //// TEST, DELET AFTER TEST.
                    //for (int sampleCount = 0; sampleCount < 50; sampleCount++)
                    //{
                        for (int r = 0; r < RegionCount; r++)
                        {
                            rndLeft = rnd.Next(/*rndRight + 10, rndRight + 10012);*/0, 10000000);
                            rndRight = rndLeft + rnd.Next(MinLenght, MaxLenght);

                            Peaks.Add(new Peak()
                            {
                                left = rndLeft,
                                right = rndRight,
                                hashKey = (uint)rnd.Next(1, 1000000)
                            });
                        }
                        //Console.Write("Done!");
                        //Console.WriteLine("");
                        rndRight = 0;


                        /// TEST DELET LATER
                        //using (var writer = new System.IO.StreamWriter(@"F:\simulationB\sample_" + sampleCount + ".bed"))
                        //    foreach (var peak in Peaks)
                        //        writer.WriteLine("chr1\t" + peak.left + "\t" + peak.right + "\t" + peak.hashKey + "\t" + peak.hashKey);
                        //Peaks.Clear();


                    //} break;

                    stopWatch.Restart();

                    /// the following functions were using Light version of Peak and PeakData.
                    var outputStrategy = new AggregateFactory<int, Peak, PeakData>().GetAggregateFunction(Aggregate.Count);
                    di3.Map<Output<int, Peak, PeakData>>(ref outputStrategy, Peaks, cpuCount);
                    stopWatch.Stop();
                    Console.WriteLine("");
                    Console.WriteLine("ET: {0}", stopWatch.Elapsed);
                }
            }
            Console.ReadKey();
        }
    }
}
