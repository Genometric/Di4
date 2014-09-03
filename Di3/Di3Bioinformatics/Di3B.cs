using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI3;
using IInterval;
using ICPMD;


namespace Di3Bioinformatics
{
    /// <summary>
    /// Dynamic intervals inverted index for Bioinformatics (Di3B).
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="M"></typeparam>
    public class Di3B<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : ICPMetadata<C>, IMetaData<C>
    {
        public Di3B(byte chrCount)
        {
            genome = new Genome<C, I, M>(chrCount);
        }

        

        Genome<C, I, M> genome { set; get; }


        public void Add(List<List<I>> peaks)
        {
            genome.Add(peaks);
        }

        public FunctionOutput<Output<C>> Cover(char strand, byte minAcc, byte maxAcc, string aggregate)
        {
            return genome.CoverSummit("cover", strand, minAcc, maxAcc, aggregate);
        }

        public FunctionOutput<Output<C>> Summit(char strand, byte minAcc, byte maxAcc, string aggregate)
        {
            return genome.CoverSummit("summit", strand, minAcc, maxAcc, aggregate);
        }
    }
}
