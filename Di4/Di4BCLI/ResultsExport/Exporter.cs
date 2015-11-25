using Polimi.DEIB.VahidJalili.DI4.DI4B;
using Polimi.DEIB.VahidJalili.DI4.DI4B.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Polimi.DEIB.VahidJalili.DI4.CLI
{
    internal static class Exporter
    {
        internal static ExecutionReport Export(string fileName, FunctionOutput<Output<int, Peak, PeakData>> results, string header, string separator = "\t")
        {
            int intervalCount = 0;
            Stopwatch stp = new Stopwatch();

            stp.Restart();
            if (!File.Exists(fileName)) File.Delete(fileName);
            using (File.Create(fileName)) { }
            using (var writter = new StreamWriter(fileName))
            {
                writter.WriteLine(header);
                foreach (var chr in results.Chrs)
                    foreach (var strand in chr.Value)
                        foreach (var interval in strand.Value)
                        {
                            writter.WriteLine(
                                chr.Key + separator +
                                interval.interval.left.ToString() + separator +
                                interval.interval.right.ToString() + separator +
                                interval.count.ToString() + separator +
                                strand.Key);
                            intervalCount++;
                        }
            }

            stp.Stop();
            return new ExecutionReport(intervalCount, stp.Elapsed);
        }
        internal static ExecutionReport Export(string fileName, ConcurrentDictionary<string, ConcurrentDictionary<char, List<AccEntry<int>>>> results, string header, string separator = "\t")
        {
            int intervalCount = 0;
            Stopwatch stp = new Stopwatch();
            stp.Restart();
            if (!File.Exists(fileName)) File.Delete(fileName);
            using (File.Create(fileName)) { }
            using (var writter = new StreamWriter(fileName))
            {
                writter.WriteLine(header);
                foreach (var chr in results)
                    foreach (var strand in chr.Value)
                    {
                        strand.Value.Sort();
                        foreach (var interval in strand.Value)
                        {
                            writter.WriteLine(chr.Key + separator + interval.Left.ToString() + separator + interval.Right.ToString() + separator + interval.Accumulation.ToString() + separator + strand.Key);
                            intervalCount++;
                        }
                    }
            }

            stp.Stop();
            return new ExecutionReport(intervalCount, stp.Elapsed);
        }
        internal static ExecutionReport Export(string fileName, ConcurrentDictionary<string, ConcurrentDictionary<char, SortedDictionary<int, int>>> results, SortedDictionary<int, int> mergedResults, string header, string separator = "\t")
        {
            int intervalCount = 0;
            Stopwatch stp = new Stopwatch();
            stp.Restart();
            if (!File.Exists(fileName)) File.Delete(fileName);
            using (File.Create(fileName)) { }
            using (var writter = new StreamWriter(fileName))
            {
                writter.WriteLine(header);
                foreach (var chr in results)
                    foreach (var strand in chr.Value)
                    {
                        foreach (var distribution in strand.Value)
                        {
                            writter.WriteLine(chr.Key + separator + strand.Key + separator + distribution.Key.ToString() + separator + distribution.Value.ToString());
                            intervalCount++;
                        }
                    }
            }

            string mergedResultsFile = fileName + "_merged";
            if (!File.Exists(mergedResultsFile)) File.Delete(mergedResultsFile);
            using (File.Create(mergedResultsFile)) { }
            using (var writter = new StreamWriter(mergedResultsFile))
            {
                writter.WriteLine("accumulation" + separator + "frequency");
                foreach (var pair in mergedResults)
                    writter.WriteLine(pair.Key + separator + pair.Value);
            }

            stp.Stop();
            return new ExecutionReport(intervalCount, stp.Elapsed);
        }
        internal static ExecutionReport Export(string fileName, ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<int>>>> results, string separator = "\t")
        {
            int intervalCount = 0;
            Stopwatch stp = new Stopwatch();
            stp.Restart();
            if (!File.Exists(fileName)) File.Delete(fileName);
            using (File.Create(fileName)) { }
            using (var writter = new StreamWriter(fileName))
                foreach (var chr in results)
                    foreach (var strand in chr.Value)
                        foreach (var block in strand.Value)
                        {
                            writter.WriteLine(chr.Key + separator + block.leftEnd + separator + block.rightEnd + separator + strand.Key);
                            intervalCount++;
                        }

            stp.Stop();
            return new ExecutionReport(intervalCount, stp.Elapsed);
        }
        internal static ExecutionReport Export(string fileName, BlockInfoDis results)
        {
            Stopwatch stp = new Stopwatch();
            stp.Restart();
            int intervalCount = 0;

            string icdF = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(fileName) + "_interval_count_distribution" + Path.GetExtension(fileName),
                macF = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(fileName) + "_maximum_accumulation_distribution" + Path.GetExtension(fileName);

            #region .::.   Interval Count Distribution          .::.
            if (!File.Exists(icdF)) File.Delete(icdF);
            using (File.Create(icdF)) { }
            using (var writter = new StreamWriter(icdF))
            {
                writter.WriteLine("IntervalCount\tCount");
                foreach (var item in results.intervalCountDis)
                {
                    writter.WriteLine(item.Key + "\t" + item.Value);
                    intervalCount++;
                }
            }
            #endregion
            #region .::.    Maximum Accumulation Distribution   .::.
            if (!File.Exists(macF)) File.Delete(macF);
            using (File.Create(macF)) { }
            using (var writter = new StreamWriter(macF))
            {
                writter.WriteLine("MaximumAccumulation\tCount");
                foreach (var item in results.maxAccDis)
                {
                    writter.WriteLine(item.Key + "\t" + item.Value);
                }
            }
            #endregion

            stp.Stop();
            return new ExecutionReport(intervalCount, stp.Elapsed);
        }
    }
}
