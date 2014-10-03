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
    public interface ICSOutput<M, O>
        //where C : IComparable<C>
        where M : IMetaData<int>
    {
        void Output(int left, int right, List<Lambda<M>> intervals);

        List<O> output { set; get; }
    }
}
