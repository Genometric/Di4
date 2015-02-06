using DI3;
using IGenomics;
using System;

namespace Di3B
{
    public class AggregateFactory<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>, IFormattable, new()
        where M : IMetaData, IFormattable, new()
    {
        public ICSOutput<C, I, M, Output<C, I, M>> GetAggregateFunction(Aggregate aggregate)
        {
            switch (aggregate)
            {
                case Aggregate.Count:
                    return new CSOutputCount<C, I, M>();
            }

            return null;
        }
    }
}
