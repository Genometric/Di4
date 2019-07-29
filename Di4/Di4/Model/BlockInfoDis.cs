using System.Collections.Generic;

namespace Genometric.Di4
{
    public class BlockInfoDis
    {
        public BlockInfoDis()
        {
            maxAccDis = new SortedDictionary<int, int>();
            intervalCountDis = new SortedDictionary<int, int>();
        }
        public SortedDictionary<int, int> maxAccDis { set; get; }
        public SortedDictionary<int, int> intervalCountDis { set; get; }
    }
}
