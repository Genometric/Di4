using CSharpTest.Net.Serialization;
using Polimi.DEIB.VahidJalili.DI4.Serializers;

namespace Polimi.DEIB.VahidJalili.DI4
{
    internal class BlockValueSerializer : ISerializer<BlockValue>
    {
        private atIArraySerializer _atIArraySerializer;
        public BlockValueSerializer()
        {
            _atIArraySerializer = new atIArraySerializer();
        }

        public BlockValue ReadFrom(System.IO.Stream stream)
        {
            return new BlockValue(
                atI: _atIArraySerializer.ReadFrom(stream),
                BoundariesLowerBound: PrimitiveSerializer.Int32.ReadFrom(stream),
                BoundariesUpperBound: PrimitiveSerializer.Int32.ReadFrom(stream),
                IntervalCount: PrimitiveSerializer.Int32.ReadFrom(stream));
        }

        public void WriteTo(BlockValue value, System.IO.Stream stream)
        {
            _atIArraySerializer.WriteTo(value.atI, stream);
            PrimitiveSerializer.Int32.WriteTo(value.boundariesLowerBound, stream);
            PrimitiveSerializer.Int32.WriteTo(value.boundariesUpperBound, stream);
            PrimitiveSerializer.Int32.WriteTo(value.intervalCount, stream);
        }
    }
}
