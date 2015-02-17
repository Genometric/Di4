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
        const int minGap = 1000;
        const int maxGap = 2000;
        const int maxLenght = 100; // this value must be >= 4
        const int chrCount = 1;
        const int regionsCount = 30; // per sample
        const string outputPath = "F:\\";
        const int sampleCount = 10; // IF YOU CHANGE THIS: remember to revise maxAcc.

        /// <summary>
        /// Maximum accumulation.
        /// This number must be less than sampleCount.
        /// </summary>
        const int maxAcc = 5;

        static Random rnd = new Random();


        static readonly int[] similarity = new int[] { 0 };//new int[] { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

        static readonly string[] chrTitles = new string[] { "chr1", "chr2", "chr3", "chr4", "chr5", "chr6", "chr7", "chr8", "chr9", "chr10", "chr11", "chr12", "chr13", "chr14", "chr15", "chr16", "chr17", "chr18", "chr19", "chr20", "chr21", "chr22", "chrX" };
        int[] regionsDistribution { set; get; }

        int newStart = 0;
        int newStop = 0;
        int lastStart = maxLenght + 1;
        int lastStop = maxLenght;

        int interStart = 0;
        int interStop = 0;

        public void GenerateRegions()
        {
            regionsDistribution = GetRegionsDistribution();

            // 0 : sample
            // 1 : chromosome
            // 2 : region
            var allRegions = new Dictionary<int, Dictionary<int, SortedDictionary<int, string>>>();

            Directory.CreateDirectory(outputPath + "\\count_" + regionsCount.ToString());

            for (int simIndex = 0; simIndex < similarity.Length; simIndex++)
            {
                Console.WriteLine("\nPreparing similarity : " + similarity[simIndex].ToString() + "%");
                Console.WriteLine("");

                Directory.CreateDirectory("count_" + regionsCount.ToString() + "\\similarity_" + similarity[simIndex].ToString() + "\\sorted\\");
                Directory.CreateDirectory("count_" + regionsCount.ToString() + "\\similarity_" + similarity[simIndex].ToString() + "\\unsorted\\");

                string path = outputPath + "\\count_" + regionsCount.ToString() + "\\similarity_" + similarity[simIndex].ToString() + "\\";

                allRegions.Clear();
                for (int sample = 0; sample < sampleCount; sample++)
                    allRegions.Add(sample, new Dictionary<int, SortedDictionary<int, string>>());


                for (int chr = 0; chr < chrCount; chr++)
                {
                    Console.WriteLine("\nGenerating Regions for chr" + chr.ToString());

                    // Each string[] representes a group; and each string in string[]
                    // represents the similar interval of different samples. 
                    List<string[]> regions = GetRegions(simIndex, chr, chrTitles[chr]);

                    for (int sample = 0; sample < sampleCount; sample++)
                    {
                        allRegions[sample].Add(chr, new SortedDictionary<int, string>());

                        for (int r = 0; r < regions.Count; r++)
                            allRegions[sample][chr].Add(r, regions[r][sample]);
                    }
                }

                Console.WriteLine("\n\nWriting to sorded file");
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    if (!Directory.Exists(path + "sorted\\")) Directory.CreateDirectory(path + "sorted\\");
                    using (FileStream fs =
                        new FileStream(path + "sorted\\sample_" + sample.ToString() + ".bed", FileMode.Append, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(fs))
                        for (int chr = 0; chr < chrCount; chr++)
                            for (int r = 0; r < allRegions[sample][chr].Count; r++)
                                sw.WriteLine(allRegions[sample][chr][r]);
                }


                int randomChr = 0;
                int randomRegion = 0;
                Console.WriteLine("\nWriting to unsorded file");
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    if (!Directory.Exists(path + "unsorted\\")) Directory.CreateDirectory(path + "unsorted\\");
                    using (FileStream fs =
                        new FileStream(path + "unsorted\\sample_" + sample.ToString() + ".bed", FileMode.Append, FileAccess.Write))
                        using (StreamWriter sw = new StreamWriter(fs))
                            while (allRegions[sample].Count > 0)
                            {
                                randomChr = allRegions[sample].ElementAt(rnd.Next(0, allRegions[sample].Count)).Key;
                                randomRegion = allRegions[sample][randomChr].ElementAt(rnd.Next(0, allRegions[sample][randomChr].Count)).Key;
                                sw.WriteLine(allRegions[sample][randomChr][randomRegion]);
                                allRegions[sample][randomChr].Remove(randomRegion);
                                if (allRegions[sample][randomChr].Count == 0)
                                    allRegions[sample].Remove(randomChr);
                            }
                }

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
        private List<string[]> GetRegions(int varIndex, int chr, string chrTitle)
        {
            // similarity combination count
            // Based on similarity percentage, how many dichotomic silimarity-groups are needed ?
            int simCount = (int)Math.Round((similarity[varIndex] * regionsDistribution[chr]) / 100.0);

            List<string[]> rtv = new List<string[]>();

            // similar or variant random switch.
            //Random svRS = new Random();

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

                rtv.Add(rtvS);

                Console.Write("\r{0} \\ {1}", group.ToString(), regionsDistribution[chr].ToString());
            }

            return rtv;
        }

        private string GetRandomName()
        {
            string rtv = "";
            char[] chars = new char[] { 'H', 'A', 'M', 'E', 'D', 'V', 'I', '5', '3', '0', '1' };

            while (rtv.Length < 6)
                rtv += chars[rnd.Next(0, chars.Length - 1)];

            return rtv;
        }
    }
}
