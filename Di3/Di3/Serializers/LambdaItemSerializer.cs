using CSharpTest.Net.Serialization;
using System;

namespace DI3
{
    class LambdaItemSerializer : ISerializer<Lambda>
    {
        public Lambda ReadFrom(System.IO.Stream stream)
        {
            return new Lambda(PrimitiveSerializer.Char.ReadFrom(stream), PrimitiveSerializer.UInt32.ReadFrom(stream));
        }

        public void WriteTo(Lambda value, System.IO.Stream stream)
        {
            if (value.atI == default(UInt32) && value.tau == default(char)) return;
            PrimitiveSerializer.Char.WriteTo(value.tau, stream);
            PrimitiveSerializer.UInt32.WriteTo(value.atI, stream);
        }
    }
}
