using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using IParsableNS;

namespace Di3BMain
{
    [ProtoContract]
    public class CoordinateClass : IComparable<CoordinateClass>, IParsable
    {
        [ProtoMember(1)]
        int coordinate { set; get; }

        public int CompareTo(CoordinateClass that)
        {
            return this.coordinate.CompareTo(that.coordinate);
        }

        public static bool TryParse(string s, out int result)
        {
            return int.TryParse(s, out result);
        }
    }
}
