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
            parserSTW = new Stopwatch();
            di3B = new Di3B<int, Peak, PeakData>(_workingDirectory, Memory.HDD, HDDPerformance.Fastest, PrimitiveSerializer.Int32, int32Comparer);
        }

        private string _workingDirectory { set; get; }
        private Stopwatch stopWatch { set; get; }
        private Stopwatch parserSTW { set; get; }
        Di3B<int, Peak, PeakData> di3B { set; get; }
        Int32Comparer int32Comparer { set; get; }
        Dictionary<int, UInt32> samplesHashtable { set; get; }

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

                    case "index":
                        stopWatch.Restart();
                        if (!Index(splittedCommand))
                        {
                            stopWatch.Stop();
                            return false;
                        }
                        break;

                    case "batchindex":
                        stopWatch.Restart();
                        if (!BatchIndex(splittedCommand))
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
                        if (!Map(splittedCommand))
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


        private bool Index(string[] args)
        {
            if (args.Length != 2)
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Missing arguments."));
                return false;
            }

            if (!Load(args[1])) return false;

            ExecutionReport exeReport = di3B.Add(Repository.parsedSample.peaks);
            Herald.Announce(Herald.MessageType.None,
                /*-*/ String.Format("Indexed #i: {0,9}     ET: {1,6}     Speed: {2,10}",
                /*0*/ String.Format("{0:N0}", exeReport.count),
                /*1*/ exeReport.ET,
                /*2*/ String.Format("{0:N0} #i\\sec", Math.Round(exeReport.count / exeReport.ET.TotalSeconds, 2))));
            return true;
        }
        private bool BatchIndex(string[] args)
        {
            DirectoryInfo dirInfo;
            if (args.Length == 2 && args[1].Length > 2 && args[1][0] == '*' && args[1][1] == '.')
            {
                dirInfo = new DirectoryInfo(_workingDirectory);
            }
            else if (args.Length == 3)
            {
                Uri logFileURI = null;
                if (!ValidateURI(args[2], out logFileURI)) return false;
                dirInfo = new DirectoryInfo(logFileURI.AbsolutePath);
            }
            else
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid arguments."));
                return false;
            }

            /// Check validity of the extension
            string extension = args[1].Substring(2, args[1].Length - 2);
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
                if (extension.Contains(c))
                {
                    Herald.Announce(Herald.MessageType.Error, "Invalid extension.");
                    return false;
                }

            FileInfo[] determinedFiles = dirInfo.GetFiles("*." + extension);

            if (determinedFiles.Length == 0)
            {
                Herald.Announce(Herald.MessageType.Warrning, String.Format("No file with \"{0}\" extension found!", extension));
                return false;
            }

            int i = 0;
            foreach (FileInfo fileInfo in determinedFiles)
            {
                Herald.Announce(
                    Herald.MessageType.Info,
                    String.Format(
                    /*-*/ "[{0}\\{1}] {2}",
                    /*0*/ ++i,
                    /*1*/ determinedFiles.Length,
                    /*2*/ Path.GetFileNameWithoutExtension(fileInfo.FullName)));

                Index(new string[] { null, fileInfo.FullName });
            }

            return true;
        }
        private bool Load(string fileName)
        {
            if (!ValidateFileName(fileName, out fileName)) return false;

            parserSTW.Restart();
            BEDParser<Peak, PeakData> bedParser = new BEDParser<Peak, PeakData>(fileName, AvailableGenomes.HomoSapiens, AvailableAssemblies.hm19);
            Repository.parsedSample = bedParser.Parse();
            parserSTW.Stop();

            Herald.Announce(Herald.MessageType.None,
                /*-*/ String.Format("Parsed  #i: {0,9}     ET: {1,6}     Speed: {2,10}",
                /*0*/ String.Format("{0:N0}", Repository.parsedSample.peaksCount),
                /*1*/ parserSTW.Elapsed,
                /*2*/ String.Format("{0:N0} #i\\sec", Math.Round(Repository.parsedSample.peaksCount / parserSTW.Elapsed.TotalSeconds, 2))));

            return true;
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

            if (!Char.TryParse(args[1], out strand))
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid strand parameter."));
                return false;
            }
            if (!Byte.TryParse(args[2], out minAcc))
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
            if (!String2Aggregate(args[4], out agg)) return false;

            switch (coverORsummit)
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
            if (args.Length != 4)
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Missing arguments."));
                return false;
            }

            char strand = '*';
            if (!Char.TryParse(args[2], out strand))
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid strand argument [{0}].", args[2]));
                return false;
            }

            Aggregate agg = Aggregate.Count;
            if (!String2Aggregate(args[3], out agg)) return false;

            if (!Load(args[1])) return false;
            di3B.Map(strand, Repository.parsedSample.peaks, agg);
            return true;
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
                    Herald.Announce(Herald.MessageType.Error, String.Format("Invalid aggregate parameter."));
                    return false;

            }
        }

        private bool ValidateURI(string strUri, out Uri uri)
        {
            uri = null;
            if (Uri.TryCreate(strUri, UriKind.Absolute, out uri) == false)
            {
                Herald.Announce(Herald.MessageType.Error, "Invalid path URI format.");
                return false;
            }
            if (!Directory.Exists(uri.AbsolutePath))
            {
                Herald.Announce(Herald.MessageType.Error, "Path does not exist.");
                return false;
            }
            return true;
        }
        private bool ValidateFileName(string iFileName, out string oFileName)
        {
            Uri fileUri = null;
            oFileName = iFileName;
            if (Path.GetDirectoryName(iFileName).Trim() == "")
                oFileName = _workingDirectory + Path.DirectorySeparatorChar + iFileName;
            else if (!ValidateURI(Path.GetDirectoryName(iFileName), out fileUri))
                return false;

            if (!File.Exists(oFileName))
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("File not found [{0}]", oFileName));
                return false;
            }
            return true;
        }
    }
}
