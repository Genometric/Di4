using DI3;
using IGenomics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Di3B
{
    /// <summary>
    /// Cover/Summit Output with the Count of regions present 
    /// at position determined by 'left' and 'right' coordinate, 
    /// as aggregate function.
    /// </summary>
    public class CSOutputCount<C, I, M> : ICSOutput<C, I, M, Output<C, I, M>>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>, IFormattable, new()
        where M : IMetaData, IFormattable, new()
    {
        public CSOutputCount()
        {
            //output = new List<Output<C, I, M>>();
            output = new ConcurrentBag<Output<C, I, M>>();
        }

        public ConcurrentBag<Output<C, I, M>> output { set; get; }

        void ICSOutput<C, I, M, Output<C, I, M>>.Output(C left, C right, List<Lambda> intervals)
        {
            output.Add(new Output<C, I, M>(left, right, intervals.Count));
        }

        void ICSOutput<C, I, M, Output<C, I, M>>.Output(I interval, List<Lambda> intervals)
        {
            output.Add(new Output<C, I, M>(interval, intervals.Count));
        }
    }
}
