using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3Interfaces
{
    public interface ICoordinate<C> : IComparable<ICoordinate<C>>
    {
        C Value { set; get; }

        void GetValueFrom(int value);
    }
}
