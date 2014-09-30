using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IParsableNS
{
    public interface IParsable
    {
        bool TryParse(string s, out int result);
    }
}
