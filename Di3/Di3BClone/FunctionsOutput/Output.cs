using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace Di3B
{
    public class Output<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : IMetaData<C>
    {
        /// ----------------------------------------
        /// ----- HIGH-LEVEL CAUTION ---------------
        /// -- THE CURRENT METHODS OF ASSIGNMENTS --
        /// -- ARE POSSIBLE MEMORY LEAKS SOURCES ---
        /// ----------------------------------------


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <param name="Count"></param>
        internal Output(C Left, C Right, int Count)
        {
            //left = Left;
            //right = Right;
            interval.left = Left;
            interval.right = Right;
            count = Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Interval"></param>
        /// <param name="Count"></param>
        internal Output(I Interval, int Count)
        {
            interval = Interval;
            count = Count;
        }

        //public C left { private set; get; }
        //public C right { private set; get; }
        public I interval { private set; get; }
        public int count { private set; get; }
    }
}
