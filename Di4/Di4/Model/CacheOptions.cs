using CSharpTest.Net.Collections;

namespace Genometric.Di4
{
    public class CacheOptions
    {
        public CacheOptions(int CacheMaximumHistory, int CacheMinimumHistory,int CacheKeepAliveTimeOut, CachePolicy CachePolicy )
        {
            this.CacheMaximumHistory = CacheMaximumHistory;
            this.CacheMinimumHistory = CacheMinimumHistory;
            this.CacheKeepAliveTimeOut = CacheKeepAliveTimeOut;
            this.CachePolicy = CachePolicy;
        }
        public int CacheMaximumHistory { private set; get; }
        public int CacheMinimumHistory { private set; get; }
        public int CacheKeepAliveTimeOut { private set; get; }
        public CachePolicy CachePolicy { private set; get; }
    }
}
