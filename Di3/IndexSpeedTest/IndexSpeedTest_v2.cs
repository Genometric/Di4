using System;
using System.IO;
using System.Diagnostics;
using DI3;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using Di3BMain;
using System.Collections.Generic;

namespace IndexSpeedTest
{
    public class IndexSpeedTest_v2
    {
        private StreamWriter writer { set; get; }
        private Stopwatch stopWatch = new Stopwatch();
        Random rnd = new Random();
        private Int32Comparer int32Comparer = new Int32Comparer();

        public void Run(
            int sampleCount,
            int regionCount,
            bool disposeDi3atEachSample,
            string outputPath,
            string TestName,
            int minGap,
            int maxGap,
            int minLenght,
            int maxLenght,
            Di3Options<int> options,
            Mode mode)
        {
            int right = 0;
            int left = 0;

            if (!Directory.Exists(outputPath) && outputPath.Trim() != string.Empty) Directory.CreateDirectory(outputPath);

            writer = new StreamWriter(outputPath + Path.DirectorySeparatorChar + "speed" + TestName + ".txt");
            writer.WriteLine("Di3 indexing speed test: " + TestName);
            writer.WriteLine("Speed(interval/sec)\tET\tET(ms)");

            if (disposeDi3atEachSample)
            {
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);

                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(options))
                    {
                        stopWatch.Restart();

                        List<LightPeak> peaks = new List<LightPeak>();

                        for (int intervals = 1; intervals <= regionCount; intervals++)
                        {
                            left = right + rnd.Next(minGap, maxGap);
                            right = left + rnd.Next(minLenght, maxLenght);

                            peaks.Add(new LightPeak()
                            {
                                left = left,
                                right = right,
                                metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                            });

                            //Console.Write("\r#Inserted intervals : {0:N0}", intervals);
                        }

                        di3.Add(peaks, mode);

                        stopWatch.Stop();
                        //Console.WriteLine("");
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine("");

                        writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2).ToString() + "\t" + stopWatch.Elapsed.ToString() + "\t" + stopWatch.ElapsedMilliseconds.ToString());
                        writer.Flush();
                    }
                }

                if (mode == Mode.MultiPass)
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(options))
                    {
                        Console.WriteLine(".::.     SECOND PASS    .::.");
                        stopWatch.Restart();
                        int TESTBlockCount = di3.SecondPass();
                        stopWatch.Stop();
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(TESTBlockCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine(".::. Total of {0:N0} blocks processed in {1}", TESTBlockCount, stopWatch.Elapsed.ToString());
                    }
            }
            else
            {
                using (var di3 = new Di3<int, LightPeak, LightPeakData>(options))
                {

                    for (int sample = 0; sample < sampleCount; sample++)
                    {
                        Console.WriteLine("processing sample   : {0:N0}", sample);
                        stopWatch.Restart();

                        for (int intervals = 1; intervals <= regionCount; intervals++)
                        {
                            left = right + rnd.Next(minGap, maxGap);
                            right = left + rnd.Next(minLenght, maxLenght);

                            di3.Add(new LightPeak()
                            {
                                left = left,
                                right = right,
                                metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                            }, sample, intervals);

                            //Console.Write("\r#Inserted intervals : {0:N0}", intervals);
                        }

                        stopWatch.Stop();
                        //Console.WriteLine("");
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine("");

                        writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2).ToString() + "\t" + stopWatch.Elapsed.ToString() + "\t" + stopWatch.ElapsedMilliseconds.ToString());
                        writer.Flush();
                    }
                }

                if (mode == Mode.MultiPass)
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(options))
                    {
                        Console.WriteLine(".::.     SECOND PASS    .::.");
                        stopWatch.Restart();
                        int TESTBlockCount = di3.SecondPass();
                        stopWatch.Stop();
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(TESTBlockCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine(".::. Total of {0:N0} blocks processed in {1}", TESTBlockCount, stopWatch.Elapsed.ToString());
                    }
            }
        }
    }
}
