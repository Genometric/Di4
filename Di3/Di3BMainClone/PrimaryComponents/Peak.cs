using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using Interfaces;

namespace Di3BMain
{
    [ProtoContract]
    public class Peak : IInterval<int, PeakData>
    {
        /*public Peak() // this part is added later after seeing problems with BEDParser. This part is also available with previous version.
        {
            metadata = new PeakData();
        }*/

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
        public PeakData metadata { set; get; }
    }
}
