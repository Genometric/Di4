using System;
using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI4
{
    internal struct BlockValue
    {
        internal BlockValue(int MaxAccumulation, int IntervalCount)
            : this()
        {
            maxAccumulation = MaxAccumulation;
            intervalCount = IntervalCount;
        }

        internal int maxAccumulation { private set; get; }
        internal int intervalCount { private set; get; }
    }
}
