using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI3.Interfaces;

namespace Di3BMain.Serializers
{
    public class DataSerializers : ICSerializer<int>, IMSerializer<PeakDataClass>
    {
        public static readonly CSerializer<int> Int32;

        public static readonly MSerializer<PeakDataClass> Peak;
    }
}
