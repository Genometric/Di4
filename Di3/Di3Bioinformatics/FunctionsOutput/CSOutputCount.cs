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
    /// <summary>
    /// Cover/Summit Output with the Count of regions present 
    /// at position determined by 'left' and 'right' coordinate, 
    /// as aggregate function.
    /// </summary>
    public class CSOutputCount<C, M> : ICSOutput<C, M, Output<C>>
        where C : IComparable<C>
        where M : IMetaData<C>
    {
        public CSOutputCount()
        {
            output = new List<Output<C>>();
        }

        public List<Output<C>> output { set; get; }

        void ICSOutput<C, M, Output<C>>.Output(C left, C right, List<Lambda<C, M>> intervals)
        {
            output.Add(new Output<C>(left, right, intervals.Count));
        }
    }
}
