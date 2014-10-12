using System;
using System.Collections.Generic;
using Interfaces;
using CSharpTest.Net.Serialization;


namespace Di3B
{
    public class Di3B<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>, new()
        where M : IMetaData<C>, new()
    {
        private ISerializer<C> CSerializer { set; get; }
        private IComparer<C> CComparer { set; get; }
        
        ////// ------- moved to genome.
        //private Dictionary<string, Di3<C, I, M>> Chrs { set; get; }

        private Genome<C, I, M> genome { set; get; }
        public Di3B(string Di3Path, Memory Memory, ISerializer<C> CSerializer, IComparer<C> CComparer)
        {
            this.CSerializer = CSerializer;
            this.CComparer = CComparer;
            genome = new Genome<C, I, M>(Di3Path, Memory, CSerializer, CComparer);


            ////// ------- moved to genome.
            //Chrs = new Dictionary<string, Di3<C, I, M>>();
        }
        public void Add(Dictionary<string, List<I>> peaks)
        {
            #region ////// ------- moved to genome.
            /*
            int counter = 0;
            foreach (var chr in peaks)
            {
                if (!Chrs.ContainsKey(chr.Key))
                    Chrs.Add(chr.Key, new Di3<C, I, M>(CSerializer, CComparer));

                foreach (var peak in chr.Value) // I create a new 'I' to avoid pass-by-reference.
                {
                    Chrs[chr.Key].Add(new I()
                    {
                        left = peak.left,
                        right = peak.right,
                        metadata = new M()
                        {
                            left = peak.metadata.left,
                            right = peak.metadata.right,
                            hashKey = peak.metadata.hashKey,
                            value = peak.metadata.value,
                            name = peak.metadata.name
                        }
                    });

                    Console.Write("\r Added: {0} - {1:N0}", chr.Key, counter++);
                }
            }*/
            #endregion


            genome.Add(peaks, '*');
        }


        public FunctionOutput<Output<C, I, M>> Cover(char strand, byte minAcc, byte maxAcc, string aggregate)
        {
            //return genome.CoverSummit("cover", strand, minAcc, maxAcc, aggregate);
            return genome.CoverSummit("cover", strand, minAcc, maxAcc, aggregate);
        }

        public FunctionOutput<Output<C, I, M>> Summit(char strand, byte minAcc, byte maxAcc, string aggregate)
        {
            return genome.CoverSummit("summit", strand, minAcc, maxAcc, aggregate);
        }

        public FunctionOutput<Output<C, I, M>> Map(char strand, Dictionary<string, List<I>> references, string aggregate)
        {
            return genome.Map(references, strand, aggregate);
        }
    }
}
