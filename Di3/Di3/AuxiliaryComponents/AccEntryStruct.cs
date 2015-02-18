using System;

namespace Polimi.DEIB.VahidJalili.DI3
{
    public struct AccEntry<C> : IComparable<AccEntry<C>>
        where C : IComparable<C>, IFormattable
    {
        internal AccEntry(C left, C right, int accumulation)
            : this()
        {
            Left = left;
            Right = right;
            Accumulation = accumulation;
        }
        public C Left { private set; get; }
        public C Right { private set; get; }
        public int Accumulation { private set; get; }

        public int CompareTo(AccEntry<C> other)
        {
            if (other.Equals(null)) return 1;

            int l = this.Left.CompareTo(other.Left);
            if (l != 0) return l;
            else return this.Right.CompareTo(other.Right);
        }
    }
}
