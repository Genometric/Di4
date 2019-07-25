using Genometric.Di4.Di4B;
using Genometric.Di4.Di4B.Logging;
using Polimi.DEIB.VahidJalili.GIFP;
using Genometric.DataSim;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Genometric.Di4.CLI
{
    public class Orchestrator
    {
        public Orchestrator()
        {
            _maxDegreeOfParallelism = new MaxDegreeOfParallelism(Environment.ProcessorCount / 2, 2);
            _workingDirectory = UserConfig.workingDirectory;
            _logFile = UserConfig.logFile;
            _sectionTitle = "Chromosome";

            int32Comparer = new Int32Comparer();
            samplesHashtable = new Dictionary<int, uint>();
            _stopWatch = new Stopwatch();
            _parserSTW = new Stopwatch();
            _commitSTW = new Stopwatch();
            _indexSTW = new Stopwatch();

            _cacheOptions = new CacheOptions(
                CacheMaximumHistory: UserConfig.maxCacheSize, //163840,//81920,
                CacheMinimumHistory: UserConfig.minCacheSize, //20240,
                CacheKeepAliveTimeOut: 600000,
                CachePolicy: CSharpTest.Net.Collections.CachePolicy.Recent);

            di4B = new Di4B<int, Peak, PeakData>(_workingDirectory, _sectionTitle, UserConfig.memory, HDDPerformance.Fastest, _cacheOptions, PrimitiveSerializer.Int32, int32Comparer);
        }

        /// <summary>
        /// Sets and gets the total number of indexed intervals.
        /// </summary>
        private int _tN2i { set; get; }

        private double _accumulatedLoadET { set; get; }
        private Mux _mux { set; get; }
        private MaxDegreeOfParallelism _maxDegreeOfParallelism { set; get; }
        private string _workingDirectory { set; get; }
        private string _logFile { set; get; }
        private string _sectionTitle { set; get; }
        private Stopwatch _stopWatch { set; get; }
        private Stopwatch _parserSTW { set; get; }
        private Stopwatch _indexSTW { set; get; }
        private Stopwatch _commitSTW { set; get; }
        private IndexingMode _indexingMode { set; get; }
        private CacheOptions _cacheOptions { set; get; }
        Di4B<int, Peak, PeakData> di4B { set; get; }
        Int32Comparer int32Comparer { set; get; }
        Dictionary<int, uint> samplesHashtable { set; get; }

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

                case "commit":
                    _stopWatch.Restart();
                    if (!Commit())
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

                    Herald.Announce(Herald.MessageType.Info, string.Format("{0,29}: {1}", "Average indexing speed", Math.Round(_tN2i / _stopWatch.Elapsed.TotalSeconds, 2) + "  #i\\sec"));
                    break;


                case "statistics":
                    _stopWatch.Restart();
                    if (!Statistics(splittedCommand))
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    _stopWatch.Stop();

                    Herald.Announce(Herald.MessageType.Info, string.Format("{0,29}: {1}", "XYZ", Math.Round(_tN2i / _stopWatch.Elapsed.TotalSeconds, 2) + "  #i\\sec"));
                    break;


                case "2pass": // 2nd pass of indexing.
                    _stopWatch.Restart();
                    if (!Index2ndPass())
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    break;

                case "2ri": // 2nd resolution index
                    _stopWatch.Restart();
                    if (!Index2ndResolution(splittedCommand))
                    {
                        _stopWatch.Stop();
                        return false;
                    }
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

                case "va":
                case "varianta":
                case "vanalysis":
                case "variantanalysis":
                    _stopWatch.Restart();
                    if (!VariantAnalysis(splittedCommand))
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
                    ReportStats(splittedCommand);
                    return false;                

                case "dichotomies":
                    _stopWatch.Restart();
                    if(!Dichotomies(splittedCommand))
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    break;

                case "blockinfo":
                    _stopWatch.Restart();
                    if (!BlockInfoDis(splittedCommand))
                    {
                        _stopWatch.Stop();
                        return false;
                    }
                    break;

                case "gsimdata":
                    _stopWatch.Restart();
                    if(!GSimData(splittedCommand))
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

                case "benchmark":
                    _stopWatch.Restart();
                    if (!Benchmark(splittedCommand))
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
            Herald.Announce(Herald.MessageType.Success, string.Format("     Overall ET: {0}", _stopWatch.Elapsed.ToString()));
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
                chrColumn: UserConfig.ParserParameters.chrColumn,
                leftEndColumn: UserConfig.ParserParameters.leftEndColumn,
                rightEndColumn: UserConfig.ParserParameters.rightEndColumn,
                summitColumn: -1,
                nameColumn: UserConfig.ParserParameters.nameColumn,
                valueColumn: UserConfig.ParserParameters.valueColumn,
                strandColumn: -1,
                defaultValue: 0.01,
                pValueFormat: pValueFormat.minus10_Log10_pValue,
                dropPeakIfInvalidValue: false,
                hashFunction: HashFunction.FNV);

            try { Repository.parsedSample = bedParser.Parse(); }
            catch (Exception e)
            {
                if (Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar == _workingDirectory &&
                    Path.GetExtension(fileName) == _logFile)
                    Herald.Announce(Herald.MessageType.Error, string.Format("The requested extension should not have same extension as the log file."));
                else
                    Herald.Announce(Herald.MessageType.Error, string.Format("{0}", e.Message));
                return false;
            }
            _parserSTW.Stop();

            _accumulatedLoadET += _parserSTW.Elapsed.TotalSeconds;
            Herald.AnnounceExeReport("Loaded", new ExecutionReport(Repository.parsedSample.intervalsCount, _parserSTW.Elapsed));

            return true;
        }
        private bool tmpLoadVCF(string fileName)
        {
            if (!ValidateFileName(fileName, out fileName)) return false;

            _parserSTW.Restart();

            VCFParser<Variant, VariantData> vcfParser = new VCFParser<Variant, VariantData>(source: fileName, species: Genomes.HomoSapiens, assembly: Assemblies.hg19);

            try { Repository.parsedVariants = vcfParser.Parse(); }
            catch (Exception e)
            {
                if (Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar == _workingDirectory &&
                    Path.GetExtension(fileName) == _logFile)
                    Herald.Announce(Herald.MessageType.Error, string.Format("The requested extension should not have same extension as the log file."));
                else
                    Herald.Announce(Herald.MessageType.Error, string.Format("{0}", e.Message));
                return false;
            }
            _parserSTW.Stop();

            _accumulatedLoadET += _parserSTW.Elapsed.TotalSeconds;
            Herald.AnnounceExeReport("Loaded", new ExecutionReport(Repository.parsedVariants.intervalsCount, _parserSTW.Elapsed));

            return true;
        }
        private bool Index(string[] args)
        {
            if (args.Length != 2)
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Missing arguments."));
                return false;
            }

            if (Path.GetExtension(args[1]) == ".vcf")
            {
                if (!tmpLoadVCF(args[1])) return false;

                var tmp = new Dictionary<string, Dictionary<char, List<Peak>>>();
                foreach (var chr in Repository.parsedVariants.intervals)
                {
                    tmp.Add(chr.Key, new Dictionary<char, List<Peak>>());
                    foreach (var strand in chr.Value)
                    {
                        tmp[chr.Key].Add(strand.Key, new List<Peak>());
                        foreach (var peak in strand.Value)
                            tmp[chr.Key][strand.Key].Add(new Peak() { left = peak.left, right = peak.right, hashKey = peak.hashKey });
                    }
                }
                

                var report = di4B.Add(Repository.parsedVariants.fileHashKey, tmp, _indexingMode, _maxDegreeOfParallelism);
                Herald.AnnounceExeReport("Indexed", report);
                _tN2i += report.count;
            }
            else
            {
                if (!Load(args[1])) return false;
                var report = di4B.Add(Repository.parsedSample.fileHashKey, Repository.parsedSample.intervals, _indexingMode, _maxDegreeOfParallelism);
                Herald.AnnounceExeReport("Indexed", report);
                _tN2i += report.count;
            }
            
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
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
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

            FileInfo[] foundFiles = dirInfo.GetFiles("*." + extension);

            if (foundFiles.Length == 0)
            {
                Herald.Announce(Herald.MessageType.Warrning, string.Format("No file with \"{0}\" extension found!", extension));
                return false;
            }

            int i = 0;
            _indexSTW.Restart();
            foreach (FileInfo fileInfo in foundFiles)
            {
                Herald.Announce(
                    Herald.MessageType.Info,
                    string.Format(
                    /*-*/ "[{0}\\{1}] {2}",
                    /*0*/ ++i,
                    /*1*/ foundFiles.Length,
                    /*2*/ Path.GetFileNameWithoutExtension(fileInfo.FullName)));
                
                if (!Index(new string[] { null, fileInfo.FullName })) return false;
            }
            _indexSTW.Stop();

            _commitSTW.Restart();
            Commit();
            _commitSTW.Stop();

            Herald.Announce(Herald.MessageType.Info, string.Format("{0,29}: {1:N0}", "#indexed intervals", _tN2i));
            Herald.Announce(Herald.MessageType.Info, string.Format("{0,29}: {1}", "Load ET (sec)", _accumulatedLoadET));
            Herald.Announce(Herald.MessageType.Info, string.Format("{0,29}: {1}", "Index ET (sec)", _indexSTW.Elapsed.TotalSeconds.ToString()));
            Herald.Announce(Herald.MessageType.Info, string.Format("{0,29}: {1}", "Commit ET (sec)", _commitSTW.Elapsed.TotalSeconds.ToString()));

            return true;
        }
        private bool Index2ndPass()
        {
            Herald.AnnounceExeReport("2ndPass", di4B.Add2ndPass(_maxDegreeOfParallelism), speedUnit: Herald.SpeedUnit.bookmarkPerSecond);
            return true;
        }
        public bool Index2ndResolution(string[] args)
        {
            if(args.Length < 2)
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                return false;
            }

            int binCount = 0;
            CuttingMethod cuttingMethod = CuttingMethod.ZeroThresholding;
            switch(args[1].ToLower())
            {
                case "zt":
                case "zerothresholding":
                    cuttingMethod = CuttingMethod.ZeroThresholding;
                    break;

                case "uq":
                case "uniformscalarquantization":
                    cuttingMethod = CuttingMethod.UniformScalarQuantization;
                    if (args.Length != 3)
                    {
                        Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                        return false;
                    }
                    if (!int.TryParse(args[2], out binCount))
                    {
                        Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                        return false;
                    }
                    break;

                case "nuq":
                case "nonuniformscalarquantization":
                    cuttingMethod = CuttingMethod.NonUniformScalarQuantization;
                    if (args.Length != 3)
                    {
                        Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                        return false;
                    }
                    if (!int.TryParse(args[2], out binCount))
                    {
                        Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                        return false;
                    }
                    break;

                default:
                    Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                    return false;
            }

            Herald.AnnounceExeReport("2RIndex", di4B.SecondResolutionIndex(cuttingMethod, binCount, _maxDegreeOfParallelism), speedUnit: Herald.SpeedUnit.blockPerSecond);
            return true;
        }
        private bool Commit()
        {
            di4B.CommitIndexedData(_maxDegreeOfParallelism);
            return true;
        }


        
        
        public bool Cover(string[] args)
        {
            if (args.Length < 6)
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Missing parameter."));
                return false;
            }

            char strand;
            int minAcc, maxAcc;
            string coverOrSummit = args[0].ToLower();

            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            if (!char.TryParse(args[2], out strand))
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid strand parameter."));
                return false;
            }
            if (!int.TryParse(args[3], out minAcc))
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid minimum accumulation parameter."));
                return false;
            }
            if (!int.TryParse(args[4], out maxAcc))
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid maximum accumulation parameter."));
                return false;
            }

            Aggregate agg = Aggregate.Count;
            if (!String2Aggregate(args[5], out agg)) return false;

            FunctionOutput<Output<int, Peak, PeakData>> result = null;
            switch (coverOrSummit)
            {
                case "cover":
                    Herald.AnnounceExeReport("Cover", di4B.Cover(CoverVariation.Cover, strand, minAcc, maxAcc, agg, out result, _maxDegreeOfParallelism), Herald.SpeedUnit.bookmarkPerSecond);
                    break;

                case "summit":
                    Herald.AnnounceExeReport("Summit", di4B.Cover(CoverVariation.Summit, strand, minAcc, maxAcc, agg, out result, _maxDegreeOfParallelism), Herald.SpeedUnit.bookmarkPerSecond);
                    break;
            }

            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, result, "chr\tleft\tright\tcount\tstrand"));

            return true;
        }
        private bool Map(string[] args)
        {
            if (args.Length != 5)
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                return false;
            }

            string resultFile = "";
            if (!ExtractResultsFile(args[2], out resultFile)) return false; // invalid file URI.

            char strand = '*';
            if (!char.TryParse(args[3], out strand))
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid strand argument [{0}].", args[3]));
                return false;
            }

            Aggregate agg = Aggregate.Count;
            if (!String2Aggregate(args[4], out agg)) return false;

            if (!Load(args[1])) return false;

            FunctionOutput<Output<int, Peak, PeakData>> result;
            Herald.AnnounceExeReport("Map", di4B.Map(strand, Repository.parsedSample.intervals, agg, out result, _maxDegreeOfParallelism));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, result, "chr\tleft\tright\tcount\tstrand"));

            return true;
        }

        private bool VariantAnalysis(string[] args)
        {
            if (args.Length != 3)
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                return false;
            }

            string resultFile = "";
            if (!ExtractResultsFile(args[2], out resultFile)) return false; // invalid file URI.



            if (!tmpLoadVCF(args[1])) return false;

            var tmp = new Dictionary<string, Dictionary<char, List<Peak>>>();
            foreach (var chr in Repository.parsedVariants.intervals)
            {
                tmp.Add(chr.Key, new Dictionary<char, List<Peak>>());
                foreach (var s in chr.Value)
                {
                    tmp[chr.Key].Add(s.Key, new List<Peak>());
                    foreach (var peak in s.Value)
                        tmp[chr.Key][s.Key].Add(new Peak() { left = peak.left, right = peak.right, hashKey = peak.hashKey });
                }
            }

            FunctionOutput<Output<int, Peak, PeakData>> result;
            Dictionary<uint, int> newRes = new Dictionary<uint, int>();
            Herald.AnnounceExeReport("Variant Analysis", di4B.VariantAnalysis('*', tmp, Aggregate.Count, out result, _maxDegreeOfParallelism, out newRes));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, result, "chr\tleft\tright\tcount\tstrand"));

            var sortedRes = newRes.ToList();
            sortedRes.Sort(
                delegate (KeyValuePair<uint, int> i, KeyValuePair<uint, int> j)
                {
                    return j.Value.CompareTo(i.Value);
                });


            using (StreamWriter writer = new StreamWriter(resultFile))
            {
                writer.WriteLine("Sampled_ID\tVariations_in_common_with_reference");
                foreach (var item in sortedRes)
                    writer.WriteLine("{0}\t{1}", item.Key, item.Value);
            }


            return true;
        }

        private bool Statistics(string[] args)
        {
            if (args.Length != 2)
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                return false;
            }

            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.


            SortedDictionary<int, int> result = new SortedDictionary<int, int>();
            Herald.AnnounceExeReport("Statistics", di4B.LambdaSizeStats(out result));
            
            using (StreamWriter writer = new StreamWriter(resultFile))
            {
                writer.WriteLine("Lambda Size\tCount");
                foreach (var item in result)
                    writer.WriteLine("{0}\t{1}", item.Key, item.Value);
            }




            return true;
        }


        private bool SetIndexingMode(string[] args)
        {
            if (args.Length < 2)
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Missing argument."));
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
                    Herald.Announce(Herald.MessageType.Error, string.Format("Incorrect argument."));
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
            if (args.Length != 2 && args.Length != 3)
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                return false;
            }

            if (args.Length == 3 && !ParseMux(args[2])) return false;

            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            ConcurrentDictionary<string, ConcurrentDictionary<char, List<AccEntry<int>>>> results;
            Herald.AnnounceExeReport("AccHistogram", di4B.AccumulationHistogram(out results, _maxDegreeOfParallelism));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, results, "chr\tleft\tright\taccumulation\tstrand", mux: _mux));
            return true;
        }
        private bool AccumulationDistribution(string[] args)
        {
            if (args.Length != 2)
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                return false;
            }

            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            ConcurrentDictionary<string, ConcurrentDictionary<char, SortedDictionary<int, int>>> results;
            SortedDictionary<int, int> mergedResults;
            Herald.AnnounceExeReport("AccDistribution", di4B.AccumulationDistribution(out results, out mergedResults, _maxDegreeOfParallelism));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, results, mergedResults, "chr\tstrand\taccumulation\tcount"));
            return true;
        }
        
        private bool Merge(string[] args)
        {
            if(args.Length != 2)
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                return false;
            }
            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<int>>>> results = null;
            Herald.AnnounceExeReport("Merge", di4B.Merge(out results, _maxDegreeOfParallelism));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, results));
            return true;
        }
        private bool Complement(string[] args)
        {
            if (args.Length != 2)
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                return false;
            }
            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<int>>>> results = null;
            Herald.AnnounceExeReport("Complement", di4B.Complement(out results, _maxDegreeOfParallelism));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, results));
            return true;
        }
        private bool Dichotomies(string[] args)
        {
            if(args.Length!= 2)
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                return false;
            }
            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<int>>>> results = null;
            Herald.AnnounceExeReport("Dichotomies", di4B.Dichotomies(out results, _maxDegreeOfParallelism));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, results));
            return true;
        }
        private bool BlockInfoDis(string[] args)
        {
            if (args.Length != 2)
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                return false;
            }
            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            BlockInfoDis results = null;
            Herald.AnnounceExeReport("BlockInfo", di4B.BlocksInfoDistribution(out results, _maxDegreeOfParallelism));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, results));

            return true;
        }

        private bool GSimData(string[] args)
        {
            int _sCount = 0, _iCount = 0, _chrCount = 0, _kk = 0, _klambda = 0, _lambdak = 0, _lambdalambda = 0, _fileSizeProb = 0;

            if (args.Length != 9 ||
                !int.TryParse(args[1], out _sCount) ||
                !int.TryParse(args[2], out _iCount) ||
                !int.TryParse(args[3], out _chrCount) ||
                !int.TryParse(args[4], out _kk) ||
                !int.TryParse(args[5], out _klambda) ||
                !int.TryParse(args[6], out _lambdak) ||
                !int.TryParse(args[7], out _lambdalambda) ||
                !int.TryParse(args[8], out _fileSizeProb))
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                return false;
            }            

            var simData = new Di4DataSim(_workingDirectory);
            simData.Generate(_sCount, _iCount, _chrCount, new ErlangDistribution(_kk, _klambda), new ErlangDistribution(_lambdak, _lambdalambda), _fileSizeProb);
            return true;
        }

        private bool Benchmark(string[] args)
        {
            if(args.Length == 2)
            {
                if (args[1].ToLower() == "cover")
                    BenchmarkCover();
                else if (args[1].ToLower() == "acchis")
                    BenchmarkAccHis();
                return true;
            }
            else
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Missing argument."));
                return false;
            }
        }
        private void BenchmarkCover()
        {
            int tries = 10;
            var agg = Aggregate.Count;
            FunctionOutput<Output<int, Peak, PeakData>> result = null;
            var combinations = new List<int[]>
            {
                new int[] { 1, 2 },
                new int[] { 5, 10 },
                new int[] { 10, 20 },
                new int[] { 50, 60 },
                new int[] { 80, 90 },
                new int[] { 100, 200 },
                new int[] { 200, 220 },
                new int[] { 300, 320 },
                new int[] { 400, 500 },
                new int[] { 550, 1000 },
                new int[] { 1, 1 },
                new int[] { 10, 10 },
                new int[] { 25, 25 },
                new int[] { 50, 50 },
                new int[] { 60, 60 },
                new int[] { 80, 80 },
                new int[] { 100, 100 },
                new int[] { 150, 150 },
                new int[] { 200, 200 },
                new int[] { 500, 500 },
            };

            foreach (var c in combinations)
                for (int i = 0; i < tries; i++)
                    Herald.AnnounceExeReport(c[0] + "," + c[1], di4B.Cover(CoverVariation.Cover, '*', c[0], c[1], agg, out result, _maxDegreeOfParallelism), Herald.SpeedUnit.bookmarkPerSecond);
        }
        private void BenchmarkAccHis()
        {
            int tries = 20;
            ConcurrentDictionary<string, ConcurrentDictionary<char, List<AccEntry<int>>>> results;
            for (int i = 0; i < tries; i++)
                Herald.AnnounceExeReport("Run_" + i, di4B.AccumulationHistogram(out results, _maxDegreeOfParallelism));
        }

        private bool GetPD()
        {
            Herald.Announce(
                Herald.MessageType.Info,
                string.Format("Maximum degree of parallelism: \nChr level = {0} {1} \nDi4 level = {2} {3}",
                _maxDegreeOfParallelism.chrDegree,
                (_maxDegreeOfParallelism.chrDegree > 1 ? "threads" : "thread"),
                _maxDegreeOfParallelism.di4Degree,
                (_maxDegreeOfParallelism.di4Degree > 1 ? "threads" : "thread")));
            return false;
        }
        private bool SetPD(string[] args)
        {
            int chrDP = 0, di4DP = 0;

            if (args.Length != 3 || !int.TryParse(args[1], out chrDP) || !int.TryParse(args[2], out di4DP))
            {
                Herald.Announce(Herald.MessageType.Error, "Invalid arguments");
                return false;
            }
            if(chrDP == 0)
            {
                Herald.Announce(Herald.MessageType.Error, "Invalid arguments; at least one thread for Chr level is required.");
                return false;
            }
            if (di4DP == 0)
            {
                Herald.Announce(Herald.MessageType.Error, "Invalid arguments; at least one thread for Di4 level is required.");
                return false;
            }

            _maxDegreeOfParallelism = new MaxDegreeOfParallelism(chrDP, di4DP);
            return GetPD();
        }
        private bool GetCI()
        {
            return true;
        }
        private bool SetCI(string[] args)
        {
            return true;
        }


        private bool ReportStats(string[] args)
        {
            if (args.Length != 2)
            {
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid arguments."));
                return false;
            }
            string resultFile = "";
            if (!ExtractResultsFile(args[1], out resultFile)) return false; // invalid file URI.

            SortedDictionary<string, SortedDictionary<char, Stats>> results = null;
            Herald.AnnounceExeReport("Statistics", di4B.Statistics(out results));
            Herald.AnnounceExeReport("Export", Exporter.Export(resultFile, results));
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
                Herald.Announce(Herald.MessageType.Error, string.Format("File not found [{0}]", oFileName));
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
                    Herald.Announce(Herald.MessageType.Error, string.Format("Invalid aggregate parameter."));
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
                Herald.Announce(Herald.MessageType.Error, string.Format("Invalid results file."));
                return false;
            }
            outputFileName = resultsFileURI.AbsolutePath;
            return true;
        }
        private bool ParseMux(string input)
        {
            input = input.ToLower().Trim();
            if (input == "join")
            {
                _mux = Mux.Join;
                return true;
            }
            if (input == "disjoin")
            {
                _mux = Mux.Disjoin;
                return true;
            }
            return false;
        }
    }
}
