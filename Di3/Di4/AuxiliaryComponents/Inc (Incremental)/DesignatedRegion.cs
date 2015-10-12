using Polimi.DEIB.VahidJalili.DI4;
using System;
using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI3.AuxiliaryComponents.Inc
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
