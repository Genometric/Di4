using Polimi.DEIB.VahidJalili.DI3.CLI;
using Polimi.DEIB.VahidJalili.GIFP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polimi.DEIB.VahidJalili.DI3.SimulationDataGenerator
{
    internal class RegionGenerator
    {
        const int minGap = 50;
        const int maxGap = 100;
        const int maxLenght = 1000; // this value must be >= 4
        const int chrCount = 23;
        const int regionsCount = 20000; // per sample
        const int sampleCount = 500; // IF YOU CHANGE THIS: remember to revise maxAcc.
        const int maxAcc = 400; // Maximum accumulation. This number must be less than sampleCount.

        int newStart = 0;
        int newStop = 0;
        int chrStart = 0;
        int chrStop = 0;
        int lastStart = maxLenght + 1;
        int lastStop = maxLenght;
        int interStart = 0;
        int interStop = 0;
        char dirSep = Path.DirectorySeparatorChar;

        const string parentPath = "";
        static string filesExtension = "bed";

        static readonly int[] similarity = new int[] { /*0, 10, 20, 30,*/ 40, 50, 60, 70, 80, 90, 100 };

        static readonly string[] chrTitles = new string[] {
            "chr1","chr2","chr3","chr4","chr5",
            "chr6","chr7","chr8","chr9","chr10",
            "chr11","chr12", "chr13", "chr14", "chr15",
            "chr16", "chr17", "chr18", "chr19", "chr20",
            "chr21", "chr22", "chrX", "chrY" };

        int[] regionsDistribution { set; get; }
        static Random rnd = new Random();
        

        public void GenerateSimulationRegions()
        {
            regionsDistribution = GetRegionsDistribution();
            Directory.CreateDirectory(parentPath + "count_" + regionsCount.ToString());
            string outputPath = "";

            for (int simIndex = 0; simIndex < similarity.Length; simIndex++)
            {
                chrStart = lastStart;
                Console.WriteLine("Preparing similarity : " + similarity[simIndex].ToString() + "%");

                outputPath = parentPath + "count_" + regionsCount.ToString() + dirSep + "similarity_" + similarity[simIndex].ToString() + dirSep;
                Directory.CreateDirectory(outputPath + dirSep + "sorted" + dirSep);
                Directory.CreateDirectory(outputPath + dirSep + "shuffled" + dirSep);

                for (int chr = 0; chr < chrCount; chr++)
                {
                    Console.WriteLine("Generating Regions for chr" + chr.ToString());
                    CreateSortedRegions(simIndex, chr, chrTitles[chr], outputPath);
                    chrStop = lastStop;
                    CreateMapFile(chr, chrTitles[chr], outputPath);
                    chrStart = lastStop;
                }
                
                CreateShuffleRegions(outputPath);

                newStart = 0;
                newStop = 0;
                lastStart = maxLenght + 1;
                lastStop = maxLenght;

                interStart = 0;
                interStop = 0;
            }
        }
        private int[] GetRegionsDistribution()
        {
            int[] rtv = new int[chrCount];
            int chrSum = (chrCount * (chrCount + 1)) / 2;

            int nDistributedRegions = 0;
            for (int chr = 0; chr < chrCount; chr++)
            {
                rtv[chr] = (int)Math.Floor((double)((chrCount - chr) * regionsCount) / (double)chrSum);
                nDistributedRegions += rtv[chr];
            }

            if (nDistributedRegions < regionsCount)
                rtv[rtv.Length - 1] += (regionsCount - nDistributedRegions);

            return rtv;
        }
        private void CreateSortedRegions(int varIndex, int chr, string chrTitle, string filePath)
        {
            // similarity combination count
            // Based on similarity percentage, how many dichotomic silimarity-groups are needed ?
            int simCount = (int)Math.Round((similarity[varIndex] * regionsDistribution[chr]) / 100.0);

            // similar or variant switch.
            char svS = 'S';

            // generated similar groups count.
            int gSimCount = 0;

            // gap between this and prevoius group
            int gapPG = 0;

            Console.WriteLine("Total Created Regions : ");

            for (int group = 0; group < regionsDistribution[chr]; group++)
            {
                #region .::. Create Variant or Similar groups ?     .::.

                if (simCount != 0 && simCount - gSimCount < regionsDistribution[chr] - group)
                { // Generate either similar or variant groups.

                    // create similar groups.
                    if (rnd.NextDouble() > 0.5) svS = 'S';

                    // create variant groups.
                    else svS = 'V';
                }

                // generate only variant groups.
                else if (simCount <= gSimCount) svS = 'V';

                // generate only similar groups.
                else svS = 'S';

                #endregion

                gapPG = rnd.Next(minGap, maxGap);
                string[] rtvS = new string[sampleCount];

                switch (svS)
                {
                    case 'S':
                        interStart = rnd.Next(lastStop + gapPG + maxLenght + 2, lastStop + gapPG + (2 * maxLenght));
                        interStop = rnd.Next(interStart + 1, interStart + 1 + (int)Math.Floor(maxLenght / 10.0));
                        /// why maxLength / 10.0 ?
                        /// This specifies the maximum allowed portion of available window to be dedicated
                        /// to intersection region. For instance, for a window of length 100, intersection
                        /// region can be at most of lenght 10.

                        for (int s = 0; s < sampleCount; s++)
                        {
                            newStart = rnd.Next(interStart + (interStop - interStart) - maxLenght + 2, interStart - 1);
                            newStop = rnd.Next(interStop + 1, maxLenght + newStart);

                            lastStop = Math.Max(lastStop, newStop);

                            rtvS[s] =
                                chrTitle + "\t" +
                                newStart.ToString() + "\t" +
                                newStop.ToString() + "\t" +
                                GetRandomName() + "\t" +
                                Math.Round(rnd.NextDouble(), 3).ToString();
                        }

                        gSimCount++;

                        lastStart = newStart;
                        break;

                    case 'V':
                        var tmpLst = new List<int[]>();
                        bool regenerateLastRegion = false;
                        bool regenerateAllRegions = false;
                        int intCount = 0;
                        int maxReTr = 5; // maximum allowed regeneration trials.
                        int regenTrialCount = 0; // regeneration trials count.
                        int tmpLastStop = 0;
                        for (int s = 0; s < sampleCount; s++)
                        {
                            regenerateLastRegion = true;
                            regenerateAllRegions = false;
                            regenTrialCount = 0;

                            do
                            {
                                do
                                {
                                    regenTrialCount++;
                                    newStart = rnd.Next(lastStop + gapPG, lastStop + gapPG + maxLenght);
                                    newStop = rnd.Next(newStart + 1, newStart + 1 + maxLenght);

                                    tmpLastStop = Math.Max(tmpLastStop, newStop);

                                    intCount = 0;
                                    foreach (var itm in tmpLst)
                                        if (!(newStop < itm[0] || newStart > itm[1]))
                                            intCount++;

                                    regenerateLastRegion = false;
                                    if (intCount < maxAcc)
                                        tmpLst.Add(new int[] { newStart, newStop });
                                    else
                                        regenerateLastRegion = true;
                                } while (regenerateLastRegion && regenTrialCount < maxReTr);

                                regenerateAllRegions = false;
                                if (regenTrialCount >= maxReTr)
                                {
                                    rtvS = new string[sampleCount];
                                    tmpLst.Clear();
                                    regenerateLastRegion = true;
                                    regenTrialCount = 0;
                                    regenerateAllRegions = true;
                                    s = 0;
                                }
                            } while (regenerateAllRegions);

                            rtvS[s] =
                                chrTitle + "\t" +
                                newStart.ToString() + "\t" +
                                newStop.ToString() + "\t" +
                                GetRandomName() + "\t" +
                                Math.Round(rnd.NextDouble(), 3).ToString();
                        }

                        lastStop = tmpLastStop;
                        break;
                }

                for (int sample = 0; sample < sampleCount; sample++)
                {
                    if (!Directory.Exists(filePath + "sorted" + dirSep)) Directory.CreateDirectory(filePath + "sorted" + dirSep);
                    using (FileStream fs =
                        new FileStream(filePath + "sorted" + dirSep + "sample_" + sample.ToString() + "." + filesExtension, FileMode.Append, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(fs))
                        sw.WriteLine(rtvS[sample]);
                }

                Console.Write("\r{0:N0} \\ {1:N0}", (group + 1).ToString(), regionsDistribution[chr].ToString());
            }

            Console.WriteLine("Done !");
        }
        private void CreateShuffleRegions(string folderPath)
        {
            Console.WriteLine("Writing shuffled files.");
            string randomChr = null;
            char randomStrand = 'V';
            int randomRegion = 0;
            Peak peak = null;
            var dirInfo = new DirectoryInfo(folderPath + dirSep+"sorted");
            FileInfo[] determinedFiles = dirInfo.GetFiles("*." + filesExtension);
            foreach (FileInfo fileInfo in determinedFiles)
            {
                Console.WriteLine(string.Format("Now writing: {0}", Path.GetFileNameWithoutExtension(fileInfo.FullName)));
                BEDParser<Peak, PeakData> bedParser = new BEDParser<Peak, PeakData>(fileInfo.FullName, Genomes.HomoSapiens, Assemblies.hm19, true);
                var parsedSample = bedParser.Parse();
                var intervals = parsedSample.intervals;

                if (!Directory.Exists(folderPath + "shuffled" + dirSep)) Directory.CreateDirectory(folderPath + "shuffled" + dirSep);
                using (FileStream fs =
                    new FileStream(folderPath + "shuffled" + dirSep + Path.GetFileNameWithoutExtension(fileInfo.FullName) + "." + filesExtension, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                    while (intervals.Count > 0)
                    {
                        randomChr = intervals.ElementAt(rnd.Next(0, intervals.Count)).Key;
                        randomStrand = intervals[randomChr].ElementAt(rnd.Next(0, intervals[randomChr].Count)).Key;
                        randomRegion = rnd.Next(0, intervals[randomChr][randomStrand].Count);
                        peak = intervals[randomChr][randomStrand][randomRegion];
                        sw.WriteLine(randomChr + "\t" + peak.ToString("\t") + "\t" + randomStrand);
                        intervals[randomChr][randomStrand].RemoveAt(randomRegion);
                        if (intervals[randomChr][randomStrand].Count == 0)
                            intervals[randomChr].Remove(randomStrand);
                        if (intervals[randomChr].Count == 0)
                            intervals.Remove(randomChr);
                    }
            }
            Console.WriteLine("Done");
        }
        private void CreateMapFile(int chr, string chrTitle, string filePath)
        {
            SortedDictionary<interval, string> intervals = new SortedDictionary<interval, string>();
            int start, stop;

            for (int group = 0; group < regionsDistribution[chr]; group++)
            {
                for (int i = 0; i < regionsDistribution[chr]; i++)
                {
                    do
                    {
                        start = rnd.Next(chrStart, chrStop);
                        stop = rnd.Next(start + 1, start + maxLenght);
                    } while (start >= lastStop && stop >= lastStop);

                    intervals.Add(new interval() { start = start, stop = stop },
                        chrTitle + "\t" +
                        start.ToString() + "\t" +
                        stop.ToString() + "\t" +
                        GetRandomName() + "\t" +
                        Math.Round(rnd.NextDouble(), 3).ToString());
                }
            }

            using (FileStream fs =
                        new FileStream(filePath + "sorted" + dirSep + "mapRef." + filesExtension, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
                foreach (var interval in intervals)
                    sw.WriteLine(interval.Value);
        }

        private string GetRandomName()
        {
            string rtv = "";
            char[] chars = new char[] { 'H', 'A', 'M', 'E', 'D', 'V', 'I', '5', '3', '0', '1' };

            while (rtv.Length < 6)
                rtv += chars[rnd.Next(0, chars.Length - 1)];

            return rtv;
        }

        private class interval : IComparable<interval>
        {
            public int start { set; get; }
            public int stop { set; get; }


            public int CompareTo(interval other)
            {
                if (other == null) return 1;
                if (this.start != other.start) return this.start.CompareTo(other.start);
                return this.stop.CompareTo(other.stop);
            }
        }
    }
}
