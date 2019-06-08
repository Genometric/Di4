using Genometric.Di4.CLI;
using Polimi.DEIB.VahidJalili.GIFP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Genometric.Di4.SimulationDataGenerator
{
    internal class RegionGenerator
    {
        public RegionGenerator()
        {
            window = new IntervalBase();
            chrBorders = new IntervalBase();
            _accDis = new SortedDictionary<int, AccDis>();
        }

        const int minGap = 50;
        const int maxGap = 100;
        const int maxLenght = 100; // Window width is four-fold this length.
        const int chrCount = 1;

        const int maxRegionCount = 100000; // per sample
        const int regionCountVarience = 0; // 0% -> 100% (0%: all samples have equal number of regions equal to max region count; 100%: some samples might be empty)
        const int sampleCount = 100; // IF YOU CHANGE THIS: remember to revise maxAcc.
        

        const int minAccPercentage = 15; // min percentage of overlapping regions (not initial regions, but the region count determined by variation).
        const int maxAccPercentage = 85; // max percentage of overlapping regions (not initial regions, but the region count determined by variation).

        
        private SortedDictionary<int, AccDis> _accDis { set; get; }


        private const int _maxRetries = 5;


        char dirSep = Path.DirectorySeparatorChar;

        const string parentPath = "";
        static string filesExtension = "bed";

        /// <summary>
        /// Percentages of windows where all samples intersec.
        /// <para> A set of percentages, where for each 
        /// a set of samples is to be generated. Each set 
        /// of samples has a given percentage of windows
        /// where intervals of all the samples overlap. </para>
        /// </summary>
        private int[] _pwAsi = new int[] { 0/*, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100*/ };

        static readonly string[] chrTitles = new string[] {
            "chr1","chr2","chr3","chr4","chr5",
            "chr6","chr7","chr8","chr9","chr10",
            "chr11","chr12", "chr13", "chr14", "chr15",
            "chr16", "chr17", "chr18", "chr19", "chr20",
            "chr21", "chr22", "chrX", "chrY" };

        int[] windowCount { set; get; }
        static Random rnd = new Random();


        /// <summary>
        /// Is a list of integers as sample
        /// numbers that supports in the process 
        /// of distributing generated intervals
        /// to different samples.
        /// </summary>
        private List<int> _jdh { set; get; }

        public void GenerateSimulationRegions()
        {
            k = 40;
            lambda = 40;
            _maxErlang = GetErlangNo((k - 1) / (double)lambda);

            _jdh = new List<int>(sampleCount);
            for (int i = 0; i < sampleCount; i++)
                _jdh.Add(i);


            windowCount = GetChrShare();
            Directory.CreateDirectory(parentPath + "count_" + maxRegionCount.ToString());
            string outputPath = "";

            for (int i = 0; i < _pwAsi.Length; i++)
            {
                Console.WriteLine("Preparing similarity : " + _pwAsi[i].ToString() + "%");

                outputPath = parentPath + "count_" + maxRegionCount.ToString() + dirSep + "similarity_" + _pwAsi[i].ToString() + dirSep;
                Directory.CreateDirectory(outputPath + dirSep + "sorted" + dirSep);
                Directory.CreateDirectory(outputPath + dirSep + "shuffled" + dirSep);

                chrBorders.left = 0;
                for (int chr = 0; chr < chrCount; chr++)
                {                    
                    Console.WriteLine("Generating Regions for chr" + chr.ToString());
                    CreateSortedRegions(i, chr, chrTitles[chr], outputPath);
                    chrBorders.right = window.right;
                    CreateMapFile(chr, chrTitles[chr], outputPath);
                    chrBorders.left = window.right;
                }

                CreateShuffleRegions(outputPath);
            }
        }
        private int[] GetChrShare()
        {
            int[] rtv = new int[chrCount];
            int chrSum = (chrCount * (chrCount + 1)) / 2;

            int nDistributedRegions = 0;
            for (int chr = 0; chr < chrCount; chr++)
            {
                rtv[chr] = (int)Math.Floor((chrCount - chr) * maxRegionCount / (double)chrSum);
                nDistributedRegions += rtv[chr];
            }

            if (nDistributedRegions < maxRegionCount)
                rtv[rtv.Length - 1] += (maxRegionCount - nDistributedRegions);

            return rtv;
        }


        /// <summary>
        /// Maximum overlapping groups to be generated.
        /// Based on maximum overlapping percentage, how many dichotomic maximum-overlapping-groups are needed ?
        /// </summary>
        private int mOG { set; get; }

        /// <summary>
        /// generated maximum overlapping groups.
        /// </summary>
        private int gOG { set; get; }


        /// <summary>
        /// Create maximum overlapping group or non-maximum-overlapping group.
        /// True: create, False: don't create.
        /// </summary>
        private bool cOG { set; get; }

        private bool CreateMaxOverlap(int groupsToGo)
        {
            if (mOG != 0 && mOG - gOG < groupsToGo)
            { // Generate either full overlapping or non-overlapping

                // create overlapping regions
                if (rnd.NextDouble() > 0.5) return true;

                // create non-overlapping regions
                else return false;
            }

            // generate only non-overlapping regions
            else if (mOG <= gOG) return false;

            // generate only overlapping regions
            else return true;
        }

        /// <summary>
        /// It is the count of accepted intervals of 
        /// a windows overlapping newly generated interval.
        /// </summary>
        private int _oC { set; get; }

        


        private IntervalBase window { set; get; }
        private IntervalBase chrBorders { set; get; }

        private void CreateSortedRegions_old(int varIndex, int chr, string chrTitle, string filePath)
        {
            mOG = (int)Math.Round((_pwAsi[varIndex] * windowCount[chr]) / 100.0);
            gOG = 0;

            int retries;
            bool accept;
            bool overlaps;

            // Total number of intervals
            // to be created.
            int iToCreate;

            // Total number of created intervals.
            int iCreated;

            // Max of overlapping
            // intervals to be created.
            int oToCreate;

            // Number of overlapping intervals created.
            // i.e., the overlap that satisfies 'oToCreate' criteria.
            int oCreated;

            Console.WriteLine("Total Created Regions : ");

            // Window number starts from 1 (not 0) because of the calculation of cOG. 
            for (int windowNumber = 1; windowNumber <= windowCount[chr]; windowNumber++)
            {
                retries = 0;
                iCreated = 0;
                oCreated = 0;

                if (CreateMaxOverlap(windowCount[chr] - windowNumber))
                {
                    iToCreate = sampleCount;
                    oToCreate = sampleCount;
                    gOG++;
                }
                else
                {
                    iToCreate = sampleCount - (int)Math.Round((sampleCount * regionCountVarience) / 100.0);
                    oToCreate = rnd.Next((iToCreate * minAccPercentage) / 100, (iToCreate * maxAccPercentage) / 100);
                    if (oToCreate == 0) oToCreate = 1;
                }
                
                gIntervals.Clear();

                window.left = window.right + rnd.Next(minGap, maxGap);
                window.right = window.left + (maxLenght * 4);
                var iRegion = new IntervalBase(window);

                while (iCreated < iToCreate)
                {
                    accept = false;
                    var ibase = new IntervalBase();
                    ibase.left = rnd.Next(window.left, window.right - maxLenght - 1);
                    ibase.right = rnd.Next(ibase.left + 1, ibase.left + 1 + maxLenght);

                    overlaps = ibase.CompareTo(iRegion) == 0 ? true : false;

                    if (overlaps && oCreated < oToCreate)
                    {
                        accept = true;
                    }
                    else if (!overlaps && oCreated >= oToCreate)
                    {
                        _oC = gIntervals.Count(kvp => kvp.Value.CompareTo(ibase) == 0);
                        if (_oC <= oToCreate)
                            accept = true;
                    }

                    if (accept)
                    {
                        var i = new Interval(chrTitle, ibase, GetRandomName(iCreated++), Math.Round(rnd.NextDouble(), 5));
                        gIntervals.Add(i.name, i);

                        if (overlaps)
                        {
                            iRegion.Intersect(i);                                
                            oCreated++;
                        }

                        retries = 0;
                        continue;
                    }
                    else
                    {
                        if (++retries > _maxRetries)
                        {
                            gIntervals.Clear();
                            iCreated = 0;
                            oCreated = 0;

                            iRegion.left = window.left;
                            iRegion.right = window.right;
                        }
                        continue;
                    }
                }

                _tSampleNo = 0;
                var tSet = new HashSet<int>(_jdh);
                foreach(var region in gIntervals)
                {
                    _tSampleNo = tSet.ElementAt(rnd.Next(0, tSet.Count));
                    tSet.Remove(_tSampleNo);

                    if (!Directory.Exists(filePath + "sorted" + dirSep)) Directory.CreateDirectory(filePath + "sorted" + dirSep);
                    using (FileStream fs =
                        new FileStream(filePath + "sorted" + dirSep + "sample_" + _tSampleNo.ToString() + "." + filesExtension, FileMode.Append, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(fs))
                        sw.WriteLine(region.Value);
                }

                Console.Write("\r{0:N0} \\ {1:N0}", (windowNumber + 1).ToString(), windowCount[chr].ToString());
            }

            Console.WriteLine("");
            Console.WriteLine("Done !");
        }


        public int k { set; get; }
        public int lambda { set; get; }
        private double _maxErlang { set; get; }

        private int windowLength { get { return maxLenght * 4; } }

        Dictionary<double, double> generatedCoordinatesFORTESTONLY = new Dictionary<double, double>();


        Dictionary<string, Interval> gIntervals = new Dictionary<string, Interval>();


        private void CreateSortedRegions(int varIndex, int chr, string chrTitle, string filePath)
        {
            mOG = (int)Math.Round((_pwAsi[varIndex] * windowCount[chr]) / 100.0);
            gOG = 0;

            int retries;
            bool accept;
            bool overlaps;

            // Total number of intervals
            // to be created.
            int iToCreate;

            // Total number of created intervals.
            int iCreated;

            // Min and Max of overlapping
            // intervals to be created.
            int[] oToCreate;

            // Number of overlapping intervals created.
            // i.e., the overlap that satisfies 'oToCreate' criteria.
            int oCreated;

            Console.WriteLine("Total Created Regions : ");

            
            // Window number starts from 1 (not 0) because of the calculation of cOG. 
            for (int windowNumber = 1; windowNumber <= windowCount[chr]; windowNumber++)
            {
                retries = 0;
                iCreated = 0;
                oCreated = 0;

                gIntervals.Clear();

                window.left = window.right + rnd.Next(minGap, maxGap);
                window.right = window.left + windowLength;
                

                if (CreateMaxOverlap(windowCount[chr] - windowNumber))
                {
                    // try to re-write this with Erlang function too.

                    iToCreate = sampleCount;
                    oToCreate = new int[] { sampleCount, sampleCount };
                    gOG++;

                    var iRegion = new IntervalBase(window);

                    while (iCreated < iToCreate)
                    {
                        accept = false;
                        var ibase = new IntervalBase();
                        ibase.left = rnd.Next(window.left, window.right - maxLenght - 1);
                        ibase.right = rnd.Next(ibase.left + 1, ibase.left + 1 + maxLenght);

                        overlaps = ibase.CompareTo(iRegion) == 0 ? true : false;

                        if (overlaps && oCreated <= oToCreate[1])
                        {
                            var i = new Interval(chrTitle, ibase, GetRandomName(iCreated++), Math.Round(rnd.NextDouble(), 5));
                            gIntervals.Add(i.name, i);
                            iRegion.Intersect(i);
                            oCreated++;
                            retries = 0;
                            continue;
                        }
                        else
                        {
                            if (++retries > _maxRetries)
                            {
                                gIntervals.Clear();
                                iCreated = 0;
                                oCreated = 0;

                                iRegion.left = window.left;
                                iRegion.right = window.right;
                            }
                            continue;
                        }
                    }
                }
                else
                {
                    iToCreate = sampleCount - (int)Math.Round((sampleCount * regionCountVarience) / 100.0);

                    while (iCreated < iToCreate)
                    {
                        var ibase = new IntervalBase();
                        ibase.left = (int)Math.Floor(((windowLength * GetErlangNo(rnd.NextDouble())) / _maxErlang) + window.left);
                        ibase.right = rnd.Next(ibase.left + 1, Math.Min(ibase.left + 1 + maxLenght, window.right));
                        var i = new Interval(chrTitle, ibase, GetRandomName(iCreated++), Math.Round(rnd.NextDouble(), 5));
                        gIntervals.Add(i.name, i);
                    }
                }

                

                

                
                
                _tSampleNo = 0;
                var tSet = new HashSet<int>(_jdh);
                foreach (var region in gIntervals)
                {
                    _tSampleNo = tSet.ElementAt(rnd.Next(0, tSet.Count));
                    tSet.Remove(_tSampleNo);

                    if (!Directory.Exists(filePath + "sorted" + dirSep)) Directory.CreateDirectory(filePath + "sorted" + dirSep);
                    using (FileStream fs =
                        new FileStream(filePath + "sorted" + dirSep + "sample_" + _tSampleNo.ToString() + "." + filesExtension, FileMode.Append, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(fs))
                        sw.WriteLine(region.Value);
                }

                

                Console.Write("\r{0:N0} \\ {1:N0}", (windowNumber + 1).ToString(), windowCount[chr].ToString());
            }

            Console.WriteLine("");
            Console.WriteLine("Done !");
        }



        public void GenerateSimulationRegions(ErlangDistribution kDis, ErlangDistribution lambdaDis, int fileSizeProb)
        {
            _kDis = kDis;
            _lambdaDis = lambdaDis;
            this.fileSizeProb = fileSizeProb;

            string outputPath = parentPath + "count_" + maxRegionCount.ToString();
            Directory.CreateDirectory(outputPath);
            Directory.CreateDirectory(outputPath + dirSep + "sorted" + dirSep);
            Directory.CreateDirectory(outputPath + dirSep + "shuffled" + dirSep);

            CreateWindows();
            for (int i = 0; i < sampleCount; i++)
            {
                Console.Write("now creating sample {0}", i);
                GenerateSample(outputPath, i);
            }


            Dictionary<int, int> llllambda = new Dictionary<int, int>();
            Dictionary<int, int> kkkk = new Dictionary<int, int>();
            foreach (var window in windows)
            {
                if (llllambda.ContainsKey(window.lambda))
                    llllambda[window.lambda]++;
                else
                    llllambda.Add(window.lambda, 1);

                if (kkkk.ContainsKey(window.k))
                    kkkk[window.k]++;
                else
                    kkkk.Add(window.k, 1);
            }

            using (FileStream fs = new FileStream(outputPath + dirSep + "WindowParameters__Lambda.txt", FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
                foreach (var window in llllambda)
                    sw.WriteLine(window.Key + "\t" + window.Value);

            using (FileStream fs = new FileStream(outputPath + dirSep + "WindowParameters__K.txt", FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
                foreach (var window in kkkk)
                    sw.WriteLine(window.Key + "\t" + window.Value);

        }

        private int fileSizeProb { set; get; }


        
        private double _kMaxErlang { set; get; }


        public class ErlangDistribution
        {
            public ErlangDistribution(int k, int lambda)
            {
                this.k = k > 0 ? k : 1;
                this.lambda = lambda > 0 ? lambda : 1;
                _maxErlang = GetErlangNo((this.k - 1) / (double)this.lambda);

                minValue = 1;
                maxValue = 40;
                _length = maxValue - minValue;
                _random = new Random();
            }
            internal ErlangDistribution(int k, int lambda, int minValue, int maxValue)
            {
                this.k = k > 0 ? k : 1;
                this.lambda = lambda > 0 ? lambda : 1;
                _maxErlang = GetErlangNo((this.k - 1) / (double)this.lambda);

                this.minValue = minValue;
                this.maxValue = maxValue;
                _length = this.maxValue - this.minValue;
                _random = new Random();
            }

            public int k { private set; get; }
            public int lambda { private set; get; }
            private double _maxErlang { set; get; }
            public int minValue { internal set; get; }
            public int maxValue { internal set; get; }
            private int _length { set; get; }
            private Random _random { set; get; }
            private double _x { set; get; }

            public int NextErlang()
            {
                do { _x = _random.NextDouble(); }
                while (_x == 0);
                return (int)Math.Floor(((_length * GetErlangNo(_x)) / _maxErlang) + minValue);
            }
            public int NextErlang(int minValue, int maxValue)
            {
                do { _x = _random.NextDouble(); }
                while (_x == 0);
                int length = maxValue - minValue;
                return (int)Math.Floor(((length * GetErlangNo(_x)) / _maxErlang) + minValue);
            }
            private double GetErlangNo(double x)
            {
                return (Math.Pow(lambda, k) * Math.Pow(x, k - 1) * Math.Exp((-1) * lambda * x)) / (Factorial(k - 1));
            }
            private double Factorial(int number)
            {
                double rtv = 1;
                for (uint i = 1; i <= number; i++)
                    rtv *= i;
                return rtv;
            }
        }

        private ErlangDistribution _kDis { set; get; }
        private ErlangDistribution _lambdaDis { set; get; }

        private void CreateWindows()
        {
            int wLeft = 0, wRight = 0;
            windows = new List<Window>();
            var chrShare = GetChrShare();

            //_kMaxErlang = GetErlangNo((this.k - 1) / (double)this.lambda);

            
            _kDis.minValue = 1;
            _kDis.maxValue = 40;
            _lambdaDis.minValue = 1;
            _lambdaDis.maxValue = 40;

            for (int chr = 0; chr < chrShare.Length; chr++)
            {
                for (int w = 0; w < chrShare[chr]; w++)
                {
                    wLeft = wRight + rnd.Next(minGap, maxGap);
                    wRight = wLeft + rnd.Next(maxLenght * 4, maxLenght * 6);

                    if (wLeft > wRight)
                        throw new OverflowException("Reached int32 maximum value when defining windows. Try smaller interval length and/or less interval count");

                    windows.Add(
                        new Window(
                            chr: chrTitles[chr],
                            left: wLeft,
                            right: wRight,
                            k: _kDis.NextErlang(),//GetNonZeroErlangNo(kk, klambda),
                            lambda: _lambdaDis.NextErlang()));// GetNonZeroErlangNo(lambdak, lambdalambda)));
                }
            }
        }

        private int GetNonZeroErlangNo(int k, int lambda)
        {
            double x = 0;
            int rtv;
            do
            {
                do { x = rnd.NextDouble(); }
                while (x == 0);
                rtv = (int)Math.Round(((Math.Pow(lambda, k) * Math.Pow(x, k - 1) * Math.Exp((-1) * lambda * x)) / (Factorial(k - 1))) * 10);
            }
            while (rtv == 0);

            return rtv;
        }

        private double GetErlangNo(double x, int k, int lambda)
        {
            return (Math.Pow(lambda, k) * Math.Pow(x, k - 1) * Math.Exp((-1) * lambda * x)) / (Factorial(k - 1));
        }


        private void GenerateSample(string filePath, int sampleNumber)
        {
            int iCreated = 0;
            var intervals = new List<Interval>();
            for (int w = 0; w < windows.Count; w++)
            {
                if (rnd.Next(0, 100) <= fileSizeProb)
                {
                    intervals.Add(windows[w].GetInterval(iCreated++));
                }
            }

            if (!Directory.Exists(filePath + dirSep + "sorted" + dirSep)) Directory.CreateDirectory(filePath + dirSep + "sorted" + dirSep);
            using (FileStream fs = new FileStream(filePath + dirSep + "sorted" + dirSep + "Di4SimSample_" + sampleNumber.ToString() + "." + filesExtension, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
                foreach (var interval in intervals)
                {
                    sw.WriteLine(interval);
                }

            int randomIndex = 0;
            if (!Directory.Exists(filePath + dirSep + "shuffled" + dirSep)) Directory.CreateDirectory(filePath + dirSep + "shuffled" + dirSep);
            using (FileStream fs = new FileStream(filePath + dirSep + "shuffled" + dirSep + "Di4SimSample_" + sampleNumber.ToString() + "." + filesExtension, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
                while (intervals.Count > 0)
                {
                    randomIndex = rnd.Next(0, intervals.Count - 1);
                    sw.WriteLine(intervals[randomIndex]);
                    intervals.RemoveAt(randomIndex);
                }

            Console.WriteLine("\t\tDone!");
        }

        private int _tSampleNo { set; get; }


        private List<Window> windows = new List<Window>();
        private class Window
        {
            public Window(string chr, int left, int right, int k, int lambda)
            {
                _chr = chr;
                _left = left;
                _right = right;
                _length = _right - _left;

                //this.k = k > 0 ? k : 1;
                //this.lambda = lambda > 0 ? lambda : 1;                
                //_maxErlang = GetErlangNo((this.k - 1) / (double)this.lambda);

                this.k = k;
                this.lambda = lambda;

                _ErlangDistribution = new ErlangDistribution(k, lambda, left, right);

                _nameAlphabet = new char[] { 'H', 'A', 'M', 'E', 'D', 'V', 'I', '5', '3', '0', '1' };
            }

            private ErlangDistribution _ErlangDistribution { set; get; }
            public int k { private set; get; }
            public int lambda { private set; get; }
            private string _chr { set; get; }
            private int _left { set; get; }
            private int _right { set; get; }
            private int _length { set; get; }
            private double _maxErlang { set; get; }
            char[] _nameAlphabet { set; get; }


            public Interval GetInterval(int count)
            {
                var ibase = new IntervalBase();
                ibase.left = _ErlangDistribution.NextErlang(_left, _right - 2);
                /*do
                {
                    ibase.left = (int)Math.Floor(((_length * GetErlangNo(rnd.NextDouble())) / _maxErlang) + _left);
                } while (ibase.left + 2 >= _right);*/



                ibase.right = _ErlangDistribution.NextErlang(ibase.left + 1, Math.Min(ibase.left + 1 + maxLenght, _right));
                //ibase.right = rnd.Next(ibase.left + 1, Math.Min(ibase.left + 1 + maxLenght, _right));


                return new Interval(_chr, ibase, GetRandomName(count), Math.Round(rnd.NextDouble(), 5));
            }
            /*private double GetErlangNo(double x)
            {
                return (Math.Pow(lambda, k) * Math.Pow(x, k - 1) * Math.Exp((-1) * lambda * x)) / (Factorial(k - 1));
            }*/
            /*private double Factorial(int number)
            {
                double rtv = 1;
                for (uint i = 1; i <= number; i++)
                    rtv *= i;
                return rtv;
            }*/
            private string GetRandomName(int count)
            {
                string rtv = "Di4SIM" +
                    count.ToString("X"); // HEX value of counter

                while (rtv.Length < 18)
                    rtv += _nameAlphabet[rnd.Next(0, _nameAlphabet.Length - 1)];

                return rtv;
            }
        }



        private double GetErlangNo(double x)//int min, int max)
        {
            /*double x = rnd.Next(min, max);
            var a1 = Math.Pow(lambda, k);
            var a2 = Math.Pow(x, k - 1);
            var a3 = Math.Exp((-1) * lambda * x);
            var a4 = Factorial(k - 1);

            var a5 = Math.Exp(-800);
            var a6 = Math.Pow(Math.E, -4000);

            var tmp = (Math.Pow(lambda, k) * Math.Pow(x, k - 1) * Math.Exp((-1) * lambda * x)) / (Factorial(k - 1));*/

            /*
            double fff = (Math.Pow(lambda, k) * Math.Pow(x, k - 1) * Math.Exp((-1) * lambda * x)) / (Factorial(k - 1));

            double aaa = Math.Pow(lambda, k) / 1000000.0;
            double bbb = Math.Pow(x, k - 1) / 1000000.0;
            double ccc = Math.Exp((-1) * lambda * x) / 1000000.0;
            double ddd = Factorial(k - 1)/ 1000000.0;

            double rrr = ((aaa * bbb * ccc) / ddd);
            double rrrr = rrr * 1E12;

    */
            return (Math.Pow(lambda, k) * Math.Pow(x, k - 1) * Math.Exp((-1) * lambda * x)) / (Factorial(k - 1));
        }
        private double Factorial(int number)
        {
            double rtv = 1;
            for (uint i = 1; i <= number; i++)
                rtv *= i;
            return rtv;
        }




        private void CreateShuffleRegions(string folderPath)
        {

                if (!Directory.Exists(folderPath + "sorted" + dirSep)) Directory.CreateDirectory(folderPath + "sorted" + dirSep);
            using (FileStream fs =
                new FileStream(folderPath + "sorted" + dirSep + "TESTONLY" + "." + filesExtension, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
                foreach (var coordinate in generatedCoordinatesFORTESTONLY)
                    sw.WriteLine(coordinate.Key.ToString() + "\t" + coordinate.Value.ToString());
            








            Console.WriteLine("Writing shuffled files.");
            string randomChr = null;
            char randomStrand = 'V';
            int randomRegion = 0;
            Peak peak = null;
            var dirInfo = new DirectoryInfo(folderPath + dirSep + "sorted");
            FileInfo[] determinedFiles = dirInfo.GetFiles("*." + filesExtension);
            foreach (FileInfo fileInfo in determinedFiles)
            {
                Console.WriteLine(string.Format("Now writing: {0}", Path.GetFileNameWithoutExtension(fileInfo.FullName)));
                BEDParser<Peak, PeakData> bedParser = new BEDParser<Peak, PeakData>(fileInfo.FullName, Genomes.HomoSapiens, Assemblies.hg19, true);
                var parsedSample = bedParser.Parse();
                var intervals = parsedSample.intervals;

                if (!Directory.Exists(folderPath + "shuffled" + dirSep)) Directory.CreateDirectory(folderPath + "shuffled" + dirSep);
                using (FileStream fs =
                    new FileStream(folderPath + "shuffled" + dirSep + Path.GetFileNameWithoutExtension(fileInfo.FullName) + "." + filesExtension, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                    while (intervals.Count > 0)
                    {
                        randomChr = intervals.ElementAt(rnd.Next(0, intervals.Count)).Key;
                        randomStrand = intervals[randomChr].ElementAt(rnd.Next(0, intervals[randomChr].Count)).Key;
                        randomRegion = rnd.Next(0, intervals[randomChr][randomStrand].Count);
                        peak = intervals[randomChr][randomStrand][randomRegion];
                        sw.WriteLine(randomChr + "\t" + peak.ToString("\t") + "\t" + randomStrand);
                        intervals[randomChr][randomStrand].RemoveAt(randomRegion);
                        if (intervals[randomChr][randomStrand].Count == 0)
                            intervals[randomChr].Remove(randomStrand);
                        if (intervals[randomChr].Count == 0)
                            intervals.Remove(randomChr);
                    }
            }
            Console.WriteLine("Done");
        }
        private void CreateMapFile(int chr, string chrTitle, string filePath)
        {
            var mapIntervals = new SortedDictionary<IntervalBase, Interval>();

            Console.WriteLine("");            

            int generatedMapIntervals = 0;
            while (generatedMapIntervals < windowCount[chr])
            {
                var iBase = new IntervalBase();
                iBase.left = rnd.Next(chrBorders.left, chrBorders.right - maxLenght - 2);
                iBase.right = rnd.Next(iBase.left + 1, iBase.left + 1 + maxLenght);

                if (!mapIntervals.ContainsKey(iBase))
                {
                    generatedMapIntervals++;
                    mapIntervals.Add(iBase, new Interval(chrTitle, iBase, GetRandomName(generatedMapIntervals), Math.Round(rnd.NextDouble(), 5)));

                    Console.Write("\r{0:N0} map intervals created.", generatedMapIntervals);
                }
            }

            using (FileStream fs = new FileStream(filePath + "sorted" + dirSep + "mapRef." + filesExtension, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
                foreach (var interval in mapIntervals)
                    sw.WriteLine(interval.Value.ToString());
        }



        private string GetRandomName(int counter)
        {
            string rtv = "Di4SIM" +
                counter.ToString("X"); // HEX value of counter

            char[] chars = new char[] { 'H', 'A', 'M', 'E', 'D', 'V', 'I', '5', '3', '0', '1' };

            while (rtv.Length < 18)
                rtv += chars[rnd.Next(0, chars.Length - 1)];

            return rtv;
        }

        


        private class IntervalBase: IComparable<IntervalBase>
        {
            public IntervalBase()
            { }

            public IntervalBase(int left, int right)
            {
                this.left = left;
                this.right = right;
            }
            public IntervalBase(IntervalBase interval)
            {
                left = interval.left;
                right = interval.right;
            }

            public int left { set; get; }
            public int right { set; get; }

            public void Intersect(IntervalBase interval)
            {
                left = Math.Max(left, interval.left);
                right = Math.Min(right, interval.right);
            }

            public int CompareTo(IntervalBase other)
            {
                if (right <= other.left) return -1;
                if (left >= other.right) return 1;
                return 0;
            }
        }

        private class Interval : IntervalBase, IComparable<Interval>, IComparable<IntervalBase>, IFormattable
        {
            public Interval(string chr, int left, int right, string name, double value) : base(left, right)
            {
                this.chr = chr;
                this.name = name;
                this.value = value;
            }
            public Interval(string chr, IntervalBase interval, string name, double value) : base(interval)
            {
                this.chr = chr;
                this.name = name;
                this.value = value;
            }


            public string chr { set; get; }
            
            public string name { set; get; }
            public double value { set; get; }

                        

            public int CompareTo(Interval other)
            {
                return base.CompareTo(other);
            }

            public string ToString(string format, IFormatProvider formatProvider)
            {
                return chr.ToString() + "\t" + left.ToString() + "\t" + right.ToString() + "\t" + name + "\t" + value.ToString();
            }
        }


        private struct AccDis
        {
            public AccDis(int maximum)
            {
                generated = 0;
                this.maximum = maximum;
            }
            
            public int maximum { set; get; }
            public int generated { set; get; }
        }

        

    }
}
