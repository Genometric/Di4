using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICPMD;
using ProtoBuf;

namespace DI3
{
    /// <summary>
    /// Representing ChIP-seq Peak Metadata.
    /// </summary>
    [ProtoContract]
    public class PeakDataClass : IMetaData<int>, IDisposable
    {
        /// <summary>
        /// Sets and gets zero-based chromosome number.
        /// </summary>
        [ProtoMember(1)]
        public byte chrNo { set; get; }

        /// <summary>
        /// Sets and gets peak name.
        /// </summary>
        [ProtoMember(2)]
        public string name { set; get; }

        /// <summary>
        /// Sets and gets peak value.
        /// </summary>
        [ProtoMember(3)]
        public double value { set; get; }

        /// <summary>
        /// Sets and gets the strand of peak.
        /// </summary>
        [ProtoMember(4)]
        public char strand { set; get; }

        /// <summary>
        /// Gets the left-end of the interval.
        /// </summary>
        [ProtoMember(5)]
        public int left { set; get; }

        /// <summary>
        /// Gets the right-end of the interval.
        /// </summary>
        [ProtoMember(6)]
        public int right { set; get; }


        /// <summary>
        /// Gets hash key generated using
        /// One-at-a-Time method based on 
        /// Dr. Dobb's left method.
        /// </summary>
        //[ProtoMember(7)]
        //public Int64 hashKey { set; get; }




        /// <summary>
        /// Returns hash key based on One-at-a-Time method
        /// generated based on Dr. Dobb's left methods.
        /// </summary>
        /// <returns>Hashkey of the interval.</returns>
        /*public UInt64 GetHashKey()
        {
            string key = chrNo.ToString() + "|" + strand.ToString() + "|" + name + "|" + left.ToString() + "|" + right.ToString() + "|";
            int len = key.Length;

            UInt64 hashKey = 0;
            for (int i = 0; i < len; i++)
            {
                hashKey += key[i];
                hashKey += (hashKey << 10);
                hashKey ^= (hashKey >> 6);
            }

            hashKey += (hashKey << 3);
            hashKey ^= (hashKey >> 11);
            hashKey += (hashKey << 15);

            return hashKey;
        }*/

        [ProtoMember(7)]
        public ulong hashKey { set; get; }


        bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing) //Free any other managed objects here. 
            {
                chrNo = default(byte);
                name = null;
                value = default(double);
                strand = default(char);
                left = default(int);
                right = default(int);
                hashKey = default(ulong);
            }

            // Free any unmanaged objects here. 

            disposed = true;
        }
    }
}