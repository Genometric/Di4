using IGenomics;

namespace Di3BCLI
{
    public class LightPeak : IInterval<int, LightPeakData>
    {
        /// <summary>
        /// Sets and gets the left-end of the interval.
        /// </summary>
        public int left { set; get; }

        /// <summary>
        /// Sets and gets the right-end of the interval.
        /// </summary>
        public int right { set; get; }

        /// <summary>
        /// Sets and gets the descriptive metadata
        /// of the interval. It could be a refChr
        /// to a _memory object, or a pointer, or 
        /// an entry ID on database, or etc. 
        /// </summary>
        public LightPeakData metadata { set; get; }

        public uint hashKey { set; get; }
    }
}
