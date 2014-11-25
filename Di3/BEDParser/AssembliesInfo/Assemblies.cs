using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEDParser.AssembliesInfo
{
    internal static class Assemblies
    {
        internal static Dictionary<string, int> Assembly(AvailableAssemblies assembly)
        {
            switch(assembly)
            {
                case AvailableAssemblies.hm19: return hm19.Data();
                case AvailableAssemblies.mm10: return mm10.Data();
            }
            return null;
        }
        internal static List<AvailableAssemblies> AllGenomeAssemblies(AvailableGenomes genome)
        {
            List<AvailableAssemblies> rtv = new List<AvailableAssemblies>();

            switch(genome)
            {
                case AvailableGenomes.HomoSapiens:
                    rtv.Add(AvailableAssemblies.hm19);
                    break;

                case AvailableGenomes.MusMusculus:
                    rtv.Add(AvailableAssemblies.mm10);
                    break;
            }

            return rtv;
        }
    }
}
