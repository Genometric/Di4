using System;
using System.Collections.Generic;
using Di3B;
using System.IO;
using BEDParser;
using System.Diagnostics;

namespace Di3BMain
{
    public class Orchestrator
    {
        public Orchestrator(string workingDirectory)
        {
            int32Comparer = new Int32Comparer();
            samplesHashtable = new Dictionary<string, uint>();
            stopWatch = new Stopwatch();
            di3B = new Di3B<int, Peak, PeakData>(workingDirectory, Memory.HDD, HDDPerformance.Fastest, PrimitiveSerializer.Int32, int32Comparer);


            ///------ TEST A
            //Load_and_Add();

            ///------ TEST B
            LID(new string[] { "", @"I:\New folder\", "narrowPeak" });
        }

        Stopwatch stopWatch { set; get; }

        internal string CommandParse(string command)
        {
            string[] splittedCommand = command.Split(' ');

            if (splittedCommand.Length > 1)
            {
                switch (splittedCommand[0].ToLower())
                {
                    case "exit":
                        return "exit";

                    case "load":
                        stopWatch.Restart();
                        Load(splittedCommand);
                        stopWatch.Stop();
                        return stopWatch.Elapsed.ToString();

                    case "loadall":
                        stopWatch.Restart();
                        LoadAll(splittedCommand);
                        stopWatch.Stop();
                        return stopWatch.Elapsed.ToString();

                    case "index":
                        stopWatch.Restart();
                        Index(splittedCommand);
                        stopWatch.Stop();
                        return stopWatch.Elapsed.ToString();

                    case "cover": // example: cover * 1 4 count
                        stopWatch.Restart();
                        Cover(splittedCommand);
                        stopWatch.Stop();
                        return stopWatch.Elapsed.ToString();

                    case "summit": // example: summit * 2 4 count
                        stopWatch.Restart();
                        Summit(splittedCommand);
                        stopWatch.Stop();
                        return stopWatch.Elapsed.ToString();

                    case "map": // example: Map E:\reference.bed * count
                        stopWatch.Restart();
                        Map(splittedCommand);
                        stopWatch.Stop();
                        return stopWatch.Elapsed.ToString();

                    case "lid":
                        stopWatch.Restart();
                        LID(splittedCommand);
                        stopWatch.Stop();
                        return stopWatch.Elapsed.ToString();


                    default:
                        return "Unknown command";
                }
            }
            else
            {
                if (splittedCommand[0].ToLower() == "exit") return "exit";
                else return "Unknown command, or missing parameters";
            }
        }

        Di3B<int, Peak, PeakData> di3B { set; get; }

        Int32Comparer int32Comparer { set; get; }

        Dictionary<string, UInt32> samplesHashtable { set; get; }

        public void Load_and_Add()
        {
            var tempData = new Dictionary<string, List<Peak>>();
            for (int i = 0; i < 5; i++)
            {
                var points = new List<Peak>();
                for (int j = 0; j < 1000000; j++)
                    points.Add(new Peak()
                    {
                        left = j,
                        right = j + 10,
                        metadata = new PeakData()
                        {
                            left = j,
                            right = j + 10,
                            name = "Hamed",
                            value = 1000,
                            hashKey = 1000000
                        }
                    });

                tempData.Add("data" + i, points);
            }
            Repository.repo.Add(10, tempData);//only one collection.

            foreach (var collection in Repository.repo)
                di3B.Add(collection.Value);

            Repository.repo.Remove(10);
        }


        private void Load(string[] args)
        {
            Repository.inputSamples.Add(args[1]);
            BEDParser<Peak, PeakData> bedParser = new BEDParser<Peak, PeakData>(args[1], "Human");

            var parsedSample = bedParser.Parse();

            Repository.parsedSamples.Add(parsedSample.fileHashKey, parsedSample);
            samplesHashtable.Add(args[1], parsedSample.fileHashKey);
        }

        private void LoadAll(string[] args)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(args[1]);

            int counter = -1;

            foreach (var file in dirInfo.GetFiles("*." + args[2]))
            {
                Herald.Announce(String.Format("... Now Loading: [{0}\\{1}] {2}", (++counter), dirInfo.GetFiles("*." + args[2]).Length, file.Name));
                Load(new string[] { "null", file.FullName });
            }
        }

        private void LID(string[] args)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(args[1]);

            int counter = 0;

            foreach (var file in dirInfo.GetFiles("*." + args[2]))
            {
                Herald.Announce(String.Format("... Now Loading: [{0}\\{1}] {2}", (++counter), dirInfo.GetFiles("*." + args[2]).Length, file.Name));
                Load(new string[] { "null", file.FullName });

                Herald.Announce(String.Format("... Now Indexing: [{0}\\{1}] {2}", counter, dirInfo.GetFiles("*." + args[2]).Length, file.Name));
                Index(new string[] { "null", file.FullName });

                Repository.parsedSamples.Remove(samplesHashtable[file.FullName]);
                
                //GC.Collect();
                //GC.SuppressFinalize(this);
                //GC.WaitForPendingFinalizers();
            }
        }

        private void Index(string[] args)
        {
            var peaks = Repository.parsedSamples[samplesHashtable[args[1]]].peaks;
            di3B.Add(peaks);
        }

        private void Cover(string[] args)
        {
            di3B.Cover(Convert.ToChar(args[1]), Convert.ToByte(args[2]), Convert.ToByte(args[3]), args[4]);
        }

        private void Summit(string[] args)
        {
            di3B.Summit(Convert.ToChar(args[1]), Convert.ToByte(args[2]), Convert.ToByte(args[3]), args[4]);
        }

        private void Map(string[] args)
        {
            Load(new string[] { "", args[1] });
            var referencePeaks = Repository.parsedSamples[samplesHashtable[args[1]]].peaks;
            di3B.Map(Char.Parse(args[2]), referencePeaks, args[3]);
        }


        private ParsedBED<int, Peak, PeakData> TESTBED()
        {
            Random rnd = new Random();
            string[] chrs = new string[] { "chr1", "chr2", "chr3", "chr4", "chr5", "chr6", "chr7", "chr8", "chr9", "chr10", "chr11", "chr12", "chr13", "chr14", "chr15", "chr16" };
            var data = new ParsedBED<int, Peak, PeakData>();

            foreach (var chr in chrs)
            {
                var peaks = new List<Peak>();

                int p = 0;
                for (int i = 0; i < 10000; i++)
                {
                    int l = rnd.Next(p, p + 20);
                    int r = rnd.Next(p + 10, p + 30);

                    peaks.Add(new Peak()
                    {
                        left = l,
                        right = r,
                        metadata = new PeakData()
                        {
                            left = l,
                            right = r,
                            name = "Hamedlkjlslkfjlksdjvlksdjvdsjfe",
                            value = rnd.NextDouble(),
                            hashKey = (UInt32)rnd.Next(10000, 100000000)
                        }
                    });

                    p += rnd.Next(20, 50);
                }

                data.peaks.Add(chr, peaks);
            }

            data.fileHashKey = (uint)rnd.Next(100, 10000000);

            return data;
        }
    }
}
