using System;

namespace Polimi.DEIB.VahidJalili.DI4
{
    public struct BlockKey<C> : IComparable<BlockKey<C>>
        where C : IComparable<C>, IFormattable
    {
        internal BlockKey(C LeftEnd, C RightEnd)
            : this()
        {
            leftEnd = LeftEnd;
            rightEnd = RightEnd;
        }

        public C leftEnd { private set; get; }
        public C rightEnd { private set; get; }

        internal BlockKey<C> UpdateLeft(C LeftEnd)
        {
            return new BlockKey<C>(LeftEnd: LeftEnd, RightEnd: rightEnd);
        }
        internal BlockKey<C> UpdateRight(C RightEnd)
        {
            return new BlockKey<C>(LeftEnd: leftEnd, RightEnd: RightEnd);
        }
        internal BlockKey<C> Update(C LeftEnd, C RightEnd)
        {
            return new BlockKey<C>(LeftEnd: LeftEnd, RightEnd: RightEnd);
        }

        public int CompareTo(BlockKey<C> other)
        {
            if (other.Equals(null)) return 1;

            int tmpCmp = leftEnd.CompareTo(other.leftEnd);
            if (tmpCmp != 0) return tmpCmp;
            return rightEnd.CompareTo(other.rightEnd);
        }
    }
}
