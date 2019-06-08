using System;

namespace Genometric.Di4
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

            int l = Left.CompareTo(other.Left);
            if (l != 0) return l;
            else return Right.CompareTo(other.Right);
        }
    }
}
