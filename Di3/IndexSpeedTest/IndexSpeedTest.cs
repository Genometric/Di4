using System;
using System.IO;
using System.Diagnostics;
using DI3;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using Di3BMain;

namespace IndexSpeedTest
{
    public class IndexSpeedTest
    {
        Random rnd = new Random();

        private int sampleCount { set; get; }
        private int regionCount { set; get; }
        private string outputPath { set; get; }
        private int minGap { set; get; }
        private int maxGap { set; get; }
        private int minLenght { set; get; }
        private int maxLenght { set; get; }
        private Stopwatch stopWatch { set; get; }
        private StreamWriter writer { set; get; }


        public void Run(
            int SampleCount,
            int RegionCount,
            bool disposeDi3atEachSample,
            string OutputPath,
            string TestName,
            int MinGap,
            int MaxGap,
            int MinLenght,
            int MaxLenght)
        {
            int right = 0;
            int left = 0;

            regionCount = RegionCount;
            sampleCount = SampleCount;
            outputPath = OutputPath;
            minGap = MinGap;
            maxGap = MaxGap;
            minLenght = MinLenght;
            maxLenght = MaxLenght;

            if (!Directory.Exists(outputPath) && outputPath.Trim() != string.Empty) Directory.CreateDirectory(outputPath);
            //StreamWriter writer = new StreamWriter(@"E:\VahidsTest\speed.txt");
            writer = new StreamWriter(outputPath + Path.DirectorySeparatorChar + "speed" + TestName + ".txt");
            writer.WriteLine("Di3 indexing speed test: " + TestName);

            Stopwatch stopWatch = new Stopwatch();
            if (!Directory.Exists(OutputPath)) Directory.CreateDirectory(OutputPath);

            string file = OutputPath + "\\bplusTree.bpt";

            if (disposeDi3atEachSample)
            {
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);

                    /// Why am I diconstructing bplustree at each iteration ? 
                    /// becasue in actual scenario there is a taxanomy and data between taxanomies are independent and 
                    /// should be in different trees. Hence I need to close the BPlusTrees at every taxonomy. 
                    using (var di3 = new Di3<int, Peak, PeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer))
                    {
                        stopWatch.Restart();

                        for (int intervals = 1; intervals <= regionCount; intervals++)
                        {
                            left = right + rnd.Next(MinGap, MaxGap);
                            right = left + rnd.Next(MinLenght, MaxLenght);

                            di3.Add(new Peak()
                            {
                                left = left,
                                right = right,
                                metadata = new PeakData()
                                {
                                    left = left,
                                    right = right,
                                    name = RandomName(),
                                    value = rnd.NextDouble(),
                                    hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                                }
                            });

                            Console.Write("\r#Inserted intervals : {0:N0}", intervals);
                        }

                        stopWatch.Stop();
                        Console.WriteLine("");
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine("");

                        writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        writer.Flush();
                    }
                }
            }
            else
            {
                file = outputPath + Path.DirectorySeparatorChar + "bplusTree.bpt";
                var di3 = new Di3<int, Peak, PeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer);

                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);

                    stopWatch.Restart();

                    for (int intervals = 1; intervals <= regionCount; intervals++)
                    {
                        left = right + rnd.Next(MinGap, MaxGap);
                        right = left + rnd.Next(MinLenght, MaxLenght);

                        di3.Add(new Peak()
                        {
                            left = left,
                            right = right,
                            metadata = new PeakData()
                            {
                                left = left,
                                right = right,
                                name = RandomName(),
                                value = rnd.NextDouble(),
                                hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                            }
                        });

                        Console.Write("\r#Inserted intervals : {0:N0}", intervals);
                    }

                    stopWatch.Stop();
                    Console.WriteLine("");
                    Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                    Console.WriteLine("");

                    writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                    writer.Flush();
                }
            }
        }

        public void Run(
            int SampleCount,
            int RegionCount,
            bool disposeDi3atEachSample,
            string OutputPath,
            string TestName,
            int MinGap,
            int MaxGap,
            int MinLenght,
            int MaxLenght,
            int avgKeySize,
            int avgValueSize)
        {
            int right = 0;
            int left = 0;

            sampleCount = SampleCount;
            regionCount = RegionCount;
            outputPath = OutputPath;
            minGap = MinGap;
            maxGap = MaxGap;
            minLenght = MinLenght;
            maxLenght = MaxLenght;

            if (!Directory.Exists(outputPath) && outputPath.Trim() != string.Empty) Directory.CreateDirectory(outputPath);

            stopWatch = new Stopwatch();
            writer = new StreamWriter(outputPath + Path.DirectorySeparatorChar + "speed" + TestName + ".txt");
            writer.WriteLine("Di3 indexing speed test: " + TestName);

            if (disposeDi3atEachSample)
            {
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);
                    string file = outputPath + Path.DirectorySeparatorChar + "bplusTree.bpt";

                    /// Why am I diconstructing bplustree at each iteration ? 
                    /// becasue in actual scenario there is a taxanomy and data between taxanomies are independent and 
                    /// should be in different trees. Hence I need to close the BPlusTrees at every taxonomy. 
                    using (var di3 = new Di3<int, Peak, PeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer, avgKeySize, avgValueSize))
                    {
                        stopWatch.Restart();

                        for (int intervals = 1; intervals <= regionCount; intervals++)
                        {
                            left = right + rnd.Next(MinGap, MaxGap);
                            right = left + rnd.Next(MinLenght, MaxLenght);

                            di3.Add(new Peak()
                            {
                                left = left,
                                right = right,
                                metadata = new PeakData()
                                {
                                    left = left,
                                    right = right,
                                    name = RandomName(),
                                    value = rnd.NextDouble(),
                                    hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                                }
                            });

                            Console.Write("\r#Inserted intervals : {0:N0}", intervals);
                        }
                    }
                }
            }
            else
            {
                string file = outputPath + Path.DirectorySeparatorChar + "bplusTree.bpt";
                var di3 = new Di3<int, Peak, PeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer, avgKeySize, avgValueSize);

                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);
                    stopWatch.Restart();

                    for (int intervals = 1; intervals <= regionCount; intervals++)
                    {
                        left = right + rnd.Next(MinGap, MaxGap);
                        right = left + rnd.Next(MinLenght, MaxLenght);

                        di3.Add(new Peak()
                        {
                            left = left,
                            right = right,
                            metadata = new PeakData()
                            {
                                left = left,
                                right = right,
                                name = RandomName(),
                                value = rnd.NextDouble(),
                                hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                            }
                        });

                        Console.Write("\r#Inserted intervals : {0:N0}", intervals);
                    }
                }
            }
        }

        public void Run(
            int SampleCount,
            int RegionCount,
            bool disposeDi3atEachSample,
            string OutputPath,
            string TestName,
            int MinGap,
            int MaxGap,
            int MinLenght,
            int MaxLenght,
            int MinChildNodes,
            int MaxChildNodes,
            int MinValueNodes,
            int MaxValueNodes)
        {
            int right = 0;
            int left = 0;

            sampleCount = SampleCount;
            regionCount = RegionCount;
            outputPath = OutputPath;
            minGap = MinGap;
            maxGap = MaxGap;
            minLenght = MinLenght;
            maxLenght = MaxLenght;

            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

            stopWatch = new Stopwatch();
            writer = new StreamWriter(outputPath + Path.DirectorySeparatorChar + "speed" + TestName + ".txt");
            writer.WriteLine("Di3 indexing speed test: " + TestName);

            if (disposeDi3atEachSample)
            {
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);
                    string file = outputPath + Path.DirectorySeparatorChar + "bplusTree.bpt";

                    /// Why am I diconstructing bplustree at each iteration ? 
                    /// becasue in actual scenario there is a taxanomy and data between taxanomies are independent and 
                    /// should be in different trees. Hence I need to close the BPlusTrees at every taxonomy. 
                    using (var di3 = new Di3<int, Peak, PeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer,
                        MaxChildNodes, MinChildNodes, MaxValueNodes, MinValueNodes))
                    {
                        stopWatch.Restart();

                        for (int intervals = 1; intervals <= regionCount; intervals++)
                        {
                            left = right + rnd.Next(MinGap, MaxGap);
                            right = left + rnd.Next(MinLenght, MaxLenght);

                            di3.Add(new Peak()
                            {
                                left = left,
                                right = right,
                                metadata = new PeakData()
                                {
                                    left = left,
                                    right = right,
                                    name = RandomName(),
                                    value = rnd.NextDouble(),
                                    hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                                }
                            });

                            Console.Write("\r#Inserted intervals : {0:N0}", intervals);
                        }
                    }
                }
            }
            else
            {
                string file = outputPath + Path.DirectorySeparatorChar + "bplusTree.bpt";
                var di3 = new Di3<int, Peak, PeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer,
                        MaxChildNodes, MinChildNodes, MaxValueNodes, MinValueNodes);
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);
                    stopWatch.Restart();

                    for (int intervals = 1; intervals <= regionCount; intervals++)
                    {
                        left = right + rnd.Next(MinGap, MaxGap);
                        right = left + rnd.Next(MinLenght, MaxLenght);

                        di3.Add(new Peak()
                        {
                            left = left,
                            right = right,
                            metadata = new PeakData()
                            {
                                left = left,
                                right = right,
                                name = RandomName(),
                                value = rnd.NextDouble(),
                                hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                            }
                        });

                        Console.Write("\r#Inserted intervals : {0:N0}", intervals);
                    }
                }
            }
        }

        private void Generate_and_Add_Regions(Di3<int, Peak, PeakData> di3)
        {
            int right = 0;
            int left = 0; 

            stopWatch.Restart();

            for (int intervals = 1; intervals <= regionCount; intervals++)
            {
                left = right + rnd.Next(minGap, maxGap);
                right = left + rnd.Next(minLenght, maxLenght);

                di3.Add(new Peak()
                {
                    left = left,
                    right = right,
                    metadata = new PeakData()
                    {
                        left = left,
                        right = right,
                        name = RandomName(),
                        value = rnd.NextDouble(),
                        hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                    }
                });

                Console.Write("\r#Inserted intervals : {0:N0}", intervals);
            }

            stopWatch.Stop();
            Console.WriteLine("");
            Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
            Console.WriteLine("");

            writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
            writer.Flush();
        }

        public string RandomName()
        {
            int lenght = rnd.Next(20, 100);
            string rtv = string.Empty;
            while (--lenght >= 0)
                rtv += (char)rnd.Next(48, 90);
            return rtv;
        }

        Int32Comparer int32Comparer = new Int32Comparer();
    }
}
