using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Di3BMain;
using System.Diagnostics;

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

            options.CalcBTreeOrder(16, 1400); //24);
            //options.CreateFile = createPolicy;
            options.ExistingLogAction = ExistingLogAction.ReplayAndCommit;
            options.StoragePerformance = StoragePerformance.Fastest;

            options.CachePolicy = CachePolicy.All;

            options.CreateFile = CreatePolicy.Always;
            options.FileName = @"D:\testtesttest.idx";

            options.FileBlockSize = 512;

            var di3 = new BPlusTree<int, B>(options);

            

            // basic tests
            Stopwatch stpwtch = new Stopwatch();
            stpwtch.Restart();
            for (int i = 0; i < 200000; i++)
                di3.Add(i, new B('L', 100));
            stpwtch.Stop();
            var et = stpwtch.Elapsed;
        }
    }
}
