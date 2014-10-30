using System;
using System.Collections.Generic;
using DI3;
using Interfaces;

namespace Di3B
{
    /// <summary>
    /// Cover/Summit Output with the Count of regions present 
    /// at position determined by 'left' and 'right' coordinate, 
    /// as aggregate function.
    /// </summary>
    public class CSOutputCount<C, I, M> : ICSOutput<C, I, M, Output<C, I, M>>
        where C : IComparable<C>
        where I : IInterval<C, M>, new()
        where M : IMetaData/*<C>*/
    {
        public CSOutputCount()
        {
            output = new List<Output<C, I, M>>();
        }

        public List<Output<C, I, M>> output { set; get; }

        void ICSOutput<C, I, M, Output<C, I, M>>.Output(C left, C right, List<Lambda<C, M>> intervals)
        {
            output.Add(new Output<C, I, M>(left, right, intervals.Count));
        }

        void ICSOutput<C, I, M, Output<C, I, M>>.Output(I interval, List<Lambda<C, M>> intervals)
        {
            output.Add(new Output<C, I, M>(interval, intervals.Count));
        }
    }
}
