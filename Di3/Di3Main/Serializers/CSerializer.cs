using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI3.Interfaces;

namespace Di3BMain.Serializers
{
    public interface CSerializer<int> : ICSerializer<int>
    {
        public int ReadFrom(System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }

        public void WriteTo(int value, System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
