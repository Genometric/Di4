using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Di3BMain
{
    [ProtoContract]
    public class CoordinateClass<C> : IComparable<C>
    {
        [ProtoMember(1)]
        C coordinate { set; get; }

        public int CompareTo(C other)
        {
            throw new NotImplementedException();
        }
    }
}
