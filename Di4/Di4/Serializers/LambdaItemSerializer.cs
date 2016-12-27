using CSharpTest.Net.Serialization;

namespace Polimi.DEIB.VahidJalili.DI4
{
    class LambdaItemSerializer : ISerializer<Lambda>
    {
        public Lambda ReadFrom(System.IO.Stream stream)
        {
            return new Lambda((Phi)PrimitiveSerializer.Byte.ReadFrom(stream), PrimitiveSerializer.UInt32.ReadFrom(stream), PrimitiveSerializer.UInt32.ReadFrom(stream));
        }

        public void WriteTo(Lambda value, System.IO.Stream stream)
        {
            /// Do I need the Following condition ?
            //if (value.atI == default(uint) && value.phi == default(Phi)) return;
            PrimitiveSerializer.Byte.WriteTo((byte)value.phi, stream);
            PrimitiveSerializer.UInt32.WriteTo(value.atI, stream);
            PrimitiveSerializer.UInt32.WriteTo(value.collectionID, stream);
        }
    }
}
