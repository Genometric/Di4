using CSharpTest.Net.Serialization;
using System.Collections.ObjectModel;
using System;
using System.IO;

namespace Polimi.DEIB.VahidJalili.DI3
{
    class IBookmarkSerializer : ISerializer<IB>
    {
        private readonly ISerializer<ReadOnlyCollection<Lambda>> _lambdaArraySerializer;
        public IBookmarkSerializer(ISerializer<ReadOnlyCollection<Lambda>> lambdaArraySerializer)
        {
            _lambdaArraySerializer = lambdaArraySerializer;
        }
        public IB ReadFrom(Stream stream)
        {
            return new IB(PrimitiveSerializer.UInt16.ReadFrom(stream), _lambdaArraySerializer.ReadFrom(stream));
        }
        public void WriteTo(IB value, Stream stream)
        {
            if (value == null) return;

            PrimitiveSerializer.UInt16.WriteTo(value.omega, stream);
            _lambdaArraySerializer.WriteTo(value.lambda, stream);
        }
    }
}
