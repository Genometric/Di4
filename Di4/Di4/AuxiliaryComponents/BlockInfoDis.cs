using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polimi.DEIB.VahidJalili.DI4
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
