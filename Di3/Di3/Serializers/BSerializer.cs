using System;
using CSharpTest.Net.Serialization;
using ProtoBuf;
using Interfaces;

namespace DI3
{
    public class BSerializer<C, M> : ISerializer<B<C, M>>
        where C : IComparable<C>
        where M : IMetaData/*<C>*/
    {
        public B<C, M> ReadFrom(System.IO.Stream stream)
        {
            return Serializer.DeserializeWithLengthPrefix<B<C, M>>(stream, PrefixStyle.Fixed32);
        }

        public void WriteTo(B<C, M> value, System.IO.Stream stream)
        {
            try { Serializer.SerializeWithLengthPrefix<B<C, M>>(stream, value, PrefixStyle.Fixed32); }
            catch (Exception exp) { }
        }
    }
}
