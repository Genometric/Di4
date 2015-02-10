using System;

namespace DI3
{
    internal struct Partition<C>
        where C : IComparable<C>, IFormattable
    {
        public C left { set; get; }
        public C right { set; get; }
    }
}
