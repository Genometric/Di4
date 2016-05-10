using CSharpTest.Net.Serialization;
using System;
using System.IO;
using System.Collections.ObjectModel;

namespace Polimi.DEIB.VahidJalili.DI4.Serializers
{
    internal class atIArraySerializer : ISerializer<ReadOnlyCollection<uint>>
    {
        public ReadOnlyCollection<uint> ReadFrom(Stream stream)
        {
            int size = PrimitiveSerializer.Int32.ReadFrom(stream);
            if (size < 0) return Array.AsReadOnly(new uint[0]);

            uint[] value = new uint[size];
            for (int i = 0; i < size; i++)
                value[i] = PrimitiveSerializer.UInt32.ReadFrom(stream);
            return Array.AsReadOnly(value);
        }

        public void WriteTo(ReadOnlyCollection<uint> value, Stream stream)
        {
            if (value == null)
            {
                PrimitiveSerializer.Int32.WriteTo(-1, stream);
                return;
            }
            PrimitiveSerializer.Int32.WriteTo(value.Count, stream);
            foreach (var i in value)
                PrimitiveSerializer.UInt32.WriteTo(i, stream);
        }
    }
}
