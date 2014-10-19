﻿using System;
using System.IO;
using System.Diagnostics;
using DI3;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using Di3BMain;
using System.Collections.Generic;
using Di3B;

namespace MapSpeedTest
{
    class MapSpeedTest
    {
        Int32Comparer int32Comparer = new Int32Comparer();

        public void Run(string IndexFile,
            int RegionCount,
            /*int MinGap,
            int MaxGap,*/
            int MinLenght,
            int MaxLenght,
            int avgKeySize,
            int avgValueSize)
        {
            List<LightPeak> Peaks = new List<LightPeak>();
            Random rnd = new Random();
            int rndLeft = 0;
            int rndRight = 0;
            Stopwatch stopWatch = new Stopwatch();

            using (var di3 = new Di3<int, LightPeak, LightPeakData>(IndexFile, CreatePolicy.IfNeeded, PrimitiveSerializer.Int32, int32Comparer, avgKeySize, avgValueSize))
            {
                /// Creating some random regions. 
                Console.WriteLine("");
                Console.Write("Creating Random Files ...  ");
                for (int r = 0; r < RegionCount; r++)
                {
                    rndLeft = rnd.Next(0, 1000000);
                    rndRight = rndLeft + rnd.Next(MinLenght, MaxLenght);

                    Peaks.Add(new LightPeak() { left = rndLeft, right = rndRight, metadata = new LightPeakData() { hashKey = (uint)rnd.Next(1, 1000000) } });
                }
                Console.Write("Done!");
                Console.WriteLine("");

                stopWatch.Restart();
                FunctionOutput<Output<int, LightPeak, LightPeakData>> output = new FunctionOutput<Output<int, LightPeak, LightPeakData>>();
                AggregateFactory<int, LightPeak, LightPeakData> aggFactory = new AggregateFactory<int, LightPeak, LightPeakData>();
                /*output.Chrs[reference.Key][strand] =*/
                di3.Map<Output<int, LightPeak, LightPeakData>>(aggFactory.GetAggregateFunction("count"), Peaks);
                stopWatch.Stop();
                Console.WriteLine("");
                Console.WriteLine("ET: {0}", stopWatch.Elapsed);


                int stopHere = 0;
            }
        }
    }
}
