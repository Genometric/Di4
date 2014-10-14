using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IExtMetaData<C> : IMetaData
    {
        string name { set; get; }
        C left { set; get; }
        C right { set; get; }
        double value { set; get; }
    }
}
