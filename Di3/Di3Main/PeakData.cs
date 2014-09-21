using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICPMD;
using DI3;

namespace Di3BMain
{
    /// <summary>
    /// Representing ChIP-seq Peak Metadata.
    /// </summary>
    public class PeakDataClass : ICPMetadata<int>
    {
        /// <summary>
        /// Sets and gets zero-based chromosome number.
        /// </summary>
        public byte chrNo { set; get; }

        /// <summary>
        /// Sets and gets peak name.
        /// </summary>
        public string name { set; get; }

        /// <summary>
        /// Sets and gets peak value.
        /// </summary>
        public double value { set; get; }

        /// <summary>
        /// Sets and gets the strand of peak.
        /// </summary>
        public char strand { set; get; }

        /// <summary>
        /// Gets the left-end of the interval.
        /// </summary>
        public int left { set; get; }

        /// <summary>
        /// Gets the right-end of the interval.
        /// </summary>
        public int right { set; get; }
        

        /// <summary>
        /// Returns hash key based on One-at-a-Time method
        /// generated based on Dr. Dobb'left methods.
        /// </summary>
        /// <returns>Hashkey of the interval.</returns>
        public UInt64 GetHashKey()
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
        }
    }
}
