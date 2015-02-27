using Polimi.DEIB.VahidJalili.DI3.DI3B;
using Polimi.DEIB.VahidJalili.DI3.DI3B.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Polimi.DEIB.VahidJalili.DI3.CLI
{
    internal static class Exporter
    {
        internal static ExecutionReport Export(string fileName, FunctionOutput<Output<int, Peak, PeakData>> results, string header, string separator = "\t")
        {
            int intervalCount = 0;
            Stopwatch stp = new Stopwatch();


            int dummyNullCounter = 0;


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
                            if (interval == null)
                                dummyNullCounter++;


                            /*
                            writter.WriteLine(
                                chr.Key + separator +
                                interval.interval.left.ToString() + separator +
                                interval.interval.right.ToString() + separator +
                                interval.count.ToString() + separator +
                                strand.Key);*/
                            intervalCount++;
                        }
            }

            stp.Stop();
            return new ExecutionReport(intervalCount, stp.Elapsed);
        }
        internal static ExecutionReport Export(string fileName, Dictionary<string, Dictionary<char, List<AccEntry<int>>>> results, string header, string separator = "\t")
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
        internal static ExecutionReport Export(string fileName, Dictionary<string, Dictionary<char, SortedDictionary<int, int>>> results, string header, string separator = "\t")
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

            stp.Stop();
            return new ExecutionReport(intervalCount, stp.Elapsed);
        }
        internal static ExecutionReport Export(string fileName, Dictionary<string, Dictionary<char, ICollection<BlockKey<int>>>> results, string separator = "\t")
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
    }
}
