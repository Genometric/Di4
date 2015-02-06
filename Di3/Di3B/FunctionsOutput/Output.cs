using System;
using IGenomics;

namespace Di3B
{
    public class Output<C, I, M> : IFormattable
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>, IFormattable, new()
        where M : IMetaData, IFormattable
    {
        internal Output(C Left, C Right, int Count)
        {
            //left = Left;
            //right = Right;
            interval = new I();
            interval.left = Left;
            interval.right = Right;
            count = Count;
        }

        internal Output(I Interval, int Count)
        {
            interval = Interval;
            count = Count;
        }

        //public C left { private set; get; }
        //public C right { private set; get; }
        public I interval { private set; get; }
        public int count { private set; get; }



        public string ToString(string separator = "\t")
        {
            return 
                interval.left.ToString() + separator +
                interval.right.ToString() + separator +
                interval.metadata.ToString() + separator +
                count.ToString();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "null";
        }
    }
}
