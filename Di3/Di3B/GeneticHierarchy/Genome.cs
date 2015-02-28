using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using Polimi.DEIB.VahidJalili.DI3;
using Polimi.DEIB.VahidJalili.DI3.DI3B.Logging;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Polimi.DEIB.VahidJalili.DI3.DI3B
{
    public class Genome<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>, IFormattable, new()
        where M : IMetaData, IFormattable, new()
    {
        public Genome(string workingDirectory, string sectionTitle, Memory memory, HDDPerformance hddPerformance, ISerializer<C> CSerializer, IComparer<C> CComparer)
        {
            _memory = memory;
            _hddPerformance = hddPerformance;
            _CSerializer = CSerializer;
            _CComparer = CComparer;
            _sectionTitle = sectionTitle;

            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _settings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings;

            if (_memory == Memory.RAM)
            {
                chrs = new Dictionary<string, Dictionary<char, Di3<C, I, M>>>();
            }
            else if (_memory == Memory.HDD && _hddPerformance == HDDPerformance.Fastest)
            {
                chrs = new Dictionary<string, Dictionary<char, Di3<C, I, M>>>();

                _chrSection = (ChrSection)ConfigurationManager.GetSection(_sectionTitle);
                if (_chrSection == null) _chrSection = new ChrSection();
                ConfigurationManager.RefreshSection(_sectionTitle);

                foreach (ChrConfigElement element in _chrSection.genomeChrs)
                {
                    if (!chrs.ContainsKey(element.chr))
                        chrs.Add(element.chr, new Dictionary<char, Di3<C, I, M>>());

                    if (!chrs[element.chr].ContainsKey(element.strand))
                        chrs[element.chr].Add(element.strand, new Di3<C, I, M>(GetDi3Options(element.index)));
                }
            }
        }

        private string _sectionTitle { set; get; }
        private KeyValueConfigurationCollection _settings { set; get; }
        private ISerializer<C> _CSerializer { set; get; }
        private IComparer<C> _CComparer { set; get; }
        private Memory _memory { set; get; }
        private HDDPerformance _hddPerformance { set; get; }
        private ChrSection _chrSection { set; get; }
        private Configuration _config { set; get; }
        internal Dictionary<string, Dictionary<char, Di3<C, I, M>>> chrs { set; get; }


        internal ExecutionReport Add(Dictionary<string, Dictionary<char, List<I>>> peaks, char strand, IndexingMode indexinMode, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            Stopwatch stpWtch = new Stopwatch();
            int totalIntervals = 0;

            switch (_memory)
            {
                case Memory.HDD:
                    if (_chrSection == null) _chrSection = new ChrSection();
                    ConfigurationManager.RefreshSection(_sectionTitle);

                    switch (_hddPerformance)
                    {
                        /// this case is problematic, because other operations 
                        /// are not implemented as they can work this way.
                        case HDDPerformance.LeastMemory:
                            foreach (var chr in peaks)
                                foreach (var strandEntry in chr.Value)
                                    using (var di3 = new Di3<C, I, M>(GetDi3Options(GetDi3File(chr.Key, strand)))) // this might be wrong
                                    {
                                        stpWtch.Start();
                                        di3.Add(strandEntry.Value, indexinMode, maxDegreeOfParallelism.di3Degree);
                                        stpWtch.Stop();
                                        totalIntervals += strandEntry.Value.Count;
                                    }
                            break;

                        case HDDPerformance.Fastest:
                            /// Initialize inside a sequential foreach loop.
                            foreach(var chr in peaks)
                                foreach(var strandEntry in chr.Value)
                                {
                                    if (!chrs.ContainsKey(chr.Key)) chrs.Add(chr.Key, new Dictionary<char, Di3<C, I, M>>());
                                    if (!chrs[chr.Key].ContainsKey(strand)) chrs[chr.Key].Add(strand, new Di3<C, I, M>(GetDi3Options(GetDi3File(chr.Key, strand))));
                                }

                            /// Populate inside a parallel foreach loop.
                            Parallel.ForEach(peaks,
                                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
                                chr =>
                                {
                                    foreach (var strandEntry in chr.Value)
                                    {
                                        stpWtch.Start();
                                        chrs[chr.Key][strand].Add(strandEntry.Value, indexinMode, maxDegreeOfParallelism.di3Degree);
                                        chrs[chr.Key][strand].Commit();
                                        stpWtch.Stop();
                                        totalIntervals += strandEntry.Value.Count;
                                    }
                                });
                            break;
                    }


                    if (_config.Sections[_sectionTitle] == null)
                        _config.Sections.Add(_sectionTitle, _chrSection);

                    _config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(_sectionTitle);
                    break;

                case Memory.RAM:
                    foreach (var chr in peaks)
                        foreach (var strandEntry in chr.Value)
                        {
                            if (!chrs.ContainsKey(chr.Key)) chrs.Add(chr.Key, new Dictionary<char, Di3<C, I, M>>());
                            if (!chrs[chr.Key].ContainsKey(strand)) chrs[chr.Key].Add(strand, new Di3<C, I, M>(GetDi3Options()));

                            stpWtch.Start();
                            chrs[chr.Key][strand].Add(strandEntry.Value, indexinMode, maxDegreeOfParallelism.di3Degree);
                            // Check if it's needed to call Commit().
                            stpWtch.Stop();
                            totalIntervals += strandEntry.Value.Count;
                        }
                    break;
            }

            return new ExecutionReport(totalIntervals, stpWtch.Elapsed);
        }
        internal ExecutionReport Add2ndPass(MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            Stopwatch stpWtch = new Stopwatch();
            int totalIntervals = 0;
            Parallel.ForEach(chrs, //var chr in chrs
               new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
               chr =>
               {
                   foreach (var sDi3 in chr.Value)
                   {
                       stpWtch.Start();
                       totalIntervals += sDi3.Value.bookmarkCount;
                       sDi3.Value.SecondPass();
                       sDi3.Value.Commit();
                       stpWtch.Stop();
                   }
               });
            return new ExecutionReport(totalIntervals, stpWtch.Elapsed);
        }

        internal ExecutionReport Cover(CoverVariation coverVariation, char strand, int minAcc, int maxAcc, Aggregate aggregate, out FunctionOutput<Output<C, I, M>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            Stopwatch stpWtch = new Stopwatch();
            int totalBookmarks = 0;
            var tmpResult = new FunctionOutput<Output<C, I, M>>();

            foreach (var chr in chrs)
                foreach (var sDi3 in chr.Value)
                {
                    if (!tmpResult.Chrs.ContainsKey(chr.Key)) tmpResult.Chrs.TryAdd(chr.Key, new ConcurrentDictionary<char, List<Output<C, I, M>>>());
                    if (!tmpResult.Chrs[chr.Key].ContainsKey(sDi3.Key)) tmpResult.Chrs[chr.Key].TryAdd(sDi3.Key, new List<Output<C, I, M>>());
                }

            stpWtch.Start();
            Parallel.ForEach(chrs,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
                chr =>
                {
                    ICSOutput<C, I, M, Output<C, I, M>> outputStrategy = new AggregateFactory<C, I, M>().GetAggregateFunction(aggregate);

                    foreach (var sDi3 in chr.Value)
                    {
                        totalBookmarks += sDi3.Value.bookmarkCount;
                        switch (coverVariation)
                        {
                            case CoverVariation.Cover:
                                sDi3.Value.Cover<Output<C, I, M>>(outputStrategy, minAcc, maxAcc, maxDegreeOfParallelism.di3Degree);
                                break;

                            case CoverVariation.Summit:
                                sDi3.Value.Summit<Output<C, I, M>>(outputStrategy, minAcc, maxAcc, maxDegreeOfParallelism.di3Degree);
                                break;
                        }

                        tmpResult.Chrs[chr.Key][sDi3.Key] = outputStrategy.output;
                    }
                });
            stpWtch.Stop();

            result = tmpResult;
            return new ExecutionReport(totalBookmarks, stpWtch.Elapsed);
        }

        internal ExecutionReport Map(Dictionary<string, Dictionary<char, List<I>>> references, char strand, Aggregate aggregate, out FunctionOutput<Output<C, I, M>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            Stopwatch stpWtch = new Stopwatch();
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

            stpWtch.Start();
            Parallel.ForEach(references,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
                refChr =>
                {
                    ICSOutput<C, I, M, Output<C, I, M>> outputStrategy = new AggregateFactory<C, I, M>().GetAggregateFunction(aggregate);

                    if (chrs.ContainsKey(refChr.Key))
                        foreach (var refStrand in refChr.Value)
                        {
                            if (!chrs[refChr.Key].ContainsKey(refStrand.Key)) continue;
                            chrs[refChr.Key][refStrand.Key].Map<Output<C, I, M>>(ref outputStrategy, refStrand.Value, maxDegreeOfParallelism.di3Degree);
                            tmpResults.Chrs[refChr.Key][refStrand.Key] = outputStrategy.output;
                            totalIntervals += refStrand.Value.Count;
                        }
                });
            stpWtch.Stop();

            result = tmpResults;
            return new ExecutionReport(totalIntervals, stpWtch.Elapsed);
        }

        internal ExecutionReport AccumulationHistogram(out ConcurrentDictionary<string, ConcurrentDictionary<char, List<AccEntry<C>>>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            Stopwatch stpWtch = new Stopwatch();
            var tmpResult = new ConcurrentDictionary<string, ConcurrentDictionary<char, List<AccEntry<C>>>>();

            foreach (var chr in chrs)
                foreach (var sDi3 in chr.Value)
                {
                    if (!tmpResult.ContainsKey(chr.Key)) tmpResult.TryAdd(chr.Key, new ConcurrentDictionary<char, List<AccEntry<C>>>());
                    if (!tmpResult[chr.Key].ContainsKey(sDi3.Key)) tmpResult[chr.Key].TryAdd(sDi3.Key, null);
                }

            Parallel.ForEach(chrs,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
                chr =>
                {
                    foreach (var sDi3 in chr.Value)
                    {
                        stpWtch.Start();
                        tmpResult[chr.Key][sDi3.Key] = sDi3.Value.AccumulationHistogram(maxDegreeOfParallelism.di3Degree);
                        stpWtch.Stop();
                    }
                });

            result = tmpResult;
            return new ExecutionReport(1, stpWtch.Elapsed);
        }

        internal ExecutionReport AccumulationDistribution(out ConcurrentDictionary<string, ConcurrentDictionary<char, SortedDictionary<int, int>>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            Stopwatch stpWtch = new Stopwatch();
            var tmpResult = new ConcurrentDictionary<string, ConcurrentDictionary<char, SortedDictionary<int, int>>>();

            foreach (var chr in chrs)
                foreach (var sDi3 in chr.Value)
                {
                    if (!tmpResult.ContainsKey(chr.Key)) tmpResult.TryAdd(chr.Key, new ConcurrentDictionary<char, SortedDictionary<int, int>>());
                    if (!tmpResult[chr.Key].ContainsKey(sDi3.Key)) tmpResult[chr.Key].TryAdd(sDi3.Key, null);
                }

            Parallel.ForEach(chrs,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
                chr =>
                {
                    foreach (var sDi3 in chr.Value)
                    {
                        stpWtch.Start();
                        tmpResult[chr.Key][sDi3.Key] = sDi3.Value.AccumulationDistribution(maxDegreeOfParallelism.di3Degree);
                        stpWtch.Stop();
                    }
                });

            result = tmpResult;
            return new ExecutionReport(1, stpWtch.Elapsed);
        }

        internal ExecutionReport SecondResolutionIndex(MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            Stopwatch stpWtch = new Stopwatch();
            int totalBookmarks = 0;

            Parallel.ForEach(chrs,
               new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
               chr =>
               {
                   foreach (var sDi3 in chr.Value)
                   {
                       stpWtch.Start();
                       sDi3.Value.SecondResolutionIndex(maxDegreeOfParallelism.di3Degree);
                       totalBookmarks += sDi3.Value.blockCount;
                       sDi3.Value.Commit();
                       stpWtch.Stop();
                   }
               });

            return new ExecutionReport(totalBookmarks, stpWtch.Elapsed);
        }

        internal ExecutionReport Merge(out ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<C>>>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            Stopwatch stpWtch = new Stopwatch();
            result = new ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<C>>>>();

            foreach (var chr in chrs)
                foreach (var sDi3 in chr.Value)
                {
                    if (!result.ContainsKey(chr.Key)) result.TryAdd(chr.Key, new ConcurrentDictionary<char, ICollection<BlockKey<C>>>());
                    if (!result[chr.Key].ContainsKey(sDi3.Key)) result[chr.Key].TryAdd(sDi3.Key, null); // is null correct here?
                    stpWtch.Start();
                    result[chr.Key][sDi3.Key] = sDi3.Value.Merge(maxDegreeOfParallelism.di3Degree);
                    stpWtch.Stop();
                }

            return new ExecutionReport(1, stpWtch.Elapsed);
        }

        internal ExecutionReport Complement(out ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<C>>>> result, MaxDegreeOfParallelism maxDegreeOfParallelism)
        {
            Stopwatch stpWtch = new Stopwatch();
            var tmpResult = new ConcurrentDictionary<string, ConcurrentDictionary<char, ICollection<BlockKey<C>>>>();

            foreach (var chr in chrs)
                foreach (var sDi3 in chr.Value)
                {
                    if (!tmpResult.ContainsKey(chr.Key)) tmpResult.TryAdd(chr.Key, new ConcurrentDictionary<char, ICollection<BlockKey<C>>>());
                    if (!tmpResult[chr.Key].ContainsKey(sDi3.Key)) tmpResult[chr.Key].TryAdd(sDi3.Key, null);
                }

            Parallel.ForEach(chrs,
               new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.chrDegree },
               chr =>
               {
                   foreach (var sDi3 in chr.Value)
                   {
                       stpWtch.Start();
                       tmpResult[chr.Key][sDi3.Key] = sDi3.Value.Complement(maxDegreeOfParallelism.di3Degree);
                       stpWtch.Stop();
                   }
               });

            result = tmpResult;
            return new ExecutionReport(1, stpWtch.Elapsed);
        }


        private string GetDi3File(string chr, char strand)
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
            if (_settings[chr] == null) _settings.Add(chr, _settings["WorkingDirectory"].Value + "Di3" + chr + s);
            else _settings[chr].Value = _settings["WorkingDirectory"].Value + "Di3" + chr + s;
            _chrSection.genomeChrs.Add(new ChrConfigElement(Chr: chr, Strand: strand, Index: _settings[chr].Value));

            /// There might be better way to wipe-out the default currentValue, even in different place with different strategy; 
            /// however, this method was the simplest I found and is a possible target of cleaning code.
            var initialDataIndex = _chrSection.genomeChrs.IndexOf(new ChrConfigElement(Chr: "Initial", Index: "Initial", Strand: '*'));
            if (initialDataIndex != -1) _chrSection.genomeChrs.RemoveAt(initialDataIndex);

            return _settings[chr].Value;
        }
        private Di3Options<C> GetDi3Options(string indexFile = "none")
        {
            Di3Options<C> options;

            switch (_memory)
            {
                case Memory.HDD:
                    options = new Di3Options<C>(
                        FileName: indexFile,
                        CreatePolicy: CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                        CSerializer: _CSerializer,
                        Comparer: _CComparer);
                    break;

                case Memory.RAM:
                default:
                    options = new Di3Options<C>(
                        CreatePolicy: CSharpTest.Net.Collections.CreatePolicy.Never,
                        CSerializer: _CSerializer,
                        Comparer: _CComparer);
                    break;
            }

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.Recent;
            options.CacheKeepAliveTimeOut = 60000;
            options.CacheMinimumHistory = 10240;
            options.CacheMaximumHistory = 40960;

            options.StoragePerformance = StoragePerformance.Fastest;

            return options;
        }
    }
}
