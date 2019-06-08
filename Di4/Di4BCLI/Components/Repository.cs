using Polimi.DEIB.VahidJalili.GIFP;

namespace Genometric.Di4.CLI
{
    public static class Repository
    {
        public static ParsedChIPseqPeaks<int, Peak, PeakData> parsedSample { set; get; }
        public static ParsedVariants<int, Variant, VariantData> parsedVariants { set; get; }
    }
}
