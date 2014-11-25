using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapSpeedTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MapSpeedTest mapSpeedTest = new MapSpeedTest();

            mapSpeedTest.Run("D:\\VahidTest\\Di3_Test_28.idx", 100000, 5, 50);
        }
    }
}
