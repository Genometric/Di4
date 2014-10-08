using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using DI3;
using System.IO;

namespace Di3B
{
    public class Genome<C, I, M> 
        where C : IComparable<C>
        where I : IInterval<C, M>, new()
        where M : IMetaData<C>, new()
    {
        public Genome(string FilePath, ISerializer<C> CSerializer, IComparer<C> CComparer)
        {
            this.CSerializer = CSerializer;
            this.CComparer = CComparer;
            FileName = FilePath + Path.DirectorySeparatorChar + "Di3";
            Chrs = new Dictionary<string, Chromosome<C, I, M>>();
        }


        int cpuCount = Environment.ProcessorCount;

        private ISerializer<C> CSerializer { set; get; }
        private IComparer<C> CComparer { set; get; }
        private string FileName { set; get; }


        /// <summary>
        /// Sets and Gets all chromosomes of the genome.
        /// </summary>
        internal Dictionary<string, Chromosome<C, I, M>> Chrs { set; get; }

        internal void Add(Dictionary<string, List<I>> peaks)
        {
            int counter = 0;
            foreach (var chr in peaks)
            {
                if (!Chrs.ContainsKey(chr.Key))
                    Chrs.Add(chr.Key, new Chromosome<C, I, M>(FileName + chr.Key + ".indx", CSerializer, CComparer));

                foreach (var peak in chr.Value)
                {
                    Chrs[chr.Key].di3US.Add(Clone(peak));
                    Console.Write("\r Added: {0} - {1:N0}", chr.Key, counter++);
                }
            }
        }

        internal FunctionOutput<Output<C, I, M>> CoverSummit(string function, char strand, byte minAcc, byte maxAcc, string aggregate)
        {
            FunctionOutput<Output<C, I, M>> output = new FunctionOutput<Output<C, I, M>>(Chrs.Count);

            AggregateFactory<C, I, M> aggFactory = new AggregateFactory<C, I, M>();

            foreach (var chrKey in Chrs)
            {
                switch (strand)
                {
                    case '+':
                        if (chrKey.Value.di3PS.blockCount > 0)
                        {
                            switch (function)
                            {
                                case "cover":
                                    output.Chrs[chrKey.Key].outputPS =
                                        Chrs[chrKey.Key].di3PS.Cover<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;

                                case "summit":
                                    output.Chrs[chrKey.Key].outputPS =
                                        Chrs[chrKey.Key].di3PS.Summit<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;
                            }
                        }
                        break;

                    case '-':
                        if (Chrs[chrKey.Key].di3NS.blockCount > 0)
                        {
                            switch (function)
                            {
                                case "cover":
                                    output.Chrs[chrKey.Key].outputNS =
                                        Chrs[chrKey.Key].di3NS.Cover<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;

                                case "summit":
                                    output.Chrs[chrKey.Key].outputNS =
                                        Chrs[chrKey.Key].di3NS.Summit<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;
                            }
                        }
                        break;

                    case '*':
                        if (Chrs[chrKey.Key].di3US.blockCount > 0)
                        {
                            switch (function)
                            {
                                case "cover":
                                    output.Chrs[chrKey.Key].outputUS =
                                        Chrs[chrKey.Key].di3US.Cover<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;

                                case "summit":
                                    output.Chrs[chrKey.Key].outputUS =
                                        Chrs[chrKey.Key].di3US.Summit<Output<C, I, M>>(aggFactory.GetAggregateFunction(aggregate), minAcc, maxAcc);
                                    break;
                            }
                        }
                        break;
                }
            }

            return output;
        }

        internal FunctionOutput<Output<C, I, M>> Map(Dictionary<string, List<I>> references, char strand, string aggregate)
        {
            FunctionOutput<Output<C, I, M>> output = new FunctionOutput<Output<C, I, M>>(references.Count);

            AggregateFactory<C, I, M> aggFactory = new AggregateFactory<C, I, M>();

            foreach (var reference in references)
            {
                if (!Chrs.ContainsKey(reference.Key))
                    Chrs.Add(reference.Key, new Chromosome<C, I, M>(FileName + reference.Key + ".indx", CSerializer, CComparer));
                if (!output.Chrs.ContainsKey(reference.Key))
                    output.Chrs.Add(reference.Key, new FunctionOutput<Output<C, I, M>>.Chromosome());

                switch (strand)
                {
                    case '+':
                        //if (chr.Value.di3PS.blockCount > 0)
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
                }
            }

            return output;
        }



        private I Clone(I i)
        {
            return new I()
            {
                left = i.left,
                right = i.right,
                metadata = new M()
                {
                    left = i.metadata.left,
                    right = i.metadata.right,
                    hashKey = i.metadata.hashKey,
                    value = i.metadata.value,
                    name = i.metadata.name
                }
            };
        }













        
        
    }
}
