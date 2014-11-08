using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using DI3;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Di3B
{
    public class Genome<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>, new()
        where M : IExtMetaData<C>, new()
    {
        public Genome(string Di3Path, Memory Memory, ISerializer<C> CSerializer, IComparer<C> CComparer)
        {
            this.memory = Memory;
            this.CSerializer = CSerializer;
            this.CComparer = CComparer;

            Di3Path = Di3Path + Path.DirectorySeparatorChar;
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            settings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings;
            if (settings["WorkingDirectory"] == null) settings.Add("WorkingDirectory", Di3Path);
            else settings["WorkingDirectory"].Value = Di3Path;

            if (!Directory.Exists(settings["WorkingDirectory"].Value)) Directory.CreateDirectory(settings["WorkingDirectory"].Value);

            if (memory == Memory.RAM)
                Chrs = new Dictionary<string, Dictionary<char, Di3<C, I, M>>>();
        }


        string sectionName = "Chromosome";
        int cpuCount = Environment.ProcessorCount;
        private KeyValueConfigurationCollection settings { set; get; }
        private ISerializer<C> CSerializer { set; get; }
        private IComparer<C> CComparer { set; get; }
        private Memory memory { set; get; }
        ChrSection chrSection { set; get; }
        Configuration config { set; get; }
        internal Dictionary<string, Dictionary<char, Di3<C, I, M>>> Chrs { set; get; }




        internal void Add(Dictionary<string, List<I>> peaks, char strand)
        {
            switch (memory)
            {
                case Memory.HDD:
                    if (chrSection == null) chrSection = new ChrSection();
                    ConfigurationManager.RefreshSection(sectionName);

                    foreach (var chr in peaks)                    
                        using (var di3 = new Di3<C, I, M>(GetDi3Options(GetDi3File(chr.Key, strand))))                        
                            di3.Add(chr.Value, Mode.MultiPass);

                    if (config.Sections[sectionName] == null)
                        config.Sections.Add(sectionName, chrSection);

                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(sectionName);
                    break;

                case Memory.RAM:
                    foreach (var chr in peaks)
                    {
                        if (!Chrs.ContainsKey(chr.Key))
                            Chrs.Add(chr.Key, new Dictionary<char, Di3<C, I, M>>());

                        Chrs[chr.Key].Add(strand, new Di3<C, I, M>(GetDi3Options(GetDi3File(chr.Key, strand))));
                        Chrs[chr.Key][strand].Add(chr.Value, Mode.MultiPass);
                    }
                    break;
            }
        }






        internal FunctionOutput<Output<C, I, M>> CoverSummit(string function, char strand, byte minAcc, byte maxAcc, string aggregate)
        {
            FunctionOutput<Output<C, I, M>> output = new FunctionOutput<Output<C, I, M>>();

            switch (memory)
            {
                case Memory.HDD:
                    chrSection = (ChrSection)ConfigurationManager.GetSection(sectionName);
                    if (chrSection == null) chrSection = new ChrSection();
                    ConfigurationManager.RefreshSection(sectionName);

                    foreach (ChrConfigElement element in chrSection.genomeChrs)
                    {
                        if (!output.Chrs.ContainsKey(element.chr)) output.Chrs.Add(element.chr, new Dictionary<char, List<Output<C, I, M>>>());
                        if (!output.Chrs[element.chr].ContainsKey(strand)) output.Chrs[element.chr].Add(strand, new List<Output<C, I, M>>());

                        // use options here
                        /*using (var _di3 = new Di3<C, I, M>(element.index, CreatePolicy.IfNeeded, CSerializer, CComparer))
                        {
                            output.Chrs[element.chr][element.strand] = ExecuteCoverSummit(function, _di3, strand, minAcc, maxAcc, aggregate);
                        }*/
                    }

                    // string sectionName = "Chromosome";                    
                    // var allChrs = config.Sections[sectionName];
                    break;


                case Memory.RAM:


                    foreach (var chr in Chrs)
                    {
                        // use options here
                        //Chrs[chr.Key].Add('*', new Di3<C, I, M>(GetDi3File(chr.Key, strand), CreatePolicy.Never, CSerializer, CComparer));
                        output.Chrs[chr.Key][strand] = ExecuteCoverSummit(function, Chrs[chr.Key][strand], strand, minAcc, maxAcc, aggregate);
                    }
                    break;
            }

            return output;
        }
        private List<Output<C, I, M>> ExecuteCoverSummit(string function, Di3<C, I, M> di3, char strand, byte minAcc, byte maxAcc, string aggregate)
        {
            AggregateFactory<C, I, M> aggFactory = new AggregateFactory<C, I, M>();

            switch (function)
            {
                case "cover":
                    return di3.Cover<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);

                case "summit":
                    return di3.Summit<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
            }

            return null;
        }






        internal FunctionOutput<Output<C, I, M>> Map(Dictionary<string, List<I>> references, char strand, string aggregate)
        {
            FunctionOutput<Output<C, I, M>> output = new FunctionOutput<Output<C, I, M>>();
            AggregateFactory<C, I, M> aggFactory = new AggregateFactory<C, I, M>();

            foreach (var reference in references)
            {
                if (!output.Chrs.ContainsKey(reference.Key))
                    output.Chrs.Add(reference.Key, new Dictionary<char, List<Output<C, I, M>>>());

                switch (memory)
                {
                    case Memory.HDD:
                        chrSection = (ChrSection)ConfigurationManager.GetSection(sectionName);
                        if (chrSection == null) chrSection = new ChrSection();
                        ConfigurationManager.RefreshSection(sectionName);

                        // use options here
                        /*using (var _di3 = new Di3<C, I, M>(GetDi3File(reference.Key, strand), CreatePolicy.IfNeeded, CSerializer, CComparer))
                        {
                            Console.WriteLine("... proecssing {0} now.", reference.Key);
                            output.Chrs[reference.Key][strand] = _di3.Map<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), references[reference.Key]);
                        }*/
                        break;

                    case Memory.RAM:
                        if (!Chrs.ContainsKey(reference.Key))
                            Chrs.Add(reference.Key, new Dictionary<char, Di3<C, I, M>>());


                        // use options here
                        //Chrs[reference.Key].Add(strand, new Di3<C, I, M>(GetDi3File(reference.Key, strand), CreatePolicy.Never, CSerializer, CComparer));
                        output.Chrs[reference.Key][strand] = Chrs[reference.Key][strand].Map<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), references[reference.Key]);
                        break;
                }

                /*
                if (!Chrs.ContainsKey(reference.Key))
                    Chrs.Add(reference.Key, new Chromosome<C, I, M>(FileName + reference.Key + ".indx", CSerializer, CComparer));
                if (!output.Chrs.ContainsKey(reference.Key))
                    output.Chrs.Add(reference.Key, new FunctionOutput<Output<C, I, M>>.Chromosome());

                switch (strand)
                {
                    case '+':
                        //if (chr.newValue.di3PS.blockCount > 0)
                        //{
                            ///////////////// ----------------- there will be an error here, because this is reading chr title from chrs but searching 
                            ////////////////------------------- for it in references ! e.g., there might be chrM in chrs but not in refrences. 
                        output.Chrs[reference.Key].outputPS =
                                Chrs[reference.Key].di3PS.Map<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), references[reference.Key]);
                        //}
                        break;

                    case '-':
                        //if (Chrs[chr.Key].di3NS.blockCount > 0)
                        //{
                            ///////////////// ----------------- there will be an error here, because this is reading chr title from chrs but searching 
                            ////////////////------------------- for it in references ! e.g., there might be chrM in chrs but not in refrences. 
                        output.Chrs[reference.Key].outputNS =
                                Chrs[reference.Key].di3NS.Map<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), references[reference.Key]);
                        //}
                        break;

                    case '*':
                        //if (Chrs[chr.Key].di3US.blockCount > 0)
                        //{
                            ///////////////// ----------------- there will be an error here, because this is reading chr title from chrs but searching 
                            ////////////////------------------- for it in references ! e.g., there might be chrM in chrs but not in refrences. 
                        output.Chrs[reference.Key].outputUS =
                                Chrs[reference.Key].di3US.Map<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), references[reference.Key]);
                        //}
                        break;
                }*/
            }

            return output;
        }


        private string GetDi3File(string chr, char strand)
        {
            if (memory == Memory.HDD)
                foreach (ChrConfigElement element in chrSection.genomeChrs)
                    if (element.chr == chr)
                        return element.index;


            /// Following codes will be met in two conditions:
            /// 1. memory = RAM 
            /// 2. memory = HDD but no index for the defined chromosome is defined in config file.
            if (settings[chr] == null) settings.Add(chr, settings["WorkingDirectory"].Value + "Di3" + chr + ".indx");
            else settings[chr].Value = settings["WorkingDirectory"].Value + "Di3" + chr + ".indx";
            chrSection.genomeChrs.Add(new ChrConfigElement(Chr: chr, Strand: strand, Index: settings[chr].Value));

            /// There might be better way to wipe-out the default currentValue, even in different place with different strategy; 
            /// however, this method was the simplest I found and is a possible target of cleaning code.
            var initialDataIndex = chrSection.genomeChrs.IndexOf(new ChrConfigElement(Chr: "Initial", Index: "Initial", Strand: '*'));
            if (initialDataIndex != -1) chrSection.genomeChrs.RemoveAt(initialDataIndex);

            return settings[chr].Value;
        }
        private Di3Options<C> GetDi3Options(string indexFile)
        {
            Di3Options<C> options = new Di3Options<C>(
                indexFile,
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                CSerializer, CComparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            return options;
        }
    }
}
