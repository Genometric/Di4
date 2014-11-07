using CSharpTest.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI3
{
    class BlockSerializer : ISerializer<B>
    {
        private readonly ISerializer<ReadOnlyCollection<Lambda>> _lambdaArraySerializer;
        public BlockSerializer(ISerializer<ReadOnlyCollection<Lambda>> lambdaArraySerializer)
        {
            _lambdaArraySerializer = lambdaArraySerializer;
        }

        public B ReadFrom(System.IO.Stream stream)
        {
            return new B(PrimitiveSerializer.Int32.ReadFrom(stream), _lambdaArraySerializer.ReadFrom(stream));
        }

        public void WriteTo(B value, System.IO.Stream stream)
        {
            if (value == null) return;

            PrimitiveSerializer.Int32.WriteTo(value.omega, stream);
            _lambdaArraySerializer.WriteTo(value.lambda, stream);
        }
    }
}
