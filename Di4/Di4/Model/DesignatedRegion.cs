using System;
using System.Collections.Generic;

namespace Genometric.Di4.AuxiliaryComponents
{
    internal class DesignatedRegion<C>
        where C : IComparable<C>, IFormattable
    {
        public DesignatedRegion()
        {
            lambdas = new Dictionary<uint, Phi>();           
        }

        public C leftEnd { set; get; }
        public C rightEnd { set; get; }
        public int mu { set; get; }
        public Dictionary<uint, Phi> lambdas { set; get; }
    }
}
