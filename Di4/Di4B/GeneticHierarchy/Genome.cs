using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using Polimi.DEIB.VahidJalili.DI4;
using Polimi.DEIB.VahidJalili.DI4.DI4B.Logging;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

namespace Polimi.DEIB.VahidJalili.DI4.DI4B
{
    public class Genome<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>, IFormattable, new()
        where M : IMetaData, IFormattable, new()
    {
        public Genome(
            string workingDirectory,
            string sectionTitle,
            Memory memory,
            HDDPerformance hddPerformance,
            IndexType indexType,
            CacheOptions cacheOptions,
            ISerializer<C> CSerializer,
            IComparer<C> CComparer)
        {
            _memory = memory;
            _hddPerformance = hddPerformance;
            _CSerializer = CSerializer;
            _CComparer = CComparer;
            _sectionTitle = sectionTitle;
            _cacheOptions = cacheOptions;
            _indexType = indexType;
            _stpWtch = new Stopwatch();

            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _settings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings;

            if (_memory == Memory.RAM)
            {
                chrs = new Dictionary<string, Dictionary<char, Di4<C, I, M>>>();
            }
            else if (_memory == Memory.HDD && _hddPerformance == HDDPerformance.Fastest)
            {
                chrs = new Dictionary<string, Dictionary<char, Di4<C, I, M>>>();

                _chrSection = (ChrSection)ConfigurationManager.GetSection(_sectionTitle);
                if (_chrSection == null) _chrSection = new ChrSection();
                ConfigurationManager.RefreshSection(_sectionTitle);

                foreach (ChrConfigElement element in _chrSection.genomeChrs)
                {
                    if (!chrs.ContainsKey(element.chr))
                        chrs.Add(element.chr, new Dictionary<char, Di4<C, I, M>>());

                    if (!chrs[element.chr].ContainsKey(element.strand))
                        chrs[element.chr].Add(element.strand, new Di4<C, I, M>(GetDi4Options(element.index)));
                }
            }
        }


        private string _sectionTitle { set; get; }
        private KeyValueConfigurationCollection _settings { set; get; }
        private ISerializer<C> _CSerializer { set; get; }
        private IComparer<C> _CComparer { set; get; }
        private Memory _memory { set; get; }
        private HDDPerformance _hddPerformance { set; get; }
        private IndexType _indexType { set; get; }
        private CacheOptions _cacheOptions { set; get; }
        private ChrSection _chrSection { set; get; }
        private Configuration _config { set; get; }
        private Stopwatch _stpWtch { set; get; }
        internal Dictionary<string, Dictionary<char, Di4<C, I, M>>> chrs { set; get; }


        internal ExecutionReport Add(
            Dictionary<string, Dictionary<char, List<I>>> intervals,
            char strand,
            IndexingMode indexinMode,
            MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            int totalIntervals = 0;

            switch (_memory)
            {
                case Memory.HDD:
                    if (_chrSection == null) _chrSection = new ChrSection();
                    ConfigurationManager.RefreshSection(_sectionTitle);

                    switch (_hddPerformance)
                    {
                        // TODO:
                        // This case is not complete, because other operations 
                        // are not supporting this method.
                        case HDDPerformance.LeastMemory:
                            _stpWtch.Restart();
                            foreach (var chr in intervals)
                                foreach (var strandEntry in chr.Value)
                                    using (var di4 = new Di4<C, I, M>(GetDi4Options(GetDi4File(chr.Key, strand)))) // this might be wrong
                                    {
                                        di4.Add(strandEntry.Value, indexinMode, maxDegreeOfParallelism.di4Degree);
                                        totalIntervals += strandEntry.Value.Count;
                                    }
                            _stpWtch.Stop();
                            break;

                        case HDDPerformance.Fastest:
                            /// Initialize by a sequential foreach loop.
                            foreach (var chr in intervals)
                                foreach (var strandEntry in chr.Value)
                                {
                                    if (!chrs.ContainsKey(chr.Key)) chrs.Add(chr.Key, new Dictionary<char, Di4<C, I, M>>());
                                    if (!chrs[chr.Key].ContainsKey(strand)) chrs[chr.Key].Add(strand, new Di4<C, I, M>(GetDi4Options(GetDi4File(chr.Key, strand))));
                                }

                            _stpWtch.Restart();
                            /// Populate inside a parallel foreach loop.
                            Parallel.ForEach(intervals,
                                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
                                chr =>
                                {
                                    foreach (var strandEntry in chr.Value)
                                    {
                                        chrs[chr.Key][strand].Add(strandEntry.Value, indexinMode, maxDegreeOfParallelism.di4Degree);
                                        //chrs[chr.Key][strand].Commit();
                                        totalIntervals += strandEntry.Value.Count;
                                    }
                                });
                            _stpWtch.Stop();
                            break;
                    }


                    if (_config.Sections[_sectionTitle] == null)
                        _config.Sections.Add(_sectionTitle, _chrSection);

                    _config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(_sectionTitle);
                    break;

                case Memory.RAM:
                    _stpWtch.Restart();
                    foreach (var chr in intervals)
                        foreach (var strandEntry in chr.Value)
                        {
                            if (!chrs.ContainsKey(chr.Key)) chrs.Add(chr.Key, new Dictionary<char, Di4<C, I, M>>());
                            if (!chrs[chr.Key].ContainsKey(strand)) chrs[chr.Key].Add(strand, new Di4<C, I, M>(GetDi4Options()));

                            chrs[chr.Key][strand].Add(strandEntry.Value, indexinMode, maxDegreeOfParallelism.di4Degree);
                            totalIntervals += strandEntry.Value.Count;
                        }
                    _stpWtch.Stop();
                    break;
            }

            return new ExecutionReport(totalIntervals, _stpWtch.Elapsed);
        }


        internal void CommitIndexedData(
            char strand,
            MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            Parallel.ForEach(chrs,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
                chr =>
                {
                    foreach (var strandEntry in chr.Value)
                    {
                        chrs[chr.Key][strand].Commit();
                    }
                });
        }


        internal ExecutionReport Add2ndPass(
            MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            int bookmarksCount = 0;

            _stpWtch.Restart();
            Parallel.ForEach(chrs, //var chr in chrs
               new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
               chr =>
               {
                   foreach (var sDi4 in chr.Value)
                   {
                       bookmarksCount += sDi4.Value.bookmarkCount;
                       sDi4.Value.SecondPass();
                       sDi4.Value.Commit();
                   }
               });
            _stpWtch.Stop();

            return new ExecutionReport(bookmarksCount, _stpWtch.Elapsed);
        }        


        internal ExecutionReport SecondResolutionIndex(MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            int blockCount = 0;

            _stpWtch.Restart();
            Parallel.ForEach(chrs,
               new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
               chr =>
               {
                   foreach (var sDi4 in chr.Value)
                   {
                       sDi4.Value.SecondResolutionIndex(maxDegreeOfParallelism.di4Degree);
                       blockCount += sDi4.Value.blockCount;
                       sDi4.Value.Commit();
                   }
               });
            _stpWtch.Stop();

            return new ExecutionReport(blockCount, _stpWtch.Elapsed); // TODO: check if blockCount += reduces speed or not
        }


        internal ExecutionReport Cover(
            CoverVariation coverVariation,
            char strand, int minAcc, int maxAcc, Aggregate aggregate,
            out FunctionOutput<Output<C, I, M>> result,
            MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            int totalBookmarks = 0;
            var tmpResult = new FunctionOutput<Output<C, I, M>>();

            foreach (var chr in chrs)
                foreach (var sDi4 in chr.Value)
                {
                    if (!tmpResult.Chrs.ContainsKey(chr.Key)) tmpResult.Chrs.TryAdd(chr.Key, new ConcurrentDictionary<char, List<Output<C, I, M>>>());
                    if (!tmpResult.Chrs[chr.Key].ContainsKey(sDi4.Key)) tmpResult.Chrs[chr.Key].TryAdd(sDi4.Key, new List<Output<C, I, M>>());
                }

            _stpWtch.Restart();
            Parallel.ForEach(chrs,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
                chr =>
                {
                    IOutput<C, I, M, Output<C, I, M>> outputStrategy = new AggregateFactory<C, I, M>().GetAggregateFunction(aggregate);

                    foreach (var sDi4 in chr.Value)
                    {
                        totalBookmarks += sDi4.Value.bookmarkCount;
                        switch (coverVariation)
                        {
                            case CoverVariation.Cover:
                                sDi4.Value.Cover<Output<C, I, M>>(outputStrategy, minAcc, maxAcc, maxDegreeOfParallelism.di4Degree);
                                break;

                            case CoverVariation.Summit:
                                sDi4.Value.Summit<Output<C, I, M>>(outputStrategy, minAcc, maxAcc, maxDegreeOfParallelism.di4Degree);
                                break;
                        }

                        tmpResult.Chrs[chr.Key][sDi4.Key] = outputStrategy.output;
                    }
                });
            _stpWtch.Stop();

            result = tmpResult;
            return new ExecutionReport(totalBookmarks, _stpWtch.Elapsed);
        }


        internal ExecutionReport Map(
            Dictionary<string, Dictionary<char, List<I>>> references,
            char strand, Aggregate aggregate,
            out FunctionOutput<Output<C, I, M>> result,
            MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            int totalIntervals = 0;

            var tmpResults = new FunctionOutput<Output<C, I, M>>();

            foreach (var refChr in references)
            {
                if (!chrs.ContainsKey(refChr.Key)) continue;
                foreach (var refStrand in refChr.Value)
                {
                    if (!chrs[refChr.Key].ContainsKey(refStrand.Key)) continue;
                    if (!tmpResults.Chrs.ContainsKey(refChr.Key)) tmpResults.Chrs.TryAdd(refChr.Key, new ConcurrentDictionary<char, List<Output<C, I, M>>>());
                    if (!tmpResults.Chrs[refChr.Key].ContainsKey(refStrand.Key)) tmpResults.Chrs[refChr.Key].TryAdd(refStrand.Key, new List<Output<C, I, M>>());
                }
            }

            _stpWtch.Restart();
            Parallel.ForEach(references,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
                refChr =>
                {
                    IOutput<C, I, M, Output<C, I, M>> outputStrategy = new AggregateFactory<C, I, M>().GetAggregateFunction(aggregate);

                    if (chrs.ContainsKey(refChr.Key))
                        foreach (var refStrand in refChr.Value)
                        {
                            if (!chrs[refChr.Key].ContainsKey(refStrand.Key)) continue;
                            chrs[refChr.Key][refStrand.Key].Map<Output<C, I, M>>(ref outputStrategy, refStrand.Value, maxDegreeOfParallelism.di4Degree);
                            tmpResults.Chrs[refChr.Key][refStrand.Key] = outputStrategy.output;
                            totalIntervals += refStrand.Value.Count;
                        }
                });
            _stpWtch.Stop();

            result = tmpResults;
            return new ExecutionReport(totalIntervals, _stpWtch.Elapsed);
        }


        internal ExecutionReport AccumulationHistogram(
            out ConcurrentDictionary<string, ConcurrentDictionary<char, List<AccEntry<C>>>> result,
            MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            var tmpResult = new ConcurrentDictionary<string, ConcurrentDictionary<char, List<AccEntry<C>>>>();

            foreach (var chr in chrs)
                foreach (var sDi4 in chr.Value)
                {
                    if (!tmpResult.ContainsKey(chr.Key)) tmpResult.TryAdd(chr.Key, new ConcurrentDictionary<char, List<AccEntry<C>>>());
                    if (!tmpResult[chr.Key].ContainsKey(sDi4.Key)) tmpResult[chr.Key].TryAdd(sDi4.Key, null);
                }

            _stpWtch.Restart();
            Parallel.ForEach(chrs,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
                chr =>
                {
                    foreach (var sDi4 in chr.Value)
                        tmpResult[chr.Key][sDi4.Key] = sDi4.Value.AccumulationHistogram(maxDegreeOfParallelism.di4Degree);
                });
            _stpWtch.Stop();
            result = tmpResult;
            return new ExecutionReport(1, _stpWtch.Elapsed);
        }


        internal ExecutionReport AccumulationDistribution(
            out ConcurrentDictionary<string, ConcurrentDictionary<char, SortedDictionary<int, int>>> result,
            out SortedDictionary<int, int> mergedResult,
            MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            mergedResult = new SortedDictionary<int, int>();
            var tmpResult = new ConcurrentDictionary<string, ConcurrentDictionary<char, SortedDictionary<int, int>>>();

            foreach (var chr in chrs)
                foreach (var sDi4 in chr.Value)
                {
                    if (!tmpResult.ContainsKey(chr.Key)) tmpResult.TryAdd(chr.Key, new ConcurrentDictionary<char, SortedDictionary<int, int>>());
                    if (!tmpResult[chr.Key].ContainsKey(sDi4.Key)) tmpResult[chr.Key].TryAdd(sDi4.Key, null);
                }

            _stpWtch.Restart();
            Parallel.ForEach(chrs,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
                chr =>
                {
                    foreach (var sDi4 in chr.Value)
                        tmpResult[chr.Key][sDi4.Key] = sDi4.Value.AccumulationDistribution(maxDegreeOfParallelism.di4Degree);
                });

            result = tmpResult;
            foreach (var chr in result)
                foreach (var strand in chr.Value)
                    foreach (var accumulation in strand.Value)
                        if (mergedResult.ContainsKey(accumulation.Key))
                            mergedResult[accumulation.Key] += accumulation.Value;
                        else
                            mergedResult.Add(accumulation.Key, accumulation.Value);

            _stpWtch.Stop();
            return new ExecutionReport(1, _stpWtch.Elapsed); // correct this
        }
        


        internal ExecutionReport Merge(
            out ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<C>>>> result,
            MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            result = new ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<C>>>>();

            _stpWtch.Restart();
            foreach (var chr in chrs)
                foreach (var sDi4 in chr.Value)
                {
                    if (!result.ContainsKey(chr.Key)) result.TryAdd(chr.Key, new ConcurrentDictionary<char, ICollection<BlockKey<C>>>());
                    if (!result[chr.Key].ContainsKey(sDi4.Key)) result[chr.Key].TryAdd(sDi4.Key, null); // is null correct here?

                    result[chr.Key][sDi4.Key] = sDi4.Value.Merge(maxDegreeOfParallelism.di4Degree);
                }

            _stpWtch.Stop();
            return new ExecutionReport(1, _stpWtch.Elapsed); // correct this
        }


        internal ExecutionReport Dichotomies(
            out ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<C>>>> result,
            MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            var tmpResults = new ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<C>>>>();
            //int bookmarkCount = 0;

            _stpWtch.Restart();
            Parallel.ForEach(chrs,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
                chr =>
                {
                    foreach (var sDi4 in chr.Value)
                    {
                        if (!tmpResults.ContainsKey(chr.Key)) tmpResults.TryAdd(chr.Key, new ConcurrentDictionary<char, ICollection<BlockKey<C>>>());
                        if (!tmpResults[chr.Key].ContainsKey(sDi4.Key)) tmpResults[chr.Key].TryAdd(sDi4.Key, null);

                        tmpResults[chr.Key][sDi4.Key] = sDi4.Value.Dichotomies();
                        //bookmarkCount += sDi4.Value.bookmarkCount;
                    }
                });

            _stpWtch.Stop();
            result = tmpResults;
            return new ExecutionReport(/*bookmarkCount*/1, _stpWtch.Elapsed); // correct this, check if bookmarkCount += reduces the speed or not.
        }


        internal ExecutionReport Complement(
            out ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<C>>>> result,
            MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            var tmpResult = new ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<C>>>>();

            foreach (var chr in chrs)
                foreach (var sDi4 in chr.Value)
                {
                    if (!tmpResult.ContainsKey(chr.Key)) tmpResult.TryAdd(chr.Key, new ConcurrentDictionary<char, ICollection<BlockKey<C>>>());
                    if (!tmpResult[chr.Key].ContainsKey(sDi4.Key)) tmpResult[chr.Key].TryAdd(sDi4.Key, null);
                }

            _stpWtch.Restart();
            Parallel.ForEach(chrs,
               new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
               chr =>
               {
                   foreach (var sDi4 in chr.Value)
                       tmpResult[chr.Key][sDi4.Key] = sDi4.Value.Complement(maxDegreeOfParallelism.di4Degree);
               });

            _stpWtch.Stop();
            result = tmpResult;
            return new ExecutionReport(1, _stpWtch.Elapsed);
        }


        internal ExecutionReport BlocksInfoDistribution(
            out BlockInfoDis result,
            MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            var tmpResults = new ConcurrentBag<BlockInfoDis>();

            _stpWtch.Restart();
            Parallel.ForEach(chrs,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
                chr =>
                {
                    foreach (var sDi4 in chr.Value)
                        tmpResults.Add(sDi4.Value.BlockInfoDistributions());
                });

            result = new BlockInfoDis();
            // TODO: update the following using Linq.
            foreach(var tmpRes in tmpResults)
            {
                foreach (var item in tmpRes.intervalCountDis)
                    if (result.intervalCountDis.ContainsKey(item.Key))
                        result.intervalCountDis[item.Key] += item.Value;
                    else
                        result.intervalCountDis.Add(item.Key, item.Value);

                foreach (var item in tmpRes.maxAccDis)
                    if (result.maxAccDis.ContainsKey(item.Key))
                        result.maxAccDis[item.Key] += item.Value;
                    else
                        result.maxAccDis.Add(item.Key, item.Value);
            }

            _stpWtch.Stop();
            return new ExecutionReport(1, _stpWtch.Elapsed);
        }


        private string GetDi4File(string chr, char strand)
        {
            string s = "";
            switch (strand)
            {
                case '+': s = "P"; break;
                case '-': s = "N"; break;
                case '*': s = "U"; break;
                default: s = "V"; break;
            }

            if (_memory == Memory.HDD)
                foreach (ChrConfigElement element in _chrSection.genomeChrs)
                    if (element.chr == chr)
                        return element.index;


            /// Following codes will be met in two conditions:
            /// 1. _memory = RAM 
            /// 2. _memory = HDD but no index for the defined chromosome is defined in _config file.
            if (_settings[chr] == null) _settings.Add(chr, _settings["WorkingDirectory"].Value + "Di4" + chr + s);
            else _settings[chr].Value = _settings["WorkingDirectory"].Value + "Di4" + chr + s;
            _chrSection.genomeChrs.Add(new ChrConfigElement(Chr: chr, Strand: strand, Index: _settings[chr].Value));

            /// There might be better way to wipe-out the default currentValue, even in different place with different strategy; 
            /// however, this method was the simplest I found and is a possible target of cleaning code.
            var initialDataIndex = _chrSection.genomeChrs.IndexOf(new ChrConfigElement(Chr: "Initial", Index: "Initial", Strand: '*'));
            if (initialDataIndex != -1) _chrSection.genomeChrs.RemoveAt(initialDataIndex);

            return _settings[chr].Value;
        }
        private Di4Options<C> GetDi4Options(string indexFile = "none")
        {
            Di4Options<C> options;

            switch (_memory)
            {
                case Memory.HDD:
                    options = new Di4Options<C>(
                        FileName: indexFile,
                        CreatePolicy: CreatePolicy.IfNeeded,
                        CSerializer: _CSerializer,
                        Comparer: _CComparer);
                    break;

                case Memory.RAM:
                default:
                    options = new Di4Options<C>(
                        CreatePolicy: CreatePolicy.Never,
                        CSerializer: _CSerializer,
                        Comparer: _CComparer);
                    break;
            }

            options.ActiveIndexes = _indexType;

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 4096;//512;//1024;//8192;

            options.cacheOptions = _cacheOptions;

            options.StoragePerformance = StoragePerformance.Fastest;

            return options;
        }
    }
}
