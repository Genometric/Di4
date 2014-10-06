using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IInterval;
using ICPMD;
using ProtoBuf;


namespace Di3Bioinformatics
{
    [ProtoContract]
    public class PeakClass : IInterval<int, PeakDataClass>, IDisposable
    {
        public PeakClass()
        {
            metadata = new PeakDataClass();
        }

        /// <summary>
        /// Sets and gets the left-end of the interval.
        /// </summary>
        [ProtoMember(1)]
        public int left { set; get; }

        /// <summary>
        /// Sets and gets the right-end of the interval.
        /// </summary>
        [ProtoMember(2)]
        public int right { set; get; }



        /// <summary>
        /// Sets and gets the descriptive metadata
        /// of the interval. It could be a reference
        /// to a memory object, or a pointer, or 
        /// an entry ID on database, or etc. 
        /// </summary>
        [ProtoMember(3)]
        public PeakDataClass metadata { set; get; }


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
                left = default(int);
                right = default(int);
                metadata = null;
            }

            // Free any unmanaged objects here. 

            disposed = true;
        }
    }
}
