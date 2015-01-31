using System;
using System.IO;
using System.Diagnostics;
using DI3;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using Di3BCLI;
using System.Collections.Generic;

namespace IndexSpeedTest
{
    public class IndexSpeedTest_v2
    {
        public string path = @"D:\VahidTest";//Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
        private StreamWriter writer { set; get; }
        private Stopwatch stopWatch = new Stopwatch();
        Random rnd = new Random();
        private Int32Comparer int32Comparer = new Int32Comparer();

        private void Run(
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

            TimeSpan totalET = new TimeSpan();

            if (disposeDi3atEachSample)
            {
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(options))
                    {
                        List<LightPeak> peaks = new List<LightPeak>();

                        for (int intervals = 1; intervals <= regionCount; intervals++)
                        {
                            left = right + rnd.Next(minGap, maxGap);
                            right = left + rnd.Next(minLenght, maxLenght);

                            peaks.Add(new LightPeak()
                            {
                                left = left,
                                right = right,
                                hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000)
                            });

                            //Console.Write("\r#Inserted _intervals : {0:N0}", _intervals);
                        }

                        stopWatch.Restart();
                        di3.Add(peaks, mode);
                        stopWatch.Stop();
                        //Console.WriteLine("");
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine("");

                        totalET += stopWatch.Elapsed;

                        writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2).ToString() + "\t" + stopWatch.Elapsed.ToString() + "\t" + stopWatch.ElapsedMilliseconds.ToString());
                        writer.Flush();
                    }
                }

                writer.WriteLine("Total_Elapsed_Time=" + totalET.ToString());
                writer.Flush();

                if (mode == Mode.MultiPass)
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(options))
                    {
                        Console.WriteLine(".::.     SECOND PASS    .::.");
                        stopWatch.Restart();
                        int blockCount = di3.SecondPass();
                        stopWatch.Stop();
                        Console.WriteLine(".::. Writting Speed : {0} _intervals\\sec", Math.Round(blockCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine(".::. Total of {0:N0} blocks processed in {1}", blockCount, stopWatch.Elapsed.ToString());

                        writer.WriteLine("_____Second__pass_____");
                        writer.WriteLine("totalBlockCount=" + blockCount);
                        writer.WriteLine("speed=" + Math.Round(blockCount / stopWatch.Elapsed.TotalSeconds, 2) + "block\\sec");
                        writer.WriteLine("ET=" + stopWatch.Elapsed.ToString());
                        writer.Flush();
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
                                //hashKey = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                                hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000)
                            });

                            //Console.Write("\r#Inserted _intervals : {0:N0}", _intervals);
                        }

                        stopWatch.Stop();
                        //Console.WriteLine("");
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine("");

                        totalET += stopWatch.Elapsed;

                        writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2).ToString() + "\t" + stopWatch.Elapsed.ToString() + "\t" + stopWatch.ElapsedMilliseconds.ToString());
                        writer.Flush();
                    }
                }

                writer.WriteLine("Total_Elapsed_Time=" + totalET.ToString());
                writer.Flush();

                if (mode == Mode.MultiPass)
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(options))
                    {
                        Console.WriteLine(".::.     SECOND PASS    .::.");
                        stopWatch.Restart();
                        int blockCount = di3.SecondPass();
                        stopWatch.Stop();
                        Console.WriteLine(".::. Writting Speed : {0} _intervals\\sec", Math.Round(blockCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine(".::. Total of {0:N0} blocks processed in {1}", blockCount, stopWatch.Elapsed.ToString());

                        writer.WriteLine("_____Second__pass_____");
                        writer.WriteLine("totalBlockCount=" + blockCount);
                        writer.WriteLine("speed=" + Math.Round(blockCount / stopWatch.Elapsed.TotalSeconds, 2) + "block\\sec");
                        writer.WriteLine("ET=" + stopWatch.Elapsed.ToString());
                        writer.Flush();
                    }
            }
        }


        private void RunSequence(
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
                        List<LightPeak> peaks = new List<LightPeak>();

                        for (int intervals = 1; intervals <= regionCount; intervals = intervals + 3)
                        {
                            left = (sample * regionCount) + intervals;
                            right = (sample * regionCount) + intervals + 1;

                            peaks.Add(new LightPeak()
                            {
                                left = left,
                                right = right,
                                //hashKey = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                                hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000)
                            });

                            //Console.Write("\r#Inserted _intervals : {0:N0}", _intervals);
                        }

                        stopWatch.Restart();
                        di3.Add(peaks, mode);
                        stopWatch.Stop();
                        //Console.WriteLine("");
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round((regionCount / 3) / stopWatch.Elapsed.TotalSeconds, 2));
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
                        di3.SecondPass();
                        stopWatch.Stop();
                        //Console.WriteLine(".::. Writting Speed : {0} _intervals\\sec", Math.Round(TESTBlockCount / stopWatch.Elapsed.TotalSeconds, 2));
                        //Console.WriteLine(".::. Total of {0:N0} blocks processed in {1}", TESTBlockCount, stopWatch.Elapsed.ToString());
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
                                //hashKey = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                                hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000)
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

                if (mode == Mode.MultiPass)
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(options))
                    {
                        Console.WriteLine(".::.     SECOND PASS    .::.");
                        stopWatch.Restart();
                        di3.SecondPass();
                        stopWatch.Stop();
                        //Console.WriteLine(".::. Writting Speed : {0} _intervals\\sec", Math.Round(TESTBlockCount / stopWatch.Elapsed.TotalSeconds, 2));
                        //Console.WriteLine(".::. Total of {0:N0} blocks processed in {1}", TESTBlockCount, stopWatch.Elapsed.ToString());
                    }
            }
        }





        public void Test_01()
        {
            string testTitle = "Test_01";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.MinimumChildNodes = 2;
            options.MaximumChildNodes = 256;
            options.MinimumValueNodes = 2;
            options.MaximumValueNodes = 256;

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_02()
        {
            string testTitle = "Test_02";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.MinimumChildNodes = 12;
            options.MaximumChildNodes = 32;
            options.MinimumValueNodes = 12;
            options.MaximumValueNodes = 32;

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_03()
        {
            string testTitle = "Test_03";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.MinimumChildNodes = 2;
            options.MaximumChildNodes = 256;
            options.MinimumValueNodes = 2;
            options.MaximumValueNodes = 256;

            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_04()
        {
            string testTitle = "Test_04";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.MinimumChildNodes = 2;
            options.MaximumChildNodes = 256;
            options.MinimumValueNodes = 2;
            options.MaximumValueNodes = 256;

            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_05()
        {
            string testTitle = "Test_05";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_06()
        {
            string testTitle = "Test_06";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 1024;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_07()
        {
            string testTitle = "Test_07";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_08()
        {
            string testTitle = "Test_08";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_09()
        {
            string testTitle = "Test_09";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 196;
            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_10()
        {
            string testTitle = "Test_10";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 196;
            options.FileBlockSize = 1024;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_11()
        {
            string testTitle = "Test_11";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 196;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_12()
        {
            string testTitle = "Test_12";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 196;
            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_13()
        {
            string testTitle = "Test_13";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_14()
        {
            string testTitle = "Test_14";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 1024;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_15()
        {
            string testTitle = "Test_15";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_16()
        {
            string testTitle = "Test_16";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_17()
        {
            string testTitle = "Test_17";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 196;
            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_18()
        {
            string testTitle = "Test_18";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 196;
            options.FileBlockSize = 1024;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_19()
        {
            string testTitle = "Test_19";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 196;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_20()
        {
            string testTitle = "Test_20";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 96;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_21()
        {
            string testTitle = "Test_21";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            //options.AverageKeySize = 4;
            //options.AverageValueSize = 96;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            options.MaximumChildNodes = 256;
            options.MinimumChildNodes = 2;

            options.MaximumValueNodes = 256;
            options.MinimumValueNodes = 2;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_22()
        {
            string testTitle = "Test_22";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.LogFileInCache;

            options.Locking = LockMode.SimpleReadWriteLocking;


            Run(
                100,
                200000,
                false,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_23()
        {
            string testTitle = "Test_23";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.WriterOnlyLocking;

            options.MaximumChildNodes = 256;
            options.MinimumChildNodes = 2;

            options.MaximumValueNodes = 256;
            options.MinimumValueNodes = 2;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_24()
        {
            string testTitle = "Test_24";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.WriterOnlyLocking;

            options.MaximumChildNodes = 256;
            options.MinimumChildNodes = 4;

            options.MaximumValueNodes = 256;
            options.MinimumValueNodes = 4;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_25()
        {
            string testTitle = "Test_25";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.WriterOnlyLocking;

            options.MaximumChildNodes = 256;
            options.MinimumChildNodes = 16;

            options.MaximumValueNodes = 256;
            options.MinimumValueNodes = 16;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_26()
        {
            string testTitle = "Test_26";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.WriterOnlyLocking;

            options.MaximumChildNodes = 256;
            options.MinimumChildNodes = 32;

            options.MaximumValueNodes = 256;
            options.MinimumValueNodes = 32;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_27()
        {
            string testTitle = "Test_27";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            options.AverageKeySize = 4;
            options.AverageValueSize = 64;

            Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_28()
        {
            string testTitle = "Test_28";

            Di3Options<int> options = new Di3Options<int>(
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer,
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx");

            options.MinimumChildNodes = 2;
            options.MaximumChildNodes = 256;
            options.MinimumValueNodes = 2;
            options.MaximumValueNodes = 256;

            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            Run(
                800,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.MultiPass);
        }
    }
}
