using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Genometric.Di4
{
    internal class BlockValue
    {
        internal BlockValue(int BoundariesLowerBound, int BoundariesUpperBound, int IntervalCount, uint[] atI)
        {
            boundariesLowerBound = BoundariesLowerBound;
            boundariesUpperBound = BoundariesUpperBound;            
            intervalCount = IntervalCount;

            _atI = new uint[atI.Length];
            atI.CopyTo(_atI, 0);
        }
        internal BlockValue(int BoundariesLowerBound, int BoundariesUpperBound, int IntervalCount, ReadOnlyCollection<uint> atI)
        {
            boundariesLowerBound = BoundariesLowerBound;
            boundariesUpperBound = BoundariesUpperBound;            
            intervalCount = IntervalCount;

            _atI = new uint[atI.Count];
            atI.CopyTo(_atI, 0);
        }

        private uint[] _atI { set; get; }

        /// <summary>
        /// Adds inverted structure (i.e., all the intervals overlapping 
        /// the coordinate, including the intervals overlapping with 
        /// left-end and middle; intervals overlapping with right-end 
        /// are not included) to the incremental snapshot 
        /// at the start of the block.
        /// </summary>
        internal ReadOnlyCollection<uint> atI { get { return Array.AsReadOnly(_atI); } }

        internal int boundariesUpperBound { private set; get; }
        internal int boundariesLowerBound { private set; get; }
        internal int intervalCount { private set; get; }
        


    }
}
