using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IMetaData<C>
    {
        string name { set; get; }
        C left { set; get; }
        C right { set; get; }
        double value { set; get; }
        UInt32 hashKey { set; get; }
    }
}
