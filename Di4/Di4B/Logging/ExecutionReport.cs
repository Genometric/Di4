using System;

namespace Genometric.Di4.Di4B.Logging
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
