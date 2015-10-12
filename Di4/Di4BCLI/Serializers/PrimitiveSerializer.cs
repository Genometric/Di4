using System.IO;
using CSharpTest.Net.Serialization;

namespace Polimi.DEIB.VahidJalili.DI4.CLI
{
    class PrimitiveSerializer : ISerializer<int>, ISerializer<uint>
    {
        public static readonly PrimitiveSerializer Instance = new PrimitiveSerializer();

        public static readonly ISerializer<int> Int32 = Instance;

        public static readonly ISerializer<uint> UInt32 = Instance;


        #region ISerializer<int> Members

        void ISerializer<int>.WriteTo(int value, Stream stream)
        {
            ((ISerializer<uint>)this).WriteTo(unchecked((uint)value), stream);
        }

        int ISerializer<int>.ReadFrom(Stream stream)
        {
            return unchecked((int)((ISerializer<uint>)this).ReadFrom(stream));
        }

        #endregion

        #region ISerializer<uint> Members

        void ISerializer<uint>.WriteTo(uint value, Stream stream)
        {
            unchecked
            {
                stream.WriteByte((byte)(value >> 24));
                stream.WriteByte((byte)(value >> 16));
                stream.WriteByte((byte)(value >> 8));
                stream.WriteByte((byte)value);
            }
        }

        uint ISerializer<uint>.ReadFrom(Stream stream)
        {
            unchecked
            {
                int b1 = stream.ReadByte();
                int b2 = stream.ReadByte();
                int b3 = stream.ReadByte();
                int b4 = stream.ReadByte();

                Check.Assert<InvalidDataException>(b4 != -1);
                return (
                    (((uint)b1) << 24) |
                    (((uint)b2) << 16) |
                    (((uint)b3) << 8) |
                    (((uint)b4) << 0)
                    );
            }
        }

        #endregion
    }
}
