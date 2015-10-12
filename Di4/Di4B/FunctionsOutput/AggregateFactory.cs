using Polimi.DEIB.VahidJalili.DI4;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;

namespace Polimi.DEIB.VahidJalili.DI4.DI4B
{
    public class AggregateFactory<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>, IFormattable, new()
        where M : IMetaData, IFormattable, new()
    {
        public IOutput<C, I, M, Output<C, I, M>> GetAggregateFunction(Aggregate aggregate)
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
