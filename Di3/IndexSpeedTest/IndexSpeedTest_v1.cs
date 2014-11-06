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
    public class IndexSpeedTest_v1
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


        public void Run(string OutputPath, string TestName)
        {
            outputPath = OutputPath;
            if (!Directory.Exists(outputPath) && outputPath.Trim() != string.Empty) Directory.CreateDirectory(outputPath);

            if (!Directory.Exists(OutputPath)) Directory.CreateDirectory(OutputPath);

            string file = OutputPath + "\\bplusTree.bpt";

            /// Why am I diconstructing bplustree at each iteration ? 
            /// becasue in actual scenario there is a taxanomy and data between taxanomies are independent and 
            /// should be in different trees. Hence I need to close the BPlusTrees at every taxonomy. 
            using (var di3 = new Di3<int, LightPeak, LightPeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer))
            {
                //_di3.Add(new LightPeak() { left = 010, right = 050, hashKey = new LightPeakData() { hashKey = 0/*(UInt32)Math.Round(rnd.NextDouble() * 100000)*/ } }, 1, 1);
                //_di3.Add(new LightPeak() { left = 060, right = 100, hashKey = new LightPeakData() { hashKey = 1/*(UInt32)Math.Round(rnd.NextDouble() * 100000)*/ } }, 1, 1);
                //_di3.Add(new LightPeak() { left = 110, right = 140, hashKey = new LightPeakData() { hashKey = 2/*(UInt32)Math.Round(rnd.NextDouble() * 100000)*/ } }, 1, 1);
                //_di3.Add(new LightPeak() { left = 030, right = 055, hashKey = new LightPeakData() { hashKey = 3/*(UInt32)Math.Round(rnd.NextDouble() * 100000)*/ } }, 1, 1);
                //_di3.Add(new LightPeak() { left = 060, right = 080, hashKey = new LightPeakData() { hashKey = 4/*(UInt32)Math.Round(rnd.NextDouble() * 100000)*/ } }, 1, 1);
                //_di3.Add(new LightPeak() { left = 100, right = 110, hashKey = new LightPeakData() { hashKey = 5/*(UInt32)Math.Round(rnd.NextDouble() * 100000)*/ } }, 1, 1);
                //_di3.Add(new LightPeak() { left = 140, right = 180, hashKey = new LightPeakData() { hashKey = 6/*(UInt32)Math.Round(rnd.NextDouble() * 100000)*/ } }, 1, 1);

                di3.Add(new LightPeak() { left = 100, right = 300, metadata = new LightPeakData() { hashKey = 1 } }, 1, 1);
                di3.Add(new LightPeak() { left = 200, right = 400, metadata = new LightPeakData() { hashKey = 2 } }, 1, 1);
                di3.Add(new LightPeak() { left = 500, right = 600, metadata = new LightPeakData() { hashKey = 3 } }, 1, 1);
                di3.Add(new LightPeak() { left = 1000, right = 1100, metadata = new LightPeakData() { hashKey = 4 } }, 1, 1);
                di3.Add(new LightPeak() { left = 80, right = 450, metadata = new LightPeakData() { hashKey = 5 } }, 1, 1);
                di3.Add(new LightPeak() { left = 460, right = 700, metadata = new LightPeakData() { hashKey = 6 } }, 1, 1);
                di3.Add(new LightPeak() { left = 800, right = 900, metadata = new LightPeakData() { hashKey = 7 } }, 1, 1);
                di3.Add(new LightPeak() { left = 1200, right = 1300, metadata = new LightPeakData() { hashKey = 8 } }, 1, 1);
                di3.Add(new LightPeak() { left = 10, right = 30, metadata = new LightPeakData() { hashKey = 9 } }, 1, 1);
                di3.Add(new LightPeak() { left = 110, right = 180, metadata = new LightPeakData() { hashKey = 10 } }, 1, 1);
                di3.Add(new LightPeak() { left = 80, right = 100, metadata = new LightPeakData() { hashKey = 11 } }, 1, 1);
                di3.Add(new LightPeak() { left = 455, right = 458, metadata = new LightPeakData() { hashKey = 12 } }, 1, 1);
                di3.Add(new LightPeak() { left = 780, right = 920, metadata = new LightPeakData() { hashKey = 13 } }, 1, 1);
                di3.Add(new LightPeak() { left = 770, right = 930, metadata = new LightPeakData() { hashKey = 14 } }, 1, 1);
                di3.Add(new LightPeak() { left = 1201, right = 1299, metadata = new LightPeakData() { hashKey = 15 } }, 1, 1);
                di3.Add(new LightPeak() { left = 5, right = 10, metadata = new LightPeakData() { hashKey = 16 } }, 1, 1);

                di3.SecondPass();
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
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer))
                    {
                        stopWatch.Restart();

                        for (int intervals = 1; intervals <= regionCount; intervals++)
                        {
                            left = right + rnd.Next(MinGap, MaxGap);
                            right = left + rnd.Next(MinLenght, MaxLenght);

                            di3.Add(new LightPeak()
                            {
                                left = left,
                                right = right,
                                metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                            }, sample, intervals);
                            /*new Peak()
                        {
                            left = left,
                            right = right,
                            hashKey = new PeakData()
                            {
                                left = left,
                                right = right,
                                name = RandomName(),
                                currentValue = rnd.NextDouble(),
                                hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                            }
                        });*/

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
                var di3 = new Di3<int, LightPeak, LightPeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer);

                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);

                    stopWatch.Restart();

                    for (int intervals = 1; intervals <= regionCount; intervals++)
                    {
                        left = right + rnd.Next(MinGap, MaxGap);
                        right = left + rnd.Next(MinLenght, MaxLenght);

                        di3.Add(new LightPeak()
                        {
                            left = left,
                            right = right,
                            metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                        }, sample, intervals);
                        /*
                        _di3.Add(new Peak()
                        {
                            left = left,
                            right = right,
                            hashKey = new PeakData()
                            {
                                left = left,
                                right = right,
                                name = RandomName(),
                                currentValue = rnd.NextDouble(),
                                hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                            }
                        });*/

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


            var Lambda_Count_Writer = new StreamWriter(outputPath + Path.DirectorySeparatorChar + "LambdaSize" + TestName + ".txt");

            if (disposeDi3atEachSample)
            {
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);
                    string file = outputPath + Path.DirectorySeparatorChar + "bplusTree.bpt";

                    /// Why am I diconstructing bplustree at each iteration ? 
                    /// becasue in actual scenario there is a taxanomy and data between taxanomies are independent and 
                    /// should be in different trees. Hence I need to close the BPlusTrees at every taxonomy. 
                    //using (var _di3 = new Di3<int, LightPeak, LightPeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer, avgKeySize, avgValueSize))
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer, avgKeySize, avgValueSize))
                    {
                        int test_Maximum_Lambda_Count = 0;
                        stopWatch.Restart();

                        for (int intervals = 1; intervals <= regionCount; intervals++)
                        {
                            left = right + rnd.Next(MinGap, MaxGap);
                            right = left + rnd.Next(MinLenght, MaxLenght);


                            di3.Add(new LightPeak()
                            {
                                left = left,
                                right = right,
                                metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                            }, sample, intervals);

                            //test_Maximum_Lambda_Count = Math.Max(test_Maximum_Lambda_Count, couuuuuunt);

                            /*
                            _di3.Add(new Peak()
                            {
                                left = left,
                                right = right,
                                hashKey = new PeakData()
                                {
                                    left = left,
                                    right = right,
                                    name = RandomName(),
                                    currentValue = rnd.NextDouble(),
                                    hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                                }
                            });*/

                            Console.Write("\r#Inserted intervals : {0:N0}", intervals);
                        }

                        stopWatch.Stop();
                        Console.WriteLine("");
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine("");

                        writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        writer.Flush();


                        Lambda_Count_Writer.WriteLine(test_Maximum_Lambda_Count);
                        Lambda_Count_Writer.Flush();
                    }
                }
            }
            else
            {
                string file = outputPath + Path.DirectorySeparatorChar + "bplusTree.bpt";
                var di3 = new Di3<int, LightPeak, LightPeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer, avgKeySize, avgValueSize);

                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);
                    stopWatch.Restart();

                    for (int intervals = 1; intervals <= regionCount; intervals++)
                    {
                        left = right + rnd.Next(MinGap, MaxGap);
                        right = left + rnd.Next(MinLenght, MaxLenght);

                        di3.Add(new LightPeak()
                        {
                            left = left,
                            right = right,
                            metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                        }, sample, intervals);

                        /*
                        _di3.Add(new Peak()
                        {
                            left = left,
                            right = right,
                            hashKey = new PeakData()
                            {
                                left = left,
                                right = right,
                                name = RandomName(),
                                currentValue = rnd.NextDouble(),
                                hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                            }
                        });*/

                        Console.Write("\r#Inserted intervals : {0:N0}", intervals);
                    }

                    stopWatch.Stop();
                    Console.WriteLine("");
                    Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                    Console.WriteLine("");
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
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer,
                        MaxChildNodes, MinChildNodes, MaxValueNodes, MinValueNodes))
                    {
                        stopWatch.Restart();

                        for (int intervals = 1; intervals <= regionCount; intervals++)
                        {
                            left = right + rnd.Next(MinGap, MaxGap);
                            right = left + rnd.Next(MinLenght, MaxLenght);

                            di3.Add(new LightPeak()
                            {
                                left = left,
                                right = right,
                                metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                            }, sample, intervals);

                            /*
                            _di3.Add(new Peak()
                            {
                                left = left,
                                right = right,
                                hashKey = new PeakData()
                                {
                                    left = left,
                                    right = right,
                                    name = RandomName(),
                                    currentValue = rnd.NextDouble(),
                                    hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                                }
                            });*/

                            Console.Write("\r#Inserted intervals : {0:N0}", intervals);
                        }

                        stopWatch.Stop();
                        Console.WriteLine("");
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine("");
                    }
                }
            }
            else
            {
                string file = outputPath + Path.DirectorySeparatorChar + "bplusTree.bpt";
                var di3 = new Di3<int, LightPeak, LightPeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer,
                        MaxChildNodes, MinChildNodes, MaxValueNodes, MinValueNodes);

                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);
                    stopWatch.Restart();

                    for (int intervals = 1; intervals <= regionCount; intervals++)
                    {
                        left = right + rnd.Next(MinGap, MaxGap);
                        right = left + rnd.Next(MinLenght, MaxLenght);

                        di3.Add(new LightPeak()
                        {
                            left = left,
                            right = right,
                            metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                        }, sample, intervals);

                        /*
                        _di3.Add(new Peak()
                        {
                            left = left,
                            right = right,
                            hashKey = new PeakData()
                            {
                                left = left,
                                right = right,
                                name = RandomName(),
                                currentValue = rnd.NextDouble(),
                                hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                            }
                        });*/

                        Console.Write("\r#Inserted intervals : {0:N0}", intervals);
                    }

                    stopWatch.Stop();
                    Console.WriteLine("");
                    Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                    Console.WriteLine("");
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
            int avgValueSize,
            bool goMultiThread)
        {
            Mode mode = Mode.MultiPass;

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


            var Lambda_Count_Writer = new StreamWriter(outputPath + Path.DirectorySeparatorChar + "LambdaSize" + TestName + ".txt");

            if (disposeDi3atEachSample)
            {
                string file = outputPath + Path.DirectorySeparatorChar + "bplusTree.bpt";
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);

                    /// Why am I diconstructing bplustree at each iteration ? 
                    /// becasue in actual scenario there is a taxanomy and data between taxanomies are independent and 
                    /// should be in different trees. Hence I need to close the BPlusTrees at every taxonomy. 
                    //using (var _di3 = new Di3<int, LightPeak, LightPeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer, avgKeySize, avgValueSize))
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer, avgKeySize, avgValueSize))
                    {
                        //int test_Maximum_Lambda_Count = 0;
                        //int couuuuuunt = 0;
                        stopWatch.Restart();

                        List<LightPeak> peaks = new List<LightPeak>();

                        for (int intervals = 1; intervals <= regionCount; intervals++)
                        {
                            left = right + rnd.Next(MinGap, MaxGap);
                            right = left + rnd.Next(MinLenght, MaxLenght);

                            peaks.Add(new LightPeak()
                            {
                                left = left,
                                right = right,
                                metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                            });

                            //test_Maximum_Lambda_Count = Math.Max(test_Maximum_Lambda_Count, couuuuuunt);

                            /*
                            _di3.Add(new Peak()
                            {
                                left = left,
                                right = right,
                                hashKey = new PeakData()
                                {
                                    left = left,
                                    right = right,
                                    name = RandomName(),
                                    currentValue = rnd.NextDouble(),
                                    hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                                }
                            });*/

                            //Console.Write("\r#Inserted _intervals : {0:N0}", _intervals);
                        }

                        di3.Add(peaks, mode);

                        stopWatch.Stop();
                        Console.WriteLine("");
                        Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        Console.WriteLine("");

                        writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                        writer.Flush();


                        //Lambda_Count_Writer.WriteLine(test_Maximum_Lambda_Count);
                        //Lambda_Count_Writer.Flush();
                    }
                }

                if (mode == Mode.MultiPass)
                    using (var di3 = new Di3<int, LightPeak, LightPeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer, avgKeySize, avgValueSize))
                    {
                        Console.WriteLine("*********     SECOND PASS    **********");
                        stopWatch.Restart();
                        di3.SecondPass();
                        stopWatch.Stop();
                        //Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(TESTBlockCount / stopWatch.Elapsed.TotalSeconds, 2));
                        //Console.WriteLine(".::. Total of {0:N0} blocks processed in {1}", TESTBlockCount, stopWatch.Elapsed.ToString());
                    }
            }
            else
            {
                string file = outputPath + Path.DirectorySeparatorChar + "bplusTree.bpt";
                var di3 = new Di3<int, LightPeak, LightPeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer, avgKeySize, avgValueSize);

                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);
                    stopWatch.Restart();

                    for (int intervals = 1; intervals <= regionCount; intervals++)
                    {
                        left = right + rnd.Next(MinGap, MaxGap);
                        right = left + rnd.Next(MinLenght, MaxLenght);

                        di3.Add(new LightPeak()
                        {
                            left = left,
                            right = right,
                            metadata = new LightPeakData() { hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) }
                        }, sample, intervals);

                        /*
                        _di3.Add(new Peak()
                        {
                            left = left,
                            right = right,
                            hashKey = new PeakData()
                            {
                                left = left,
                                right = right,
                                name = RandomName(),
                                currentValue = rnd.NextDouble(),
                                hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                            }
                        });*/

                        Console.Write("\r#Inserted intervals : {0:N0}", intervals);
                    }

                    stopWatch.Stop();
                    Console.WriteLine("");
                    Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                    Console.WriteLine("");
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
                /*
                _di3.Add(new Peak()
                {
                    left = left,
                    right = right,
                    hashKey = new PeakData()
                    {
                        left = left,
                        right = right,
                        name = RandomName(),
                        currentValue = rnd.NextDouble(),
                        hashKey = (UInt32)Math.Round(rnd.NextDouble() * 100000) // we won't use hashkey in this test, hence lets consider this "correct"
                    }
                });*/

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
