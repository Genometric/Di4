using System.Collections.Generic;

namespace Genometric.Di4.Comparers
{
    internal class StringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return x.CompareTo(y);
        }
    }
}
