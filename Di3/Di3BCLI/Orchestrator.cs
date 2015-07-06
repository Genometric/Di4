using Polimi.DEIB.VahidJalili.DI3;
using Polimi.DEIB.VahidJalili.DI3.DI3B;
using Polimi.DEIB.VahidJalili.DI3.DI3B.Logging;
using Polimi.DEIB.VahidJalili.GIFP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace Polimi.DEIB.VahidJalili.DI3.CLI
{
    public class Orchestrator
    {
        public Orchestrator(string workingDirectory, string logFileExtension)
        {
            _maxDegreeOfParallelism = new MaxDegreeOfParallelism(Environment.ProcessorCount / 2, 2);
            _workingDirectory = workingDirectory;
            _logFileExtension = logFileExtension;
            _sectionTitle = "Chromosome";

            int32Comparer = new Int32Comparer();
            samplesHashtable = new Dictionary<int, uint>();
            _stopWatch = new Stopwatch();
            _parserSTW = new Stopwatch();

            _cacheOptions = new CacheOptions(
                CacheMaximumHistory: 163840,//81920,
                CacheMinimumHistory: 20240,
                CacheKeepAliveTimeOut: 600000,
                CachePolicy: CSharpTest.Net.Collections.CachePolicy.Recent);

            di3B = new Di3B<int, Peak, PeakData>(_workingDirectory, _sectionTitle, Memory.HDD, HDDPerformance.Fastest, _cacheOptions, PrimitiveSerializer.Int32, int32Comparer);
        }

        /// <summary>
        /// Sets and gets the total number of indexed intervals.
        /// </summary>
        private int _tN2i { set; get; }
        private MaxDegreeOfParallelism _maxDegreeOfParallelism { set; get; }
        private string _workingDirectory { set; get; }
        private string _logFileExtension { set; get; }
        private string _sectionTitle { set; get; }
        private Stopwatch _stopWatch { set; get; }
        private Stopwatch _parserSTW { set; get; }
        private IndexingMode _indexingMode { set; get; }
        private CacheOptions _cacheOptions { set; get; }
        Di3B<int, Peak, PeakData> di3B { set; get; }
        Int32Comparer int32Comparer { set; get; }
        Dictionary<int, UInt32> samplesHashtable { set; get; }

        internal bool CommandParse(string command)
        {
            Herald.Announce(Herald.MessageType.None, "> " + command, Herald.Destination.File);

            command = command.Trim();
            string[] splittedCommand = command.Split(' ');

            switch (splittedCommand[0].ToLower())
            {
                case "exit":
                    return true;

                case "index":
                    _stopWatch.Restart();
                    if (!Index(splittedCommand))
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    break;

                case "batchindex":
                    _stopWatch.Restart();
                    if (!BatchIndex(splittedCommand))
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    _stopWatch.Stop();
                    Herald.Announce(Herald.MessageType.Info, String.Format("Total number of indexed intervals: {0:N0} ", _tN2i));
                    Herald.Announce(Herald.MessageType.Info, String.Format("       Average indexing speed was: {0}", Math.Round(_tN2i / _stopWatch.Elapsed.TotalSeconds, 2) + "  #i\\sec"));
                    break;

                case "cover":
                case "summit":
                    _stopWatch.Restart();
                    if (!Cover(splittedCommand))
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    break;

                case "map": // example: Map E:\refChr.bed * count
                    _stopWatch.Restart();
                    if (!Map(splittedCommand))
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    break;

                case "merge":
                    _stopWatch.Restart();
                    if(!Merge(splittedCommand))
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    break;

                case "complement":
                    _stopWatch.Restart();
                    if (!Complement(splittedCommand))
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    break;

                case "stats":
                    ReportStats();
                    return false;

                case "2ri": // 2nd resolution index
                    _stopWatch.Restart();
                    if (!SecondResolutionIndex())
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    break;

                case "dichotomies":
                    _stopWatch.Restart();
                    if(!Dichotomies(splittedCommand))
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    break;


                case "getim": // get indexing mode.
                    return GetIndexingMode();

                case "setim": // set indexing mode.
                    return SetIndexingMode(splittedCommand);

                case "getdp": // get degree of parallelization
                case "getpd": // get parallelization degree
                    return GetPD();

                case "setdp": // set degree of parallelization
                case "setpd": // set parallelization degree
                    return SetPD(splittedCommand);

                case "getci": // get cache information
                    return GetCI();

                case "setci": // set cache information
                    return SetCI(splittedCommand);

                case "2pass": // 2nd pass of indexing.
                    _stopWatch.Restart();
                    if (!Index_2ndpass())
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    break;

                case "acchis": // get Accumulation Histogram.
                    _stopWatch.Restart();
                    if (!AccumulationHistogram(splittedCommand))
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    break;

                case "accdis": // get Accumulation Distribution.
                    _stopWatch.Restart();
                    if (!AccumulationDistribution(splittedCommand))
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    break;

                default:
                    Herald.Announce(Herald.MessageType.Error, "Unknown Command.");
                    return false;
            }


            _stopWatch.Stop();
            Herald.Announce(Herald.MessageType.Success, String.Format("    Overall ET: {0}", _stopWatch.Elapsed.ToString()));
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
            var report = di3B.Add(Repository.parsedSample.intervals, _indexingMode, _maxDegreeOfParallelism);
            Herald.AnnounceExeReport("Indexed", report);
            _tN2i += report.count;
            return true;
        }
        private bool BatchIndex(string[] args)
        {
            _tN2i = 0;
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
        private bool Index_2ndpass()
        {
            Herald.AnnounceExeReport("2nd-pass", di3B.Add2ndPass(_maxDegreeOfParallelism));
            return false;
        }
        private bool Load(string fileName)
        {
            if (!ValidateFileName(fileName, out fileName)) return false;

            _parserSTW.Restart();
            BEDParser<Peak, PeakData> bedParser = new BEDParser<Peak, PeakData>(
                source: fileName,
                species: Genomes.HomoSapiens,
                assembly: Assemblies.hg19,
                readOnlyValidChrs: true,
                startOffset: 0,
                chrColumn: 0,
                leftEndColumn: 1,//3,
                rightEndColumn: 2,//4,
                summitColumn: 3,//-1,
                nameColumn: 4,//8,
                valueColumn: 5,
                strandColumn: -1,
                defaultValue: 0.01,
                pValueFormat: pValueFormat.minus10_Log10_pValue,
                dropPeakIfInvalidValue: false);

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
            _parserSTW.Stop();

            Herald.AnnounceExeReport("Loaded", new ExecutionReport(Repository.parsedSample.intervalsCount, _parserSTW.Elapsed));

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
            int minAcc, maxAcc;
            string coverOrSummit = args[0].ToLower();

            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            if (!Char.TryParse(args[2], out strand))
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid strand parameter."));
                return false;
            }
            if (!int.TryParse(args[3], out minAcc))
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Invalid minimum accumulation parameter."));
                return false;
            }
            if (!int.TryParse(args[4], out maxAcc))
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
                    Herald.AnnounceExeReport("Cover", di3B.Cover(CoverVariation.Cover, strand, minAcc, maxAcc, agg, out result, _maxDegreeOfParallelism), Herald.SpeedUnit.bookmarkPerSecond);
                    break;

                case "summit":
                    Herald.AnnounceExeReport("Summit", di3B.Cover(CoverVariation.Summit, strand, minAcc, maxAcc, agg, out result, _maxDegreeOfParallelism), Herald.SpeedUnit.bookmarkPerSecond);
                    break;
            }

            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, result, "chr\tleft\tright\tcount\tstrand"));

            return true;
        }
        private bool Map(string[] args)
        {
            if (args.Length != 5)
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Missing arguments."));
                return false;
            }

            string resultFile = "";
            if (!ExtractResultsFile(args[2], out resultFile)) return false; // invalid file URI.

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
            Herald.AnnounceExeReport("Map", di3B.Map(strand, Repository.parsedSample.intervals, agg, out result, _maxDegreeOfParallelism));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, result, "chr\tleft\tright\tcount\tstrand"));

            return true;
        }
        private bool SecondResolutionIndex()
        {
            Herald.AnnounceExeReport("2R Index", di3B.SecondResolutionIndex(_maxDegreeOfParallelism));
            return true;
        }
        private bool SetIndexingMode(string[] args)
        {
            if (args.Length < 2)
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Missing argument."));
                return false;
            }

            switch (args[1].Trim().ToLower())
            {
                case "single":
                    _indexingMode = IndexingMode.SinglePass;
                    break;

                case "multi":
                    _indexingMode = IndexingMode.MultiPass;
                    break;

                default:
                    Herald.Announce(Herald.MessageType.Error, String.Format("Incorrect argument."));
                    break;
            }

            return GetIndexingMode();
        }
        private bool GetIndexingMode()
        {
            switch (_indexingMode)
            {
                case IndexingMode.SinglePass:
                    Console.WriteLine("Indexing mode is set to <Single-pass> indexing");
                    return false;

                case IndexingMode.MultiPass:
                    Console.WriteLine("Indexing mode is set to <Multi-pass>  indexing");
                    return false;

                default: return false;
            }
        }
        private bool AccumulationHistogram(string[] args)
        {
            if (args.Length != 2)
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Missing argument."));
                return false;
            }

            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            ConcurrentDictionary<string, ConcurrentDictionary<char, List<AccEntry<int>>>> results;
            Herald.AnnounceExeReport("AccHistogram", di3B.AccumulationHistogram(out results, _maxDegreeOfParallelism));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, results, "chr\tleft\tright\taccumulation\tstrand"));
            return true;
        }
        private bool AccumulationDistribution(string[] args)
        {
            if (args.Length != 2)
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Missing argument."));
                return false;
            }

            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            ConcurrentDictionary<string, ConcurrentDictionary<char, SortedDictionary<int, int>>> results;
            SortedDictionary<int, int> mergedResults;
            Herald.AnnounceExeReport("AccDistribution", di3B.AccumulationDistribution(out results, out mergedResults, _maxDegreeOfParallelism));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, results, mergedResults, "chr\tstrand\taccumulation\tcount"));
            return true;
        }
        private bool GetPD()
        {
            Herald.Announce(
                Herald.MessageType.Info,
                String.Format( "Maximum degree of parallelism: \nChr level = {0} threads \nDi3 level = {1} threads",
                _maxDegreeOfParallelism.chrDegree,
                _maxDegreeOfParallelism.di3Degree));
            return false;
        }
        private bool SetPD(string[] args)
        {
            int chrDP = 0, di3DP = 0;

            if (args.Length != 3 || !int.TryParse(args[1], out chrDP) || !int.TryParse(args[2], out di3DP))
            {
                Herald.Announce(Herald.MessageType.Error, "Invalid arguments");
                return false;
            }

            _maxDegreeOfParallelism = new MaxDegreeOfParallelism(chrDP, di3DP);
            return GetPD();
        }
        private bool Merge(string[] args)
        {
            if(args.Length != 2)
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Missing parameter."));
                return false;
            }
            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<int>>>> results = null;
            Herald.AnnounceExeReport("Merge", di3B.Merge(out results, _maxDegreeOfParallelism));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, results));
            return true;
        }
        private bool Complement(string[] args)
        {
            if (args.Length != 2)
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Missing parameter."));
                return false;
            }
            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<int>>>> results = null;
            Herald.AnnounceExeReport("Complement", di3B.Complement(out results, _maxDegreeOfParallelism));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, results));
            return true;
        }
        private bool Dichotomies(string[] args)
        {
            if(args.Length!= 2)
            {
                Herald.Announce(Herald.MessageType.Error, String.Format("Missing parameter."));
                return false;
            }
            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<int>>>> results = null;
            Herald.AnnounceExeReport("Dichotomies", di3B.Dichotomies(out results, _maxDegreeOfParallelism));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, results));
            return true;
        }
        private bool GetCI()
        {
            return true;
        }
        private bool SetCI(string[] args)
        {
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
