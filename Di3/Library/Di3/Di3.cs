using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3
{
    internal sealed class Di3<C, I> : B<C, I>
    {
        internal List<B<C, I>> Bi { private set; get; }

        internal Di3()
        {
            Bi = new List<B<C, I>>();
        }

        public void AddInterval(I interval)
        {

        }
    }
}
