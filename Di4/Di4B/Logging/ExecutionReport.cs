using System;

namespace Polimi.DEIB.VahidJalili.DI4.DI4B.Logging
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
