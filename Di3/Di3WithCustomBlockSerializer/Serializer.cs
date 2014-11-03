using CSharpTest.Net.Serialization;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Di3WithCustomBlockSerializer
{
    class BlockSerializer : ISerializer<B>
    {
        private readonly ISerializer<Lambda[]> _itemSerializer;
        public BlockSerializer(ISerializer<Lambda[]> itemSerializer)
        {
            _itemSerializer = itemSerializer;
        }

        public B ReadFrom(System.IO.Stream stream)
        {
            int omega = PrimitiveSerializer.Int32.ReadFrom(stream);
            Lambda[] lambda = _itemSerializer.ReadFrom(stream);

            return new B(omega, lambda);
        }

        public void WriteTo(B value, System.IO.Stream stream)
        {
            if (value == null) return;

            PrimitiveSerializer.Int32.WriteTo(value.omega, stream);
            _itemSerializer.WriteTo(value.lambda, stream);
        }
    }






    class LambdaArraySerializer : ISerializer<Lambda[]>
    {
        private readonly ISerializer<Lambda> _itemSerializer;
        public LambdaArraySerializer(ISerializer<Lambda> itemSerializer)
        {
            _itemSerializer = itemSerializer;
        }


        public Lambda[] ReadFrom(System.IO.Stream stream)
        {
            int size = PrimitiveSerializer.Int32.ReadFrom(stream);
            if (size < 0)
                return null;
            Lambda[] value = new Lambda[size];
            for (int i = 0; i < size; i++)
                value[i] = _itemSerializer.ReadFrom(stream);
            return value;
        }

        public void WriteTo(Lambda[] value, System.IO.Stream stream)
        {
            if (value == null)
            {
                PrimitiveSerializer.Int32.WriteTo(-1, stream);
                return;
            }
            PrimitiveSerializer.Int32.WriteTo(value.Length, stream);
            foreach (var i in value)
                _itemSerializer.WriteTo(i, stream);
        }
    }


    class LambdaItemSerializer : ISerializer<Lambda>
    {
        public Lambda ReadFrom(System.IO.Stream stream)
        {
            char tau = PrimitiveSerializer.Char.ReadFrom(stream);
            UInt32 atI = PrimitiveSerializer.UInt32.ReadFrom(stream);

            return new Lambda(tau, atI);
        }

        public void WriteTo(Lambda value, System.IO.Stream stream)
        {
            if (value == null) return;
            PrimitiveSerializer.Char.WriteTo(value.tau, stream);
            PrimitiveSerializer.UInt32.WriteTo(value.atI, stream);
        }
    }
}
