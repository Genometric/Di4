using System;

namespace Polimi.DEIB.VahidJalili.DI3
{
    public struct AccEntry<C>
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
    }
}
