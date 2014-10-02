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
    class AggregateFactory<C, I, M>
        where C : IComparable<C>
        where I : IInterval<int, M>
        where M : IMetaData<int>
    {
        public ICSOutput<C,  M, Output<C>> GetAggregateFunction(string aggregate)
        {
            switch (aggregate.ToLower())
            {
                case "count":
                    return new CSOutputCount<C,  M>();
            }

            return null;
        }
    }
}
