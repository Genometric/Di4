using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI3;
using IInterval;
using ICPMD;
using ProtoBuf;
using System.Diagnostics;

namespace DirectDi3TEST
{
    class Program
    {
        static void Main(string[] args)
        {
            var di3 = new Di3<PeakClass, PeakDataClass>(PrimitiveSerializer.Int32);

            var peaks = new List<PeakClass>();
            for (int i = 0; i < 90000000; i = i + 3)
                peaks.Add(new PeakClass() { left = i, right = i + 3, metadata = new PeakDataClass() { value = 10.0 } });

            int ii = -1;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int collecterCounter = 0;
            foreach (var peak in peaks)
            {
                di3.Add(peak);
                Console.Write("\rAdded {0:N0} items", ++ii);

                if(++collecterCounter == 5000)
                {
                    GC.Collect();
                    collecterCounter = 0;
                }
            }
            watch.Stop();
            Console.WriteLine("ET: {0}", watch.Elapsed.ToString());
        }
    }

    [ProtoContract]
    public class PeakClass : IInterval<int, PeakDataClass>//, IDisposable
    {
        public PeakClass()
        {
            metadata = new PeakDataClass();
        }

        [ProtoMember(1)]
        public int left { set; get; }

        [ProtoMember(2)]
        public int right { set; get; }

        [ProtoMember(3)]
        public PeakDataClass metadata { set; get; }

        /*private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.Collect();
            GC.SuppressFinalize(this);
            GC.WaitForPendingFinalizers();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                metadata = null;
            }
        }*/
    }

    [ProtoContract]
    public class PeakDataClass : ICPMetadata<int>, IDisposable
    {
        //[ProtoMember(1)]
        //public byte chrNo { set; get; }

        //[ProtoMember(2)]
        //public string name { set; get; }

        [ProtoMember(4)]
        public double value { set; get; }

        /*[ProtoMember(4)]
        public char strand { set; get; }

        [ProtoMember(5)]
        public int left { set; get; }

        [ProtoMember(6)]
        public int right { set; get; }

        [ProtoMember(7)]
        public Int64 hashKey { set; get; }

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
        }*/

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class intComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x.CompareTo(y);
        }
    }
}
