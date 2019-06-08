using CSharpTest.Net.Serialization;
using System.Collections.ObjectModel;
using System.IO;

namespace Genometric.Di4.Inv
{
    class BSerializer : ISerializer<B>
    {
        private readonly ISerializer<ReadOnlyCollection<Lambda>> _lambdaArraySerializer;
        public BSerializer(ISerializer<ReadOnlyCollection<Lambda>> lambdaArraySerializer)
        {
            _lambdaArraySerializer = lambdaArraySerializer;
        }
        public B ReadFrom(Stream stream)
        {
            return new B(PrimitiveSerializer.UInt16.ReadFrom(stream), _lambdaArraySerializer.ReadFrom(stream));
        }
        public void WriteTo(B value, Stream stream)
        {
            if (value == null) return;

            PrimitiveSerializer.UInt16.WriteTo(value.omega, stream);
            _lambdaArraySerializer.WriteTo(value.lambda, stream);
        }
    }
}
