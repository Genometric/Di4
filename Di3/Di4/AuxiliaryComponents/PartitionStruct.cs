using System;

namespace Polimi.DEIB.VahidJalili.DI4
{
    internal struct Partition<C>
        where C : IComparable<C>, IFormattable
    {
        public C left { set; get; }
        public C right { set; get; }
    }
}
