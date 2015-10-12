using Polimi.DEIB.VahidJalili.DI4.DI4B;

namespace Polimi.DEIB.VahidJalili.DI4.CLI
{
    static class UserConfig
    {
        public static string workingDirectory { set; get; }
        public static string logFile { set; get; }
        public static IndexType indexType { set; get; }
        public static int minCacheSize { set; get; }
        public static int maxCacheSize { set; get; }
        public static Memory memory { set; get; }

        public static class ParserParameters
        {
            public static byte chrColumn { set; get; }
            public static byte leftEndColumn { set; get; }
            public static byte rightEndColumn { set; get; }
            public static byte nameColumn { set; get; }
            public static byte valueColumn { set; get; }
        }
    }
}
