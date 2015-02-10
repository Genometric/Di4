using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI3
{
    internal struct PartitionBlock<C>
        where C : IComparable<C>, IFormattable
    {
        public BlockKey<C> left { set; get; }
        public BlockKey<C> right { set; get; }
    }
}
