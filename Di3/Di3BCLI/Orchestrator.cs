using System;
using System.Collections.Generic;
using System.Linq;
using Di3B;
using System.IO;
using BEDParser;
using System.Diagnostics;
using Di3B.Logging;
using BEDParser.AssembliesInfo;

namespace Di3BCLI
{
    public class Orchestrator
    {
        public Orchestrator(string workingDirectory)
        {
            _workingDirectory = workingDirectory;

            int32Comparer = new Int32Comparer();
            samplesHashtable = new Dictionary<int, uint>();
            stopWatch = new Stopwatch();
            di3B = new Di3B<int, Peak, PeakData>(_workingDirectory, Memory.HDD, HDDPerformance.Fastest, PrimitiveSerializer.Int32, int32Comparer);
        }

        private string _workingDirectory { set; get; }

        Stopwatch stopWatch { set; get; }

        Di3B<int, Peak, PeakData> di3B { set; get; }

        internal bool CommandParse(string command)
        {
            Herald.Announce(Herald.MessageType.None, "> " + command, Herald.Destination.File);

            string[] splittedCommand = command.Split(' ');

            if (splittedCommand.Length > 1)
            {
                switch (splittedCommand[0].ToLower())
                {
                    case "exit":
                        return true;

                    case "load":
                        stopWatch.Restart();
                        if (!Load(splittedCommand))
                        {
                            stopWatch.Stop();
                            return false;
                        }
                        break;
                        

                    case "loadall":
                        stopWatch.Restart();
                        if(!LoadAll(splittedCommand))
                        {
                            stopWatch.Stop();
                            return false;
                        }
                        break;

                    case "index":
                        stopWatch.Restart();
                        if (!Index(splittedCommand))
                        {
                            stopWatch.Stop();
                            return false;
                        }
                        break;

                    case "cover":
                    case "summit":
                        stopWatch.Restart();
                        if (!Cover(splittedCommand, splittedCommand[0].ToLower()))
                        {
                            stopWatch.Stop();
                            return false;
                        }
                        break;

                    case "map": // example: Map E:\reference.bed * count
                        stopWatch.Restart();
                        if(!Map(splittedCommand))
                        {
                            stopWatch.Stop();
                            return false;
                        }
                        break;

                    case "lid":
                        stopWatch.Restart();
                        if(!LID(splittedCommand))
                        {
                            stopWatch.Stop();
                            return false;
                        }
                        break;


                    default:
                        Herald.Announce(Herald.MessageType.Error, "Unknown Command");
                        return false;
                }
            }
            else
            {
                if (splittedCommand[0].ToLower() == "exit") return true;
                else
                {
                    Herald.Announce(Herald.MessageType.Error, "Unknown command, or missing parameters");
                    return false;
                }
            }

            stopWatch.Stop();
            Herald.Announce(Herald.MessageType.Success, String.Format("Runtime: {0}", stopWatch.Elapsed.ToString()));
            return false;
        }

        

        Int32Comparer int32Comparer { set; get; }

        Dictionary<int, UInt32> samplesHashtable { set; get; }


