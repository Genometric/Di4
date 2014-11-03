using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPlusTreeTest
{
    static class LargeInsertion
    {
        public static void Sequence(string outputPath, string TestName)
        {
            int regionCount = 200000;
            int sampleCount = 300;
            Random rnd = new Random();
            Stopwatch stopWatch = new Stopwatch();
            if (!Directory.Exists(outputPath + Path.DirectorySeparatorChar))
                Directory.CreateDirectory(outputPath + Path.DirectorySeparatorChar);
            StreamWriter writer = new StreamWriter(outputPath + Path.DirectorySeparatorChar + "speed" + TestName + ".txt");
            writer.WriteLine("Di3 indexing speed test: " + TestName);
            writer.WriteLine("Speed(interval/sec)\tET\tET(ms)");


            BPlusTree<int, int>.OptionsV2 options = new BPlusTree<int, int>.OptionsV2(PrimitiveSerializer.Int32, PrimitiveSerializer.Int32);
            options.CreateFile = CreatePolicy.Always;
            options.FileName = outputPath + Path.DirectorySeparatorChar + "speed" + TestName + ".idx";


            LargeInsertionAddUpdateValue update = new LargeInsertionAddUpdateValue();

            using (var tree = new BPlusTree<int, int>(options))
            {
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);
                    stopWatch.Restart();
                    for (int region = 0; region < regionCount; region++)
                    {
                        update.Value = rnd.Next(0, Int32.MaxValue);
                        tree.AddOrUpdate(region, ref update);
                    }
                    stopWatch.Stop();
                    Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                    Console.WriteLine("");
                    writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2).ToString() + "\t" + stopWatch.Elapsed.ToString() + "\t" + stopWatch.ElapsedMilliseconds.ToString());
                    writer.Flush();
                }
            }
        }
        public static void RandomOverlaps(string outputPath, string TestName)
        {
            int regionCount = 200000;
            int sampleCount = 300;
            Random rnd = new Random();
            Stopwatch stopWatch = new Stopwatch();
            if (!Directory.Exists(outputPath + Path.DirectorySeparatorChar))
                Directory.CreateDirectory(outputPath + Path.DirectorySeparatorChar);
            StreamWriter writer = new StreamWriter(outputPath + Path.DirectorySeparatorChar + "speed" + TestName + ".txt");
            writer.WriteLine("Di3 indexing speed test: " + TestName);
            writer.WriteLine("Speed(interval/sec)\tET\tET(ms)");


            BPlusTree<int, int>.OptionsV2 options = new BPlusTree<int, int>.OptionsV2(PrimitiveSerializer.Int32, PrimitiveSerializer.Int32);
            options.CreateFile = CreatePolicy.Always;
            options.FileName = outputPath + Path.DirectorySeparatorChar + "speed" + TestName + ".idx";


            LargeInsertionAddUpdateValue update = new LargeInsertionAddUpdateValue();

            using (var tree = new BPlusTree<int, int>(options))
            {
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Console.WriteLine("processing sample   : {0:N0}", sample);
                    stopWatch.Restart();
                    for (int region = 0; region < regionCount; region++)
                    {
                        update.Value = rnd.Next(0, Int32.MaxValue);
                        tree.AddOrUpdate(rnd.Next(10, 12), ref update);
                    }
                    stopWatch.Stop();
                    Console.WriteLine(".::. Writting Speed : {0} intervals\\sec", Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2));
                    Console.WriteLine("");
                    writer.WriteLine(Math.Round(regionCount / stopWatch.Elapsed.TotalSeconds, 2).ToString() + "\t" + stopWatch.Elapsed.ToString() + "\t" + stopWatch.ElapsedMilliseconds.ToString());
                    writer.Flush();
                }
            }
        }
    }

    struct LargeInsertionAddUpdateValue : ICreateOrUpdateValue<int, int>, IRemoveValue<int, int>
    {
        public int OldValue;
        public int Value;
        public bool CreateValue(int key, out int value)
        {
            OldValue = 0;
            value = Value;
            return Value != 0;
        }
        public bool UpdateValue(int key, ref int value)
        {
            OldValue = value;
            value = Value;
            return Value != 0;
        }
        public bool RemoveValue(int key, int value)
        {
            OldValue = value;
            return value == Value;
        }
    }
}
