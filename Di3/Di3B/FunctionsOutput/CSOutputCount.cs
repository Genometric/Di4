using Polimi.DEIB.VahidJalili.DI3;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI3.DI3B
{
    /// <summary>
    /// Cover/Summit Output with the Count of regions present 
    /// at position determined by 'currentBlockLeftEnd' and 'right' coordinate, 
    /// as aggregate function.
    /// </summary>
    public class CSOutputCount<C, I, M> : ICSOutput<C, I, M, Output<C, I, M>>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>, IFormattable, new()
        where M : IMetaData, IFormattable, new()
    {
        public CSOutputCount()
        {
            output = new List<Output<C, I, M>>();
            //output = new ConcurrentBag<Output<C, I, M>>();
        }

        public List<Output<C, I, M>> output { set; get; }

        void ICSOutput<C, I, M, Output<C, I, M>>.Output(C left, C right, List<UInt32> intervals, Object lockOnMe)
        {
            lock (lockOnMe) { output.Add(new Output<C, I, M>(left, right, intervals.Count)); }
        }

        void ICSOutput<C, I, M, Output<C, I, M>>.Output(I interval, List<UInt32> intervals, Object lockOnMe)
        {
            lock (lockOnMe) { output.Add(new Output<C, I, M>(interval, intervals.Count)); }
        }
    }
}
