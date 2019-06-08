using Polimi.DEIB.VahidJalili.IGenomics;
using System;

namespace Genometric.Di4.Di4B
{
    public class Output<C, I, M> : IFormattable
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>, IFormattable, new()
        where M : IMetaData, IFormattable, new()
    {
        internal Output(C Left, C Right, int Count)
        {
            interval = new I();
            interval.left = Left;
            interval.right = Right;
            count = Count;

            /// Note:
            /// If constructor is not expensive, execute following command,
            /// otherwise, leave it commented-out as it is now.
            //interval.atI = new M();
        }

        internal Output(I Interval, int Count)
        {
            interval = Interval;
            count = Count;
        }

        public I interval { private set; get; }
        public int count { private set; get; }

        public string ToString(string separator = "\t")
        {
            if (interval.metadata == null)
                return
                    interval.left.ToString() + separator +
                    interval.right.ToString() + separator +
                    count.ToString();

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
