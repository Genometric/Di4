using System;
using CSharpTest.Net.Serialization;
using ProtoBuf;
using Interfaces;
using System.IO;
using System.Collections.ObjectModel;

namespace DI3
{
    public class BSerializer/*<C, M>*/ : ISerializer<B>
        /*where C : IComparable<C>
        where M : IMetaData, new()*/
    {
        public B ReadFrom(System.IO.Stream stream)
        {
            return Serializer.DeserializeWithLengthPrefix<B>(stream, PrefixStyle.Fixed32);
        }

        public void WriteTo(B value, System.IO.Stream stream)
        {
            Serializer.SerializeWithLengthPrefix<B>(stream, value, PrefixStyle.Fixed32);
        }
    }

    class BlockSerializer/*<C, M> */: ISerializer<B>
        /*where C : IComparable<C>
        where M : IMetaData, new()*/
    {
        private readonly ISerializer<ReadOnlyCollection<Lambda>> _itemSerializer;
        public BlockSerializer(ISerializer<ReadOnlyCollection<Lambda>> itemSerializer)
        {
            _itemSerializer = itemSerializer;
        }

        public B ReadFrom(System.IO.Stream stream)
        {
            //int omega = PrimitiveSerializer.Int32.ReadFrom(stream);
            //ReadOnlyCollection<Lambda> lambda = _itemSerializer.ReadFrom(stream);
            //return new B(omega, lambda);

            return new B(PrimitiveSerializer.Int32.ReadFrom(stream), _itemSerializer.ReadFrom(stream));
        }

        public void WriteTo(B value, System.IO.Stream stream)
        {
            if (value == null) return;

            PrimitiveSerializer.Int32.WriteTo(value.omega, stream);
            _itemSerializer.WriteTo(value.lambda, stream);
        }
    }







    class LambdaArraySerializer/*<C, M>*/ : ISerializer<ReadOnlyCollection<Lambda>>
        /*where C : IComparable<C>
        where M : IMetaData, new()*/
    {
        private readonly ISerializer<Lambda> _itemSerializer;
        public LambdaArraySerializer(ISerializer<Lambda> itemSerializer)
        {
            _itemSerializer = itemSerializer;
        }


        public ReadOnlyCollection<Lambda> ReadFrom(System.IO.Stream stream)
        {
            int size = PrimitiveSerializer.Int32.ReadFrom(stream);
            if (size < 0)
                //return null;
                return Array.AsReadOnly(new Lambda[0]);

            Lambda[] value = new Lambda[size];
            for (int i = 0; i < size; i++)
                value[i] = _itemSerializer.ReadFrom(stream);
            return Array.AsReadOnly(value);
        }

        public void WriteTo(ReadOnlyCollection<Lambda> value, System.IO.Stream stream)
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









    class LambdaItemSerializer/*<C, M> */: ISerializer<Lambda>
       // where C : IComparable<C>
        //where M : IMetaData, new()
    {
        public Lambda ReadFrom(System.IO.Stream stream)
        {
            //char tau = PrimitiveSerializer.Char.ReadFrom(stream);
            //UInt32 hashKey = PrimitiveSerializer.UInt32.ReadFrom(stream);
            //return new Lambda(tau, hashKey);

            return new Lambda(PrimitiveSerializer.Char.ReadFrom(stream), PrimitiveSerializer.UInt32.ReadFrom(stream));
        }

        public void WriteTo(Lambda value, System.IO.Stream stream)
        {
            if (value.atI == default(UInt32) && value.tau == default(char)) return;
            PrimitiveSerializer.Char.WriteTo(value.tau, stream);
            PrimitiveSerializer.UInt32.WriteTo(value.atI, stream);
        }
    }
}
