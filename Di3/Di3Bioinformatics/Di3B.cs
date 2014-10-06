using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI3;
using DI3.Interfaces;
using IInterval;
using ICPMD;
using CSharpTest.Net.Serialization;


namespace Di3Bioinformatics
{
    /// <summary>
    /// Dynamic intervals inverted index for Bioinformatics (Di3B).
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="M"></typeparam>
    public class Di3B<C, I, M> : Genome<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>, new()
        where M : ICPMetadata<C>, IMetaData<C>, new()
    {
        public Di3B(ISerializer<C> CoordinateSerializer, IComparer<C> CoordinateComparer)
        {
            //genome = new Genome<C, I, M>(chrCount, CoordinateSerializer);

            coordinateSerializer = CoordinateSerializer;
            coordinateComparer = CoordinateComparer;
        }

        

        //Genome<C, I, M> genome { set; get; }


        public void Add(Dictionary<string, List<I>> peaks)
        {
            base.Add(peaks);
        }

        public FunctionOutput<Output<C>> Cover(char strand, byte minAcc, byte maxAcc, string aggregate)
        {
            return base.CoverSummit("cover", strand, minAcc, maxAcc, aggregate);
        }

        public FunctionOutput<Output<C>> Summit(char strand, byte minAcc, byte maxAcc, string aggregate)
        {
            return base.CoverSummit("summit", strand, minAcc, maxAcc, aggregate);
        }
    }
}
