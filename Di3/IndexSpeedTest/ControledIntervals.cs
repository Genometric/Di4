using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using Polimi.DEIB.VahidJalili.DI3;
using Polimi.DEIB.VahidJalili.DI3.CLI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace IndexSpeedTest
{
    public class ControledIntervals
    {
        private StreamWriter writer { set; get; }
        private Stopwatch stopWatch = new Stopwatch();
        Random rnd = new Random();
        private Int32Comparer int32Comparer = new Int32Comparer();
        public string path = @"D:\VahidTest";


        public void Sequence()
        {
            string TestTitle = "ControlledIntervals__Sequence";
            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + TestTitle);

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            _Sequence(
                400,
                200000,
                true,
                path,
                TestTitle,
                options,
                IndexingMode.SinglePass);
        }
        private void _Sequence(int sampleCount,
            int regionCount,
            bool disposeDi3atEachSample,
            string outputPath,
            string TestName,
            Di3Options<int> options,
            IndexingMode mode)
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
                        List<LightPeak> peaks = new List<LightPeak>();

                        for (int intervals = 1; intervals <= regionCount * 2; intervals = intervals + 2)
                        {
                            left = (sample * regionCount) + intervals;
                            right = (sample * regionCount) + intervals + 1;

                            peaks.Add(new LightPeak()
                            {
                                left = left,
                                right = right,
                                metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                            });

                            //Console.Write("\r#Inserted _intervals : {0:N0}", _intervals);
                        }

                        stopWatch.Restart();
                        di3.Add(peaks, mode);
                        stopWatch.Stop();
                        //Console.WriteLine("");
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine("");

                        writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2).ToString() + "\t" + stopWatch.Elapsed.ToString() + "\t" + stopWatch.ElapsedMilliseconds.ToString());
                        writer.Flush();
                    }
                }

                if (mode == IndexingMode.MultiPass)
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(options))
                    {
                        Console.WriteLine(".::.     SECOND PASS    .::.");
                        stopWatch.Restart();
                        di3.SecondPass();
                        stopWatch.Stop();
                        //Console.WriteLine(".::. Writting Speed : {0} _intervals\\sec", Math.Round(TESTBlockCount / _stopWatch.Elapsed.TotalSeconds, 2));
                        //Console.WriteLine(".::. Total of {0:N0} blocks processed in {1}", TESTBlockCount, _stopWatch.Elapsed.ToString());
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
                            left = (sample * regionCount) + intervals;
                            right = (sample * regionCount) + intervals + 1;

                            di3.Add(new LightPeak()
                            {
                                left = left,
                                right = right,
                                metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                            });

                            //Console.Write("\r#Inserted _intervals : {0:N0}", _intervals);
                        }

                        stopWatch.Stop();
                        //Console.WriteLine("");
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine("");

                        writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2).ToString() + "\t" + stopWatch.Elapsed.ToString() + "\t" + stopWatch.ElapsedMilliseconds.ToString());
                        writer.Flush();
                    }
                }

                if (mode == IndexingMode.MultiPass)
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(options))
                    {
                        Console.WriteLine(".::.     SECOND PASS    .::.");
                        stopWatch.Restart();
                        di3.SecondPass();
                        stopWatch.Stop();
                        //Console.WriteLine(".::. Writting Speed : {0} _intervals\\sec", Math.Round(TESTBlockCount / _stopWatch.Elapsed.TotalSeconds, 2));
                        //Console.WriteLine(".::. Total of {0:N0} blocks processed in {1}", TESTBlockCount, _stopWatch.Elapsed.ToString());
                    }
            }
        }

        public void Controlled_10_05_10()
        {
            string TestTitle = "ControlledIntervals__10_05_10";
            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + TestTitle);

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            _Controlled_10_05_10(
                200000,
                true,
                path,
                TestTitle,
                options,
                IndexingMode.SinglePass);
        }
        private void _Controlled_10_05_10(
            int regionCount,
            bool disposeDi3atEachSample,
            string outputPath,
            string TestName,
            Di3Options<int> options,
            IndexingMode mode)
        {
            int right = 0;
            int left = 0;

            if (!Directory.Exists(outputPath) && outputPath.Trim() != string.Empty) Directory.CreateDirectory(outputPath);

            writer = new StreamWriter(outputPath + Path.DirectorySeparatorChar + "speed" + TestName + ".txt");
            writer.WriteLine("Di3 indexing speed test: " + TestName);
            writer.WriteLine("Speed(interval/sec)\tET\tET(ms)");

            if (disposeDi3atEachSample)
            {
                for (int sample = 0; sample < 22; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);

                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(options))
                    {
                        List<LightPeak> peaks = new List<LightPeak>();

                        if (sample < 10 || sample > 13)
                        {
                            for (int intervals = 1; intervals <= regionCount * 2; intervals = intervals + 2)
                            {
                                left = (sample * regionCount) + intervals;
                                right = (sample * regionCount) + intervals + 1;

                                peaks.Add(new LightPeak()
                                {
                                    left = left,
                                    right = right,
                                    metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                                });

                                //Console.Write("\r#Inserted _intervals : {0:N0}", _intervals);
                            }
                        }
                        else if (sample == 10)
                        {
                            for (int intervals = 1; intervals < regionCount - 50; intervals++)
                                peaks.Add(new LightPeak()
                                {
                                    left = (sample * regionCount),
                                    right = (sample * regionCount) + 1,
                                    metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                                });

                            for (int intervals = 1; intervals < 50; intervals++)
                                peaks.Add(new LightPeak()
                                {
                                    left = (sample * regionCount),
                                    right = (sample * regionCount) + 1,
                                    metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                                });
                        }
                        else if (sample == 11)
                        {
                            for (int intervals = 1; intervals < regionCount - 500; intervals++)
                                peaks.Add(new LightPeak()
                                {
                                    left = (sample * regionCount),
                                    right = (sample * regionCount) + 1,
                                    metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                                });

                            for (int intervals = 1; intervals < 500; intervals++)
                                peaks.Add(new LightPeak()
                                {
                                    left = (sample * regionCount),
                                    right = (sample * regionCount) + 1,
                                    metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                                });
                        }
                        else if (sample == 12)
                        {
                            for (int intervals = 1; intervals < regionCount - 5000; intervals++)
                                peaks.Add(new LightPeak()
                                {
                                    left = (sample * regionCount),
                                    right = (sample * regionCount) + 1,
                                    metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                                });

                            for (int intervals = 1; intervals < 5000; intervals++)
                                peaks.Add(new LightPeak()
                                {
                                    left = (sample * regionCount),
                                    right = (sample * regionCount) + 1,
                                    metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                                });
                        }
                        else if (sample == 13)
                        {
                            for (int i = 0; i < 16; i++)
                                for (int intervals = 1; intervals < regionCount / 16; intervals++)
                                {
                                    peaks.Add(new LightPeak()
                                    {
                                        left = (sample * regionCount),
                                        right = (sample * regionCount) + 1,
                                        metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                                    });
                                }
                        }
                        

                        stopWatch.Restart();
                        di3.Add(peaks, mode);
                        stopWatch.Stop();
                        //Console.WriteLine("");
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine("");

                        writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2).ToString() + "\t" + stopWatch.Elapsed.ToString() + "\t" + stopWatch.ElapsedMilliseconds.ToString());
                        writer.Flush();
                    }
                }

                if (mode == IndexingMode.MultiPass)
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(options))
                    {
                        Console.WriteLine(".::.     SECOND PASS    .::.");
                        stopWatch.Restart();
                        di3.SecondPass();
                        stopWatch.Stop();
                        //Console.WriteLine(".::. Writting Speed : {0} _intervals\\sec", Math.Round(TESTBlockCount / _stopWatch.Elapsed.TotalSeconds, 2));
                        //Console.WriteLine(".::. Total of {0:N0} blocks processed in {1}", TESTBlockCount, _stopWatch.Elapsed.ToString());
                    }
            }
            else
            {
                using (var di3 = new Di3<int, LightPeak, LightPeakData>(options))
                {
                    for (int sample = 0; sample < 22; sample++)
                    {
                        Console.WriteLine("processing sample   : {0:N0}", sample);
                        stopWatch.Restart();

                        if (sample < 10 || sample > 11)
                        {
                            for (int intervals = 1; intervals <= regionCount * 2; intervals = intervals + 2)
                            {
                                left = (sample * regionCount) + intervals;
                                right = (sample * regionCount) + intervals + 1;

                                di3.Add(new LightPeak()
                                {
                                    left = left,
                                    right = right,
                                    metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                                });

                                //Console.Write("\r#Inserted _intervals : {0:N0}", _intervals);
                            }
                        }
                        else
                        {
                            for (int intervals = 1; intervals <= regionCount; intervals++)
                                di3.Add(new LightPeak()
                                {
                                    left = (sample * regionCount) + 10,
                                    right = (sample * regionCount) + 12,
                                    metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                                });
                        }

                        stopWatch.Stop();
                        //Console.WriteLine("");
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine("");

                        writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2).ToString() + "\t" + stopWatch.Elapsed.ToString() + "\t" + stopWatch.ElapsedMilliseconds.ToString());
                        writer.Flush();
                    }
                }

                if (mode == IndexingMode.MultiPass)
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(options))
                    {
                        Console.WriteLine(".::.     SECOND PASS    .::.");
                        stopWatch.Restart();
                        di3.SecondPass();
                        stopWatch.Stop();
                        //Console.WriteLine(".::. Writting Speed : {0} _intervals\\sec", Math.Round(TESTBlockCount / _stopWatch.Elapsed.TotalSeconds, 2));
                        //Console.WriteLine(".::. Total of {0:N0} blocks processed in {1}", TESTBlockCount, _stopWatch.Elapsed.ToString());
                    }
            }

        }
    }
}
