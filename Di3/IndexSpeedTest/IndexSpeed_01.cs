using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using DI3;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using Di3BMain;

namespace IndexSpeedTest
{
    public class IndexSpeed_01
    {
        Random rnd = new Random();

        public void Run()
        {
            int right = 0;
            int left = 0;

            const int regionCount = 200000;

            StreamWriter writer = new StreamWriter(@"E:\VahidsTest\speed.txt");

            Stopwatch stopWatch = new Stopwatch();
            string path = @"E:\VahidsTest";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            for (int sample = 0; sample < 2000; sample++)
            {
                Console.WriteLine("processing sample   : {0:N0}", sample);
                string file = path + "\\bplusTree.bpt";

                /// Why am I diconstructing bplustree at each iteration ? 
                /// becasue in actual scenario there is a taxanomy and data between taxanomies are independent and 
                /// should be in different trees. Hence I need to close the BPlusTrees at every taxonomy. 
                using (var di3 = new Di3<int, Peak, PeakData>(file, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer))
                {
                    stopWatch.Restart();

                    for (int intervals = 1; intervals <= regionCount; intervals++)
                    {
                        left = right + rnd.Next(5, 500);
                        right = left + rnd.Next(500, 1000);

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
