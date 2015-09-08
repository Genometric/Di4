using CSharpTest.Net.Serialization;
using System.Collections.ObjectModel;

namespace Polimi.DEIB.VahidJalili.DI3
{
    class BookmarkSerializer : ISerializer<IIB>
    {
        private readonly ISerializer<ReadOnlyCollection<Lambda>> _lambdaArraySerializer;
        //private readonly ISerializer<Lambda[]> _lambdaArraySerializer;
        public BookmarkSerializer(ISerializer<ReadOnlyCollection<Lambda>> lambdaArraySerializer)
        //    public BookmarkSerializer(ISerializer<Lambda[]> lambdaArraySerializer)
        {
            _lambdaArraySerializer = lambdaArraySerializer;
        }

        public IIB ReadFrom(System.IO.Stream stream)
        {
            return new IIB(PrimitiveSerializer.Int32.ReadFrom(stream), PrimitiveSerializer.UInt16.ReadFrom(stream), _lambdaArraySerializer.ReadFrom(stream));
            //return new B(PrimitiveSerializer.Int32.ReadFrom(stream), PrimitiveSerializer.UInt16.ReadFrom(stream), _lambdaArraySerializer.ReadFrom(stream));
        }

        public void WriteTo(IIB value, System.IO.Stream stream)
        {
            if (value == null) return;

            PrimitiveSerializer.Int32.WriteTo(value.mu, stream);
            PrimitiveSerializer.UInt16.WriteTo(value.omega, stream);
            _lambdaArraySerializer.WriteTo(value.lambda, stream);
        }
    }
}
