using System;

namespace Genometric.Di4.AuxiliaryComponents
{
    internal struct DecomposerValue<C>
        where C : IComparable<C>, IFormattable
    {
        public DecomposerValue(int leftDesignatedRegion, int rightDesignatedRegion)
        {
            lastObservedEnd = Phi.LeftEnd;
            this.leftDesignatedRegion = leftDesignatedRegion;
            this.rightDesignatedRegion = rightDesignatedRegion;
        }
        private DecomposerValue(int[] designatedRegions)
        {
            lastObservedEnd = Phi.RightEnd;
            leftDesignatedRegion = designatedRegions[0];
            rightDesignatedRegion = designatedRegions[1];
        }

        public Phi lastObservedEnd { private set; get; }
        public int leftDesignatedRegion { set; get; }
        public int rightDesignatedRegion { set; get; }

        public DecomposerValue<C> UpdateRightEnd(int value)
        {
            return new DecomposerValue<C>(new int[] { leftDesignatedRegion, value });
        }
        public DecomposerValue<C> UpdateLeftEnd(int value)
        {
            return new DecomposerValue<C>(value, rightDesignatedRegion);
        }
    }

    internal struct DesignatedRegionNEW<C>
        where C : IComparable<C>, IFormattable
    {
        public DesignatedRegionNEW(C left, C right)
        {
            this.left = left;
            this.right = right;
        }

        public C left { set; get; }
        public C right { set; get; }

        public DesignatedRegionNEW<C> UpdateLeft(C newLeft)
        {
            return new DesignatedRegionNEW<C>(newLeft, right);
        }
        public DesignatedRegionNEW<C> UpdateRight(C newRight)
        {
            return new DesignatedRegionNEW<C>(left, newRight);
        }
    }
}
