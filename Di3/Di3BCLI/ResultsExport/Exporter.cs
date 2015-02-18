using Polimi.DEIB.VahidJalili.DI3.DI3B;
using Polimi.DEIB.VahidJalili.DI3.DI3B.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Polimi.DEIB.VahidJalili.DI3.CLI
{
    internal static class Exporter
    {
        public static ExecutionReport Export(string fileName, FunctionOutput<Output<int, Peak, PeakData>> result, string separator = "\t")
        {
            int intervalCount = 0;
            Stopwatch stp = new Stopwatch();

            stp.Restart();
            if (!File.Exists(fileName)) File.Delete(fileName);
            using (File.Create(fileName)) { }
            using (var writter = new StreamWriter(fileName))
                foreach (var chr in result.Chrs)
                    foreach (var strand in chr.Value)
                        foreach (var interval in strand.Value)
                        {
                            writter.WriteLine(chr.Key + separator + interval.interval.ToString() + separator + strand.Key);
                            intervalCount++;
                        }

            stp.Stop();
            return new ExecutionReport(intervalCount, stp.Elapsed);
        }

        public static ExecutionReport Export(string fileName, Dictionary<string, Dictionary<char, IEnumerable<AccEntry<int>>>> result, string separator = "\t")
        {
            int intervalCount = 0;
            Stopwatch stp = new Stopwatch();

            stp.Restart();
            if (!File.Exists(fileName)) File.Delete(fileName);
            using (File.Create(fileName)) { }
            using (var writter = new StreamWriter(fileName))
                foreach (var chr in result)
                    foreach (var strand in chr.Value)
                        foreach (var interval in strand.Value)
                        {
                            writter.WriteLine(chr.Key + separator + interval.Left.ToString() + separator + interval.Right.ToString() + separator + interval.Accumulation.ToString() + separator + strand.Key);
                            intervalCount++;
                        }

            stp.Stop();
            return new ExecutionReport(intervalCount, stp.Elapsed);
        }
    }
}
