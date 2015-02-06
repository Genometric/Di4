using Di3B;
using Di3B.Logging;
using GIFP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Di3BCLI
{
    public class Orchestrator
    {
        public Orchestrator(string workingDirectory, string logFileExtension)
        {
            _workingDirectory = workingDirectory;
            _logFileExtension = logFileExtension;
            _sectionTitle = "Chromosome";

            int32Comparer = new Int32Comparer();
            samplesHashtable = new Dictionary<int, uint>();
            stopWatch = new Stopwatch();
            parserSTW = new Stopwatch();
            di3B = new Di3B<int, Peak, PeakData>(_workingDirectory, _sectionTitle, Memory.HDD, HDDPerformance.Fastest, PrimitiveSerializer.Int32, int32Comparer);
        }

        private string _workingDirectory { set; get; }
        private string _logFileExtension { set; get; }
        private string _sectionTitle { set; get; }
        private Stopwatch stopWatch { set; get; }
        private Stopwatch parserSTW { set; get; }
        Di3B<int, Peak, PeakData> di3B { set; get; }
        Int32Comparer int32Comparer { set; get; }
        Dictionary<int, UInt32> samplesHashtable { set; get; }

        internal bool CommandParse(string command)
        {
            Herald.Announce(Herald.MessageType.None, "> " + command, Herald.Destination.File);

            string[] splittedCommand = command.Split(' ');

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
                    if (!Cover(splittedCommand))
                    {
                        stopWatch.Stop();
                        return false;
                    }
                    break;

                case "map": // example: Map E:\refChr.bed * count
                    stopWatch.Restart();
                    if (!Map(splittedCommand))
                    {
                        stopWatch.Stop();
                        return false;
                    }
                    break;

                case "stats":
                    ReportStats();
                    return false;


                default:
                    Herald.Announce(Herald.MessageType.Error, "Unknown Command.");
                    return false;
            }


            stopWatch.Stop();
            Herald.Announce(Herald.MessageType.Success, String.Format("    Overall ET: {0}", stopWatch.Elapsed.ToString()));
            return false;
        }

        private void ReportStats()
        {
            ChrSection chrSection = (ChrSection)ConfigurationManager.GetSection(_sectionTitle);
            if (chrSection == null)
            {
                Herald.Announce(Herald.MessageType.Info, "Configuration does not contain any Di3 index.");
                return;
            }

            Herald.Announce(Herald.MessageType.Info, String.Format("Configuration contains {0,5} chromosomes as following:", chrSection.genomeChrs.Count));
            foreach (ChrConfigElement element in chrSection.genomeChrs)
                Herald.Announce(Herald.MessageType.Info, String.Format("Chromosome {0,5} is indexed in Di3 file: {1}", element.chr, element.index));
        }


        private bool Index(string[] args)
        {
            if (args.Length != 2)
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Missing arguments."));
                return false;
            }

            if (!Load(args[1])) return false;
            Herald.AnnounceExeReport("Indexed", di3B.Add(Repository.parsedSample.intervals));
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

            /// Checks the validity of the extension
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
            BEDParser<Peak, PeakData> bedParser = new BEDParser<Peak, PeakData>(fileName, Genomes.HomoSapiens, Assemblies.hm19, true);

            try { Repository.parsedSample = bedParser.Parse(); }
            catch (Exception e)
            {
                if (Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar == _workingDirectory &&
                    Path.GetExtension(fileName) == _logFileExtension)
                    Herald.Announce(Herald.MessageType.Error, String.Format("The requested extension should not have same extension as the log file."));
                else
                    Herald.Announce(Herald.MessageType.Error, String.Format("{0}", e.Message));
                return false;
            }
            parserSTW.Stop();

            Herald.AnnounceExeReport("Loaded", new ExecutionReport(Repository.parsedSample.intervalsCount, parserSTW.Elapsed));
            
            return true;
        }
        private bool Cover(string[] args)
        {
            if (args.Length < 6)
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Missing parameter."));
                return false;
            }

            char strand;
            byte minAcc, maxAcc;
            string coverOrSummit = args[0].ToLower();

            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            if (!Char.TryParse(args[2], out strand))
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid strand parameter."));
                return false;
            }
            if (!Byte.TryParse(args[3], out minAcc))
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid minimum accumulation parameter."));
                return false;
            }
            if (!Byte.TryParse(args[4], out maxAcc))
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid maximum accumulation parameter."));
                return false;
            }

            Aggregate agg = Aggregate.Count;
            if (!String2Aggregate(args[5], out agg)) return false;

            FunctionOutput<Output<int, Peak, PeakData>> result = null;
            switch (coverOrSummit)
            {
                case "cover":
                    Herald.AnnounceExeReport("Cover", di3B.Cover(CoverVariation.Cover, strand, minAcc, maxAcc, agg, out result), Herald.SpeedUnit.bookmarkPerSecond);
                    break;

                case "summit":
                    Herald.AnnounceExeReport("Summit", di3B.Cover(CoverVariation.Summit, strand, minAcc, maxAcc, agg, out result), Herald.SpeedUnit.bookmarkPerSecond);
                    break;
            }

            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, result));

            return true;
        }
        private bool Map(string[] args)
        {
            string resultFile = "";
            if (!ExtractResultsFile(args[2], out resultFile)) return false; // invalid file URI.

            if (args.Length != 5)
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Missing arguments."));
                return false;
            }

            char strand = '*';
            if (!Char.TryParse(args[3], out strand))
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid strand argument [{0}].", args[3]));
                return false;
            }

            Aggregate agg = Aggregate.Count;
            if (!String2Aggregate(args[4], out agg)) return false;

            if (!Load(args[1])) return false;

            FunctionOutput<Output<int, Peak, PeakData>> result;
            Herald.AnnounceExeReport("Map", di3B.Map(strand, Repository.parsedSample.intervals, agg, out result));

            return true;
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
        private bool ExtractResultsFile(string inputFileName, out string outputFileName)
        {
            Uri resultsFileURI = null;
            outputFileName = "";
            if (Path.GetDirectoryName(inputFileName).Trim() == "")
                inputFileName = _workingDirectory + Path.GetFileName(inputFileName);

            if (Uri.TryCreate(inputFileName, UriKind.Absolute, out resultsFileURI) == false ||
                    Path.GetFileName(resultsFileURI.AbsolutePath) == null ||
                    Path.GetFileName(resultsFileURI.AbsolutePath).Trim() == "")
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid results file."));
                return false;
            }
            outputFileName = resultsFileURI.AbsolutePath;
            return true;
        }
    }
}
