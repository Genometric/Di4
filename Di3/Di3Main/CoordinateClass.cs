using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using Di3Interfaces;


namespace Di3BMain
{
    [ProtoContract]
    public class CoordinateClass<C> : ICoordinate<C>
        where C: IComparable
    {
        [ProtoMember(1)]
        public C Value { get; set; }

        public CoordinateClass(C value)
        {
            Value = value;
        }

        public static implicit operator C(CoordinateClass<C> v)
        {
            return v.Value;
        }

        public void GetValueFrom(int value)
        {
            Value = (C)(object)value;
        }


        public int CompareTo(ICoordinate<C> other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }
}
