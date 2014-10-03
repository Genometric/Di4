using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpTest.Net.Serialization;
using DI3;
using ProtoBuf;
using ICPMD;

namespace DI3.Di3Serializers
{
    public class BlockSerializer<C, M> : ISerializer<B<C, M>>
        where C : IComparable<C>
        where M : IMetaData<C>
    {
        public B<C, M> ReadFrom(System.IO.Stream stream)
        {
            return Serializer.Deserialize<B<C, M>>(stream);
        }

        public void WriteTo(B<C, M> value, System.IO.Stream stream)
        {
            // for test only: 
            //string theProto = Serializer.GetProto<B<C, M>>();

            Serializer.Serialize<B<C, M>>(stream, value);
        }
    }

    /*public class CoordinateSerializer<C> : ISerializer<C>
    {
        public C ReadFrom(System.IO.Stream stream)
        {
            return Serializer.Deserialize<C>(stream);
        }

        public void WriteTo(C value, System.IO.Stream stream)
        {
            Serializer.Serialize<C>(stream, value);
        }
    }*/
}
