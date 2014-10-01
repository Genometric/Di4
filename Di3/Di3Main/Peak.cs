using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEDParser;
using IInterval;
using ICPMD;
using ProtoBuf;

namespace Di3BMain
{
    [ProtoContract]
    public class PeakClass<C> : IInterval<C, PeakDataClass<C>>
    {
        public PeakClass()
        {
            metadata = new PeakDataClass<C>();
        }

        /// <summary>
        /// Sets and gets the left-end of the interval.
        /// </summary>
        [ProtoMember(1)]
        public C left { set; get; }

        /// <summary>
        /// Sets and gets the right-end of the interval.
        /// </summary>
        [ProtoMember(2)]
        public C right { set; get; }

        

        /// <summary>
        /// Sets and gets the descriptive metadata
        /// of the interval. It could be a reference
        /// to a memory object, or a pointer, or 
        /// an entry ID on database, or etc. 
        /// </summary>
        [ProtoMember(3)]
        public PeakDataClass<C> metadata { set; get; }
    }
}
