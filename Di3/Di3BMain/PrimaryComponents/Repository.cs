using System;
using System.Collections.Generic;
using BEDParser;

namespace Di3BMain
{
    public static class Repository
    {
        public static List<string> inputSamples = new List<string>();

        // maybe I'm not using this ?!!
        public static Dictionary<UInt32, Dictionary<string, List<Peak>>> repo = new Dictionary<uint, Dictionary<string, List<Peak>>>();

        public static Dictionary<UInt32, ParsedBED<int, Peak, PeakData>> parsedSamples = new Dictionary<uint, ParsedBED<int, Peak, PeakData>>();
    }
}
