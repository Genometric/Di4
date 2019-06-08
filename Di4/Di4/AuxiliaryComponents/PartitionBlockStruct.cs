using System;

namespace Genometric.Di4
{
    internal struct PartitionBlock<C>
        where C : IComparable<C>, IFormattable
    {
        public BlockKey<C> left { set; get; }
        public BlockKey<C> right { set; get; }
    }
}
