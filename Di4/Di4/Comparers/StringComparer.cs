using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI4.Comparers
{
    internal class StringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return x.CompareTo(y);
        }
    }
}
