using System.Collections.Generic;

namespace IndexSpeedTest
{
    public class Int32Comparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x.CompareTo(y);
        }
    }
}
