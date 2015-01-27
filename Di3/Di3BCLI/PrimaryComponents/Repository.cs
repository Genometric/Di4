using System;
using System.Collections.Generic;
using BEDParser;

namespace Di3BCLI
{
    public static class Repository
    {
        public static ParsedBED<int, Peak, PeakData> parsedSample { set; get; }
    }
}
