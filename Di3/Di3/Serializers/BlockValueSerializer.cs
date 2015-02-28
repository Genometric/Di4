using CSharpTest.Net.Serialization;

namespace Polimi.DEIB.VahidJalili.DI3
{
    internal class BlockValueSerializer : ISerializer<BlockValue>
    {
        public BlockValue ReadFrom(System.IO.Stream stream)
        {
            return new BlockValue(
                MaxAccumulation: PrimitiveSerializer.Int32.ReadFrom(stream),
                IntervalCount: PrimitiveSerializer.Int32.ReadFrom(stream)); 
        }

        public void WriteTo(BlockValue value, System.IO.Stream stream)
        {
            PrimitiveSerializer.Int32.WriteTo(value.maxAccumulation, stream);
            PrimitiveSerializer.Int32.WriteTo(value.intervalCount, stream);
        }
    }
}
