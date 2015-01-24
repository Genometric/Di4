using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Di3BCLI;
using System.Diagnostics;
using System.IO;
using CSharpTest.Net.Threading;

namespace Di3WithCustomBlockSerializer
{
    class Program
    {
        static void Main(string[] args)
        {
            Int32Comparer int32Comparer = new Int32Comparer();
            LambdaItemSerializer lambdaItemSerializer = new LambdaItemSerializer();
            LambdaArraySerializer lambdaArraySerializer = new LambdaArraySerializer(lambdaItemSerializer);
            BlockSerializer blockSerializer = new BlockSerializer(lambdaArraySerializer);

            var options = new BPlusTree<int, B>.OptionsV2(PrimitiveSerializer.Int32, blockSerializer, int32Comparer);

            options.MinimumChildNodes = 2;
            options.MaximumChildNodes = 256;
            options.MinimumValueNodes = 2;
            options.MaximumValueNodes = 256;

            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.CreateFile = CreatePolicy.IfNeeded;
            options.FileName = @"D:\testtesttest.idx";

            int right = 0;
            int left = 0;
            StreamWriter writer = new StreamWriter(@"D:\" + Path.DirectorySeparatorChar + "speed________________.txt");
            Stopwatch stopWatch = new Stopwatch();
            Random rnd = new Random();
            Stopwatch watch = new Stopwatch();

            for (int sample = 0; sample < 400; sample++)
            {
                Console.WriteLine("processing sample   : {0:N0}", sample);

                using (var di3 = new BPlusTree<int, B>(options))
                {
                    List<Intervals> intervals = new List<Intervals>();

                    for (int intvrl = 1; intvrl <= 200000; intvrl++)
                    {
                        left = right + rnd.Next(50, 500);
                        right = left + rnd.Next(500, 1000);

                        intervals.Add(new Intervals()
                        {
                            left = left,
                            right = right,
                            hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000)
                        });

                        //Console.Write("\r#Inserted intervals : {0:N0}", intervals);
                    }

                    stopWatch.Restart();


                    int start = 0, stop = 0, range = (int)Math.Ceiling(intervals.Count / 4.0);
                    using (WorkQueue work = new WorkQueue(4))
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            start = i * range;
                            stop = (i + 1) * range;
                            if (stop > intervals.Count) stop = intervals.Count;
                            work.Enqueue(new INDEX(di3, intervals, start, stop).Index);
                        }

                        watch.Restart();
                        work.Complete(true, -1);
                        watch.Stop();
                        Console.WriteLine("waited : {0}ms", watch.ElapsedMilliseconds);
                    }


                    stopWatch.Stop();
                    //Console.WriteLine("");
                    Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(200000 / stopWatch.Elapsed.TotalSeconds, 2));
                    Console.WriteLine("");

                    writer.WriteLine(Math.Round(200000 / stopWatch.Elapsed.TotalSeconds, 2).ToString() + "\t" + stopWatch.Elapsed.ToString() + "\t" + stopWatch.ElapsedMilliseconds.ToString());
                    writer.Flush();
                }
            }



            // basic tests
            /*Stopwatch stpwtch = new Stopwatch();
            stpwtch.Restart();
            for (int i = 0; i < 200000; i++)
                di3.Add(i, new B('L', 100));
            stpwtch.Stop();
            var et = stpwtch.Elapsed;*/
        }
    }
}
