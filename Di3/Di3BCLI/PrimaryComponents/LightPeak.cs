﻿using Polimi.DEIB.VahidJalili.IGenomics;

namespace Polimi.DEIB.VahidJalili.DI3.CLI
{
    public class LightPeak : IInterval<int, LightPeakData>
    {
        /// <summary>
        /// Sets and gets the left-end of the distribution.
        /// </summary>
        public int left { set; get; }

        /// <summary>
        /// Sets and gets the right-end of the distribution.
        /// </summary>
        public int right { set; get; }

        /// <summary>
        /// Sets and gets the descriptive metadata
        /// of the distribution. It could be a refChr
        /// to a _memory object, or a pointer, or 
        /// an entry ID on database, or etc. 
        /// </summary>
        public LightPeakData metadata { set; get; }

        public uint hashKey { set; get; }
    }
}
