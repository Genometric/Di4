using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEDParser;

namespace Di3BMain
{
    internal static class DATA
    {
        internal static List<string> inputSamples = new List<string>();

        internal static Dictionary<UInt32, ParsedBED<CoordinateClass<int>, PeakClass<CoordinateClass<int>>, PeakDataClass<CoordinateClass<int>>>> parsedSamples = new Dictionary<uint, ParsedBED<CoordinateClass<int>, PeakClass<CoordinateClass<int>>, PeakDataClass<CoordinateClass<int>>>>();
    }
}
