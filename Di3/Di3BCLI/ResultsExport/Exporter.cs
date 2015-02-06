using Di3B;
using Di3B.Logging;
using System.Diagnostics;
using System.IO;

namespace Di3BCLI
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
    }
}
