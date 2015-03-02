using CSharpTest.Net.Serialization;
using System;
using System.Collections.ObjectModel;

namespace Polimi.DEIB.VahidJalili.DI3
{
    //class LambdaArraySerializer : ISerializer<ReadOnlyCollection<Lambda>>
    class LambdaArraySerializer : ISerializer<Lambda[]>
    {
        private readonly ISerializer<Lambda> _lambdaItemSerializer;
        public LambdaArraySerializer(ISerializer<Lambda> lambdaItemSerializer)
        {
            _lambdaItemSerializer = lambdaItemSerializer;
        }

        //public ReadOnlyCollection<Lambda> ReadFrom(System.IO.Stream stream)
        public Lambda[] ReadFrom(System.IO.Stream stream)
        {
            int size = PrimitiveSerializer.Int32.ReadFrom(stream);
            if (size < 0) return new Lambda[0];//Array.AsReadOnly(new Lambda[0]);

            Lambda[] value = new Lambda[size];
            for (int i = 0; i < size; i++)
                value[i] = _lambdaItemSerializer.ReadFrom(stream);
            return value;//Array.AsReadOnly(value);
        }

        //public void WriteTo(ReadOnlyCollection<Lambda> value, System.IO.Stream stream)
        public void WriteTo(Lambda[] value, System.IO.Stream stream)
        {
            if (value == null)
            {
                PrimitiveSerializer.Int32.WriteTo(-1, stream);
                return;
            }
            PrimitiveSerializer.Int32.WriteTo(value.Length, stream);
            foreach (var i in value)
                _lambdaItemSerializer.WriteTo(i, stream);
        }
    }
}
