using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IInterval;
using ICPMD;

namespace DI3
{
    /// <summary>
    /// Interface for Cover/Summit Output
    /// </summary>
    public interface ICSOutput<C, M, O>
        where C : IComparable<C>
        where M : IMetaData<C>
    {
        void Output(C left, C right, List<Lambda<C, M>> intervals);

        List<O> output { set; get; }
    }
}
