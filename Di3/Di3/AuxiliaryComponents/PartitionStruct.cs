using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace DI3
{
    internal struct Partition<C>
        where C : IComparable<C>
    {
        public C left { set; get; }
        public C right { set; get; }
    }
}
