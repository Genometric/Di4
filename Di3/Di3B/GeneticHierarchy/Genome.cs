using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using DI3;
using Di3B.Logging;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace Di3B
{
    public class Genome<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>, new()
        where M : IMetaData, new()
    {
        public Genome(string workingDirectory, Memory memory, HDDPerformance hddPerformance, ISerializer<C> CSerializer, IComparer<C> CComparer)
        {
            _memory = memory;
            _hddPerformance = hddPerformance;
            _CSerializer = CSerializer;
            _CComparer = CComparer;
            _sectionName = "Chromosome";
            _cpuCount = Environment.ProcessorCount;

            /// if following commented-out section is not required, then the following code is neither necessary.
            //workingDirectory = workingDirectory + Path.DirectorySeparatorChar;


            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _settings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings;

            /// Previously, the following code was in-use. However, it seems to be redundant after what is done by the "ReadAndSetConfiguration" 
            /// function of Di3BCLI Program class.  
            //if (_settings["WorkingDirectory"] == null) _settings.Add("WorkingDirectory", workingDirectory);
            //else _settings["WorkingDirectory"].Value = workingDirectory;
            //if (!Directory.Exists(_settings["WorkingDirectory"].Value)) Directory.CreateDirectory(_settings["WorkingDirectory"].Value);

            if (_memory == Memory.RAM || (_memory == Memory.HDD && _hddPerformance == HDDPerformance.Fastest))
                chrs = new Dictionary<string, Dictionary<char, Di3<C, I, M>>>();
        }


        private string _sectionName { set; get; }
        private int _cpuCount { set; get; }
        private KeyValueConfigurationCollection _settings { set; get; }
        private ISerializer<C> _CSerializer { set; get; }
        private IComparer<C> _CComparer { set; get; }
        private Memory _memory { set; get; }
        private HDDPerformance _hddPerformance { set; get; }
        private ChrSection _chrSection { set; get; }
        private Configuration _config { set; get; }
        internal Dictionary<string, Dictionary<char, Di3<C, I, M>>> chrs { set; get; }



        internal ExecutionReport Add(Dictionary<string, List<I>> peaks, char strand)
        {
            Stopwatch stpWtch = new Stopwatch();
            int totalIntervals = 0;

            switch (_memory)
            {
                case Memory.HDD:
                    if (_chrSection == null) _chrSection = new ChrSection();
                    ConfigurationManager.RefreshSection(_sectionName);

                    switch (_hddPerformance)
                    {
                        case HDDPerformance.LeastMemory:
                            foreach (var chr in peaks)
                                using (var di3 = new Di3<C, I, M>(GetDi3Options(GetDi3File(chr.Key, strand))))
                                {
                                    stpWtch.Start();
                                    di3.Add(chr.Value, Mode.MultiPass);
                                    stpWtch.Stop();
                                    totalIntervals += chr.Value.Count;
                                }
                            break;

                        case HDDPerformance.Fastest:
                            foreach (var chr in peaks)
                            {
                                if (!chrs.ContainsKey(chr.Key))
                                    chrs.Add(chr.Key, new Dictionary<char, Di3<C, I, M>>());

                                if (!chrs[chr.Key].ContainsKey(strand))
                                    chrs[chr.Key].Add(strand, new Di3<C, I, M>(GetDi3Options(GetDi3File(chr.Key, strand))));

                                stpWtch.Start();
                                chrs[chr.Key][strand].Add(chr.Value, Mode.MultiPass);
                                stpWtch.Stop();
                                totalIntervals += chr.Value.Count;
                            }
                            break;
                    }
                    

                    if (_config.Sections[_sectionName] == null)
                        _config.Sections.Add(_sectionName, _chrSection);

                    _config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(_sectionName);
                    break;

                case Memory.RAM:
                    foreach (var chr in peaks)
                    {
                        if (!chrs.ContainsKey(chr.Key))
                            chrs.Add(chr.Key, new Dictionary<char, Di3<C, I, M>>());

                        // Previously was:
                        //chrs[chr.Key].Add(strand, new Di3<C, I, M>(GetDi3Options(GetDi3File(chr.Key, strand))));

                        // Now is:
                        if (!chrs[chr.Key].ContainsKey(strand))
                            chrs[chr.Key].Add(strand, new Di3<C, I, M>(GetDi3Options(GetDi3File(chr.Key, strand))));

                        stpWtch.Start();
                        chrs[chr.Key][strand].Add(chr.Value, Mode.MultiPass);
                        stpWtch.Stop();
                        totalIntervals += chr.Value.Count;
                    }
                    break;
            }

            return new ExecutionReport(totalIntervals, stpWtch.Elapsed);
        }


        internal FunctionOutput<Output<C, I, M>> Cover(CoverVariation coverVariation, char strand, byte minAcc, byte maxAcc, Aggregate aggregate)
        {
            FunctionOutput<Output<C, I, M>> output = new FunctionOutput<Output<C, I, M>>();

            switch (_memory)
            {
                case Memory.HDD:
                    _chrSection = (ChrSection)ConfigurationManager.GetSection(_sectionName);
                    if (_chrSection == null) _chrSection = new ChrSection();
                    ConfigurationManager.RefreshSection(_sectionName);

                    foreach (ChrConfigElement element in _chrSection.genomeChrs)
                    {
                        if (!output.Chrs.ContainsKey(element.chr)) output.Chrs.Add(element.chr, new Dictionary<char, List<Output<C, I, M>>>());
                        if (!output.Chrs[element.chr].ContainsKey(strand)) output.Chrs[element.chr].Add(strand, new List<Output<C, I, M>>());

                        // use options here
                        /*using (var _di3 = new Di3<C, I, M>(element.index, CreatePolicy.IfNeeded, CSerializer, CComparer))
                        {
                            output.chrs[element.chr][element.strand] = ExecuteCover(function, _di3, strand, minAcc, maxAcc, aggregate);
                        }*/
                    }

                    // string _sectionName = "Chromosome";                    
                    // var allChrs = _config.Sections[_sectionName];
                    break;


                case Memory.RAM:


                    foreach (var chr in chrs)
                    {
                        // use options here
                        //chrs[chr.Key].Add('*', new Di3<C, I, M>(GetDi3File(chr.Key, strand), CreatePolicy.Never, CSerializer, CComparer));
                        output.Chrs[chr.Key][strand] = ExecuteCover(coverVariation, chrs[chr.Key][strand], strand, minAcc, maxAcc, aggregate);
                    }
                    break;
            }

            return output;
        }
        private List<Output<C, I, M>> ExecuteCover(CoverVariation coverVariation, Di3<C, I, M> di3, char strand, byte minAcc, byte maxAcc, Aggregate aggregate)
        {
            AggregateFactory<C, I, M> aggFactory = new AggregateFactory<C, I, M>();

            switch (coverVariation)
            {
                case CoverVariation.Cover:
                    return di3.Cover<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);

                case CoverVariation.Summit:
                    return di3.Summit<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
            }

            return null;
        }


        internal FunctionOutput<Output<C, I, M>> Map(Dictionary<string, List<I>> references, char strand, Aggregate aggregate)
        {
            FunctionOutput<Output<C, I, M>> output = new FunctionOutput<Output<C, I, M>>();
            AggregateFactory<C, I, M> aggFactory = new AggregateFactory<C, I, M>();

            foreach (var reference in references)
            {
                if (!output.Chrs.ContainsKey(reference.Key))
                    output.Chrs.Add(reference.Key, new Dictionary<char, List<Output<C, I, M>>>());

                switch (_memory)
                {
                    case Memory.HDD:
                        _chrSection = (ChrSection)ConfigurationManager.GetSection(_sectionName);
                        if (_chrSection == null) _chrSection = new ChrSection();
                        ConfigurationManager.RefreshSection(_sectionName);

                        // use options here
                        /*using (var _di3 = new Di3<C, I, M>(GetDi3File(reference.Key, strand), CreatePolicy.IfNeeded, CSerializer, CComparer))
                        {
                            Console.WriteLine("... proecssing {0} now.", reference.Key);
                            output.chrs[reference.Key][strand] = _di3.Map<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), references[reference.Key]);
                        }*/
                        break;

                    case Memory.RAM:
                        if (!chrs.ContainsKey(reference.Key))
                            chrs.Add(reference.Key, new Dictionary<char, Di3<C, I, M>>());


                        // use options here
                        //chrs[reference.Key].Add(strand, new Di3<C, I, M>(GetDi3File(reference.Key, strand), CreatePolicy.Never, CSerializer, CComparer));
                        output.Chrs[reference.Key][strand] = chrs[reference.Key][strand].Map<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), references[reference.Key]);
                        break;
                }

                /*
                if (!chrs.ContainsKey(reference.Key))
                    chrs.Add(reference.Key, new Chromosome<C, I, M>(FileName + reference.Key + ".indx", CSerializer, CComparer));
                if (!output.chrs.ContainsKey(reference.Key))
                    output.chrs.Add(reference.Key, new FunctionOutput<Output<C, I, M>>.Chromosome());

                switch (strand)
                {
                    case '+':
                        //if (chr.newValue.di3PS.blockCount > 0)
                        //{
                            ///////////////// ----------------- there will be an error here, because this is reading chr title from chrs but searching 
                            ////////////////------------------- for it in references ! e.g., there might be chrM in chrs but not in refrences. 
                        output.chrs[reference.Key].outputPS =
                                chrs[reference.Key].di3PS.Map<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), references[reference.Key]);
                        //}
                        break;

                    case '-':
                        //if (chrs[chr.Key].di3NS.blockCount > 0)
                        //{
                            ///////////////// ----------------- there will be an error here, because this is reading chr title from chrs but searching 
                            ////////////////------------------- for it in references ! e.g., there might be chrM in chrs but not in refrences. 
                        output.chrs[reference.Key].outputNS =
                                chrs[reference.Key].di3NS.Map<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), references[reference.Key]);
                        //}
                        break;

                    case '*':
                        //if (chrs[chr.Key].di3US.blockCount > 0)
                        //{
                            ///////////////// ----------------- there will be an error here, because this is reading chr title from chrs but searching 
                            ////////////////------------------- for it in references ! e.g., there might be chrM in chrs but not in refrences. 
                        output.chrs[reference.Key].outputUS =
                                chrs[reference.Key].di3US.Map<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), references[reference.Key]);
                        //}
                        break;
                }*/
            }

            return output;
        }


        private string GetDi3File(string chr, char strand)
        {
            if (_memory == Memory.HDD)
                foreach (ChrConfigElement element in _chrSection.genomeChrs)
                    if (element.chr == chr)
                        return element.index;


            /// Following codes will be met in two conditions:
            /// 1. _memory = RAM 
            /// 2. _memory = HDD but no index for the defined chromosome is defined in _config file.
            if (_settings[chr] == null) _settings.Add(chr, _settings["WorkingDirectory"].Value + "Di3" + chr + ".indx");
            else _settings[chr].Value = _settings["WorkingDirectory"].Value + "Di3" + chr + ".indx";
            _chrSection.genomeChrs.Add(new ChrConfigElement(Chr: chr, Strand: strand, Index: _settings[chr].Value));

            /// There might be better way to wipe-out the default currentValue, even in different place with different strategy; 
            /// however, this method was the simplest I found and is a possible target of cleaning code.
            var initialDataIndex = _chrSection.genomeChrs.IndexOf(new ChrConfigElement(Chr: "Initial", Index: "Initial", Strand: '*'));
            if (initialDataIndex != -1) _chrSection.genomeChrs.RemoveAt(initialDataIndex);

            return _settings[chr].Value;
        }
        private Di3Options<C> GetDi3Options(string indexFile)
        {
            Di3Options<C> options;

            switch (_memory)
            {
                case Memory.HDD:
                    options = new Di3Options<C>(
                        indexFile,
                        CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                        _CSerializer, _CComparer);
                    break;

                case Memory.RAM:
                default:
                    options = new Di3Options<C>(
                        indexFile,
                        CSharpTest.Net.Collections.CreatePolicy.Never,
                        _CSerializer, _CComparer);
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
