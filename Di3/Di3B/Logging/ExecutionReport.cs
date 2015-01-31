using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3B.Logging
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
