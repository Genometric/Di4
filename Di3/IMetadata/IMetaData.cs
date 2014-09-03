using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICPMD
{
    public interface IMetaData<C>
    {
        C left { set; get; }
        C right { set; get; }
        UInt64 GetHashKey();
    }
}
