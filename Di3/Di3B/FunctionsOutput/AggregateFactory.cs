using System;
using DI3;
using Interfaces;

namespace Di3B
{
    public class AggregateFactory<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>, new()
        where M : IMetaData, new()
    {
        public ICSOutput<C, I, M, Output<C, I, M>> GetAggregateFunction(string aggregate)
        {
            switch (aggregate.ToLower())
            {
                case "count":
                    return new CSOutputCount<C, I, M>();
            }

            return null;
        }
    }
}
