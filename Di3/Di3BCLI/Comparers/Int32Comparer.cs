using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI3.CLI
{
    public class Int32Comparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x.CompareTo(y);
        }
    }
}
