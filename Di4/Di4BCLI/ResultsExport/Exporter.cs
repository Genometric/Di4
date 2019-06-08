using Genometric.Di4.Di4B;
using Genometric.Di4.Di4B.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Genometric.Di4.CLI
{
    internal static class Exporter
    {
        private static Stopwatch _stopWatch = new Stopwatch();
        internal static ExecutionReport Export(string fileName, FunctionOutput<Output<int, Peak, PeakData>> results, string header, string separator = "\t")
        {
            int intervalCount = 0;
            _stopWatch.Restart();
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

            _stopWatch.Stop();
            return new ExecutionReport(intervalCount, _stopWatch.Elapsed);
        }
        internal static ExecutionReport Export(string fileName, ConcurrentDictionary<string, ConcurrentDictionary<char, List<AccEntry<int>>>> results, string header, Mux mux = Mux.Join, string separator = "\t")
        {
            int intervalCount = 0;
            _stopWatch.Restart();

            if (mux == Mux.Join)
            {
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
            }
            else
            {
                string chrFile;
                foreach (var chr in results)
                {
                    chrFile = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(fileName) + "_" + chr.Key + Path.GetExtension(fileName);
                    if (!File.Exists(chrFile)) File.Delete(chrFile);
                    using (File.Create(chrFile)) { }
                    using (var writter = new StreamWriter(chrFile))
                    {
                        writter.WriteLine(header);
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
                }
            }

            _stopWatch.Stop();
            return new ExecutionReport(intervalCount, _stopWatch.Elapsed);
        }
        internal static ExecutionReport Export(string fileName, ConcurrentDictionary<string, ConcurrentDictionary<char, SortedDictionary<int, int>>> results, SortedDictionary<int, int> mergedResults, string header, string separator = "\t")
        {
            int intervalCount = 0;
            _stopWatch.Restart();
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

            _stopWatch.Stop();
            return new ExecutionReport(intervalCount, _stopWatch.Elapsed);
        }
        internal static ExecutionReport Export(string fileName, ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<int>>>> results, string separator = "\t")
        {
            int intervalCount = 0;
            _stopWatch.Restart();
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

            _stopWatch.Stop();
            return new ExecutionReport(intervalCount, _stopWatch.Elapsed);
        }
        internal static ExecutionReport Export(string fileName, SortedDictionary<string, SortedDictionary<char, Stats>> results, string separator = "\t")
        {
            _stopWatch.Restart();
            if (!File.Exists(fileName)) File.Delete(fileName);
            using (File.Create(fileName)) { }
            using (var writter = new StreamWriter(fileName))
            {
                writter.WriteLine("Chr" + separator + "strand" + separator + "intervalCount" + separator + "bookmarkCount" + separator + "blockCount");
                foreach (var chr in results)
                    foreach (var strand in chr.Value)
                    {
                        writter.WriteLine(
                            chr.Key + separator +
                            strand.Key + separator +
                            strand.Value.intervalCount + separator +
                            strand.Value.bookmarkCount + separator +
                            strand.Value.blockCount);
                    }
            }
            _stopWatch.Stop();
            return new ExecutionReport(0, _stopWatch.Elapsed);
        }
        internal static ExecutionReport Export(string fileName, BlockInfoDis results)
        {
            int intervalCount = 0;
            _stopWatch.Restart();

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

            _stopWatch.Stop();
            return new ExecutionReport(intervalCount, _stopWatch.Elapsed);
        }
    }
}
