using System;
using DI3;
using Interfaces;

namespace Di3B
{
    class AggregateFactory<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>, new() // new is new
        where M : IMetaData<C>
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
