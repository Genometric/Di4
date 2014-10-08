using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using Interfaces;

namespace Di3BMain
{
    /// <summary>
    /// Representing ChIP-seq Peak Metadata.
    /// </summary>
    [ProtoContract]
    public class PeakData : IMetaData<int>
    {
        /// <summary>
        /// Sets and gets peak name.
        /// </summary>
        [ProtoMember(1)]
        public string name { set; get; }

        /// <summary>
        /// Gets the left-end of the interval.
        /// </summary>
        [ProtoMember(2)]
        public int left { set; get; }

        /// <summary>
        /// Gets the right-end of the interval.
        /// </summary>
        [ProtoMember(3)]
        public int right { set; get; }

        /// <summary>
        /// Sets and gets peak value.
        /// </summary>
        [ProtoMember(4)]
        public double value { set; get; }

        /// <summary>
        /// Gets hash key generated using
        /// One-at-a-Time method based on 
        /// Dr. Dobb's left method.
        /// </summary>
        [ProtoMember(5)]
        public ulong hashKey { set; get; }



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
    }
}
