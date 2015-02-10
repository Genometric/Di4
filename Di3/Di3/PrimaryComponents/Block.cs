using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI3
{
    /// <summary>
    /// for second resolution.
    /// </summary>
    internal struct BlockKey<C>
        where C : IComparable<C>, IFormattable
    {
        internal BlockKey(C LeftEnd, C RightEnd)
            :this()
        {
            leftEnd = LeftEnd;
            rightEnd = RightEnd;
        }

        internal C leftEnd { private set; get; }
        internal C rightEnd { private set; get; }

        internal BlockKey<C> UpdateLeft(C LeftEnd)
        {
            return new BlockKey<C>(LeftEnd: LeftEnd, RightEnd: this.rightEnd);
        }
        internal BlockKey<C> UpdateRight(C RightEnd)
        {
            return new BlockKey<C>(LeftEnd: this.leftEnd, RightEnd: RightEnd);
        }
        internal BlockKey<C> Update(C LeftEnd, C RightEnd)
        {
            return new BlockKey<C>(LeftEnd: LeftEnd, RightEnd: RightEnd);
        }
    }

    internal struct BlockValue
    {
        internal BlockValue(int MaxAccumulation, int IntervalCount)
            : this()
        {
            maxAccumulation = MaxAccumulation;
            intervalCount = IntervalCount;
        }

        internal int maxAccumulation { private set; get; }
        internal int intervalCount { private set; get; }
    }
}
