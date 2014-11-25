using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEDParser.AssembliesInfo
{
    public static class GenomeAssemblies
    {
        internal static Dictionary<string, int> Assembly(Assemblies assembly)
        {
            switch(assembly)
            {
                case Assemblies.hm19: return hm19.Data();
                case Assemblies.mm10: return mm10.Data();
            }
            return null;
        }
        public static Dictionary<Genomes, GenomeInfo> AllGenomeAssemblies()
        {
            Dictionary<Genomes, GenomeInfo> rtv = new Dictionary<Genomes, GenomeInfo>();

            GenomeInfo hs = new GenomeInfo();
            hs.genomeTitle = "Homo Sapiens";
            hs.genomeAssemblies = new Dictionary<Assemblies, string>();
            hs.genomeAssemblies.Add(Assemblies.hm19, "hm19 (GENECODE 19)");
            rtv.Add(Genomes.HomoSapiens, hs);


            GenomeInfo mm = new GenomeInfo();
            mm.genomeTitle = "Mus musculus";
            mm.genomeAssemblies = new Dictionary<Assemblies, string>();
            mm.genomeAssemblies.Add(Assemblies.hm19, "mm10 (GENCODE M2)");
            rtv.Add(Genomes.MusMusculus, mm);

            return rtv;
        }

        public class GenomeInfo
        {
            public string genomeTitle { internal set; get; }
            public Dictionary<Assemblies, string> genomeAssemblies { internal set; get; }
        }
    }
}
