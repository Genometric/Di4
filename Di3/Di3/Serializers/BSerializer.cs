using System;
using CSharpTest.Net.Serialization;
using ProtoBuf;
using Interfaces;
using System.IO;
using System.Collections.ObjectModel;

namespace DI3
{
    public class BSerializer<C, M> : ISerializer<B<C, M>>
        where C : IComparable<C>
        where M : IMetaData, new()
    {
        public B<C, M> ReadFrom(System.IO.Stream stream)
        {
            return Serializer.DeserializeWithLengthPrefix<B<C, M>>(stream, PrefixStyle.Fixed32);
        }

        public void WriteTo(B<C, M> value, System.IO.Stream stream)
        {
            Serializer.SerializeWithLengthPrefix<B<C, M>>(stream, value, PrefixStyle.Fixed32);
        }
    }

    class BlockSerializer<C, M> : ISerializer<B<C, M>>
        where C : IComparable<C>
        where M : IMetaData, new()
    {
        private readonly ISerializer<ReadOnlyCollection<Lambda<C, M>>> _itemSerializer;
        public BlockSerializer(ISerializer<ReadOnlyCollection<Lambda<C, M>>> itemSerializer)
        {
            _itemSerializer = itemSerializer;
        }

        public B<C, M> ReadFrom(System.IO.Stream stream)
        {
            int omega = PrimitiveSerializer.Int32.ReadFrom(stream);
            ReadOnlyCollection<Lambda<C, M>> lambda = _itemSerializer.ReadFrom(stream);

            return new B<C, M>(omega, lambda);
        }

        public void WriteTo(B<C,M> value, System.IO.Stream stream)
        {
            if (value == null) return;

            PrimitiveSerializer.Int32.WriteTo(value.omega, stream);
            _itemSerializer.WriteTo(value.lambda, stream);
        }
    }







    class LambdaArraySerializer<C, M> : ISerializer<ReadOnlyCollection<Lambda<C, M>>>
        where C : IComparable<C>
        where M : IMetaData, new()
    {
        private readonly ISerializer<Lambda<C,M>> _itemSerializer;
        public LambdaArraySerializer(ISerializer<Lambda<C,M>> itemSerializer)
        {
            _itemSerializer = itemSerializer;
        }


        public ReadOnlyCollection<Lambda<C, M>> ReadFrom(System.IO.Stream stream)
        {
            int size = PrimitiveSerializer.Int32.ReadFrom(stream);
            if (size < 0)
                //return null;
                return Array.AsReadOnly(new Lambda<C, M>[0]);

            Lambda<C, M>[] value = new Lambda<C, M>[size];
            for (int i = 0; i < size; i++)
                value[i] = _itemSerializer.ReadFrom(stream);
            return Array.AsReadOnly(value);
        }

        public void WriteTo(ReadOnlyCollection<Lambda<C, M>> value, System.IO.Stream stream)
        {
            if (value == null)
            {
                PrimitiveSerializer.Int32.WriteTo(-1, stream);
                return;
            }
            PrimitiveSerializer.Int32.WriteTo(value.Count, stream);
            foreach (var i in value)
                _itemSerializer.WriteTo(i, stream);
        }
    }









    class LambdaItemSerializer<C, M> : ISerializer<Lambda<C,M>>
        where C : IComparable<C>
        where M : IMetaData, new()
    {
        public Lambda<C, M> ReadFrom(System.IO.Stream stream)
        {
            char tau = PrimitiveSerializer.Char.ReadFrom(stream);
            UInt32 hashKey = PrimitiveSerializer.UInt32.ReadFrom(stream);

            return new Lambda<C, M>(tau, hashKey);
        }

        public void WriteTo(Lambda<C, M> value, System.IO.Stream stream)
        {
            if (value == null) return;
            PrimitiveSerializer.Char.WriteTo(value.tau, stream);
            PrimitiveSerializer.UInt32.WriteTo(value.atI.hashKey, stream);
        }
    }
}
