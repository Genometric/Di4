using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polimi.DEIB.VahidJalili.Di3.SimulationDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            RegionGenerator gR = new RegionGenerator();
            Console.WriteLine("");

            gR.GenerateRegions();
        }
    }
}