        private bool Load(string[] args)
        {
            if (Path.GetDirectoryName(args[1]).Trim() == "")
                args[1] = _workingDirectory + Path.DirectorySeparatorChar + args[1];

            if (File.Exists(args[1]))
            {
                Repository.inputSamples.Add(args[1]);
                BEDParser<Peak, PeakData> bedParser = new BEDParser<Peak, PeakData>(args[1], AvailableGenomes.HomoSapiens, AvailableAssemblies.hm19);
                if (!Repository.parsedSamples.ContainsKey(bedParser.fileHashKey))
                {
                    var parsedSample = bedParser.Parse();

                    Repository.parsedSamples.Add(parsedSample.fileHashKey, parsedSample);
                    samplesHashtable.Add(samplesHashtable.Count, parsedSample.fileHashKey);

                    Herald.Announce(Herald.MessageType.Info, String.Format("Parsed {0:N0} peaks", parsedSample.peaksCount));
                    Herald.Announce(Herald.MessageType.Info, String.Format("Reference No. : {0}", samplesHashtable.Count - 1));

                    return true;
                }
                else
                {
                    Herald.Announce(Herald.MessageType.Info,
                        String.Format("The file is already parsed with following reference No. {0}",
                        samplesHashtable.Keys.OfType<int>().First(i => samplesHashtable[i] == bedParser.fileHashKey)));

                    return false;
                }
            }
            else
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("File not found [{0}]", args[1]));
                return false;
            }
        }

        private bool LoadAll(string[] args)
        {
            DirectoryInfo dirInfo;
            if (args.Length < 3)
            {
                dirInfo = new DirectoryInfo(_workingDirectory);
            }
            else
            {
                Uri logFileURI = null;
                if (Uri.TryCreate(args[2], UriKind.Absolute, out logFileURI) == false)
                {
                    Herald.Announce(Herald.MessageType.Error, "Invalid path URI format.");
                    return false;
                }
                if (!Directory.Exists(logFileURI.AbsolutePath))
                {
                    Herald.Announce(Herald.MessageType.Error, "Path does not exist.");
                    return false;
                }
                dirInfo = new DirectoryInfo(logFileURI.AbsolutePath);
            }

            int counter = 0;
            FileInfo[] determinedFiles = dirInfo.GetFiles("*." + args[1]);

            foreach (var file in determinedFiles)
            {
                Herald.Announce(Herald.MessageType.Info, String.Format("... Now Loading: [{0}\\{1}] {2}", (++counter), determinedFiles.Length, file.Name));
                Load(new string[] { "null", file.FullName });
                Herald.Announce(Herald.MessageType.None, "");
            }

            return true;
        }

        private bool LID(string[] args)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(args[1]);

            int counter = 0;

            foreach (var file in dirInfo.GetFiles("*." + args[2]))
            {
                //Herald.Announce(String.Format("... Now Loading: [{0}\\{1}] {2}", (++counter), dirInfo.GetFiles("*." + args[2]).Length, file.Name));
                Herald.Announce(Herald.MessageType.Info, String.Format("... {0,8} : [{1:N0}\\{2:N0}] {3,30}", "Loading", (++counter), dirInfo.GetFiles("*." + args[2]).Length, file.Name));
                Load(new string[] { "null", file.FullName });

                Herald.Announce(Herald.MessageType.Info, String.Format("... {0,8} : [{1:N0}\\{2:N0}] {3,30}", "Indexing", counter, dirInfo.GetFiles("*." + args[2]).Length, file.Name));
                Index(new string[] { "null", file.FullName });

                //Repository.parsedSamples.Remove(samplesHashtable[file.FullName]);

                //GC.Collect();
                //GC.SuppressFinalize(this);
                //GC.WaitForPendingFinalizers();
            }

            return true;
        }

        private bool Index(string[] args)
        {
            int refNo;
            if (int.TryParse(args[1], out refNo) && samplesHashtable.ContainsKey(refNo))
            {
                var peaks = Repository.parsedSamples[samplesHashtable[refNo]].peaks;
                ExecutionReport exeReport = di3B.Add(peaks);
                Herald.Announce(Herald.MessageType.None,
                    /*-*/ String.Format(" #i: {0,9}     ET: {1,6}     Speed: {2,10}",
                    /*0*/ String.Format("{0:N0}", exeReport.count),
                    /*1*/ exeReport.ET,
                    /*2*/ String.Format("{0:N0} #i\\sec", Math.Round(exeReport.count / exeReport.ET.TotalSeconds, 2))));
                return true;
            }
            else
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid Reference No."));
                return false;
            }
        }

        private bool Cover(string[] args, string coverORsummit)
        {
            if (args.Length < 5)
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Missing parameter."));
                return false;
            }

            char strand;
            byte minAcc, maxAcc;
            
            if(!Char.TryParse(args[1], out strand))
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid strand parameter."));
                return false;
            }
            if(!Byte.TryParse(args[2], out minAcc))
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid minimum accumulation parameter."));
                return false;
            }
            if (!Byte.TryParse(args[3], out maxAcc))
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid maximum accumulation parameter."));
                return false;
            }

            Aggregate agg = Aggregate.Count;
            if(!String2Aggregate(args[4], out agg))
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid aggregate parameter."));
                return false;
            }

            switch(coverORsummit)
            {
                case "cover":
                    di3B.Cover(CoverVariation.Cover, strand, minAcc, maxAcc, agg);
                    break;

                case "summit":
                    di3B.Cover(CoverVariation.Summit, strand, minAcc, maxAcc, agg);
                    break;
            }

            return true;
        }


        private bool Map(string[] args)
        {
            int refNo;
            if (int.TryParse(args[1], out refNo))
            {
                Load(new string[] { "", args[1] });
                var referencePeaks = Repository.parsedSamples[samplesHashtable[refNo]].peaks;

                /// -----------       FIX aggregate function
                //di3B.Map(Char.Parse(args[2]), referencePeaks, args[3]);
                return true;
            }
            else return false;
        }

        private bool String2Aggregate(string strAggregate, out Aggregate aggregate)
        {
            switch (strAggregate.ToLower().Trim())
            {
                case "count":
                    aggregate = Aggregate.Count;
                    return true;

                case "sum":
                    aggregate = Aggregate.Sum;
                    return true;

                case "max":
                case "maximum":
                    aggregate = Aggregate.Maximum;
                    return true;

                case "min":
                case "minimum":
                    aggregate = Aggregate.Minimum;
                    return true;

                case "mean":
                case "average":
                    aggregate = Aggregate.Mean;
                    return true;

                case "median":
                    aggregate = Aggregate.Median;
                    return true;

                case "std":
                case "standarddeviation":
                    aggregate = Aggregate.StandardDeviation;
                    return true;

                default:
                    aggregate = Aggregate.Count;
                    return false;

            }
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
