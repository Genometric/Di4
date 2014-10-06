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
            /*var b0 = stream.ReadByte();
            var b1 = stream.ReadByte();
            var b2 = stream.ReadByte();
            var b3 = stream.ReadByte();
            var b4 = stream.ReadByte();
            var b5 = stream.ReadByte();
            var b6 = stream.ReadByte();
            var b7 = stream.ReadByte();
            var b8 = stream.ReadByte();
            var b9 = stream.ReadByte();*/
            return Serializer.Deserialize<B<C, M>>(stream);
        }

        public void WriteTo(B<C, M> value, System.IO.Stream stream)
        {
            // for test only: 
            //string theProto = Serializer.GetProto<B<C, M>>();

            try { Serializer.Serialize<B<C, M>>(stream, value); }
            catch (Exception exp) { }
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
