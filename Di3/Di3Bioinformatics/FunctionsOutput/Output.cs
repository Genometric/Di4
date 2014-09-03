using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3Bioinformatics
{
    public class Output<C>
        where C : IComparable<C>
    {
        internal Output(C Left, C Right, int Count)
        {
            left = Left;
            right = Right;
            count = Count;
        }

        public C left { private set; get; }
        public C right { private set; get; }
        public int count { private set; get; }
    }
}
