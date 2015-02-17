using System;

namespace Polimi.DEIB.VahidJalili.DI3.DI3B.Logging
{
    public struct ExecutionReport
    {
        public ExecutionReport(int count, TimeSpan ET)
            : this()
        {
            this.count = count;
            this.ET = ET;
        }
        public int count { private set; get; }
        public TimeSpan ET { private set; get; }
    }
}
