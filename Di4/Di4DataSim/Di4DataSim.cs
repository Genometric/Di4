using System;
using System.Collections.Generic;
using System.IO;

namespace Polimi.DEIB.VahidJalili.DI4DataSim
{
    public class Di4DataSim
    {
        public Di4DataSim(string outputDirectory)
        {
            _outputDirectory = outputDirectory;
            _random = new Random();
            _windows = new List<Window>();
        }

        private const int _minGap = 50;
        private const int _maxGap = 100;
        private const int _maxLenght = 100;
        private const string _fileExtension = "bed";


        private int _chrCount { set; get; }
        private int _maxICount { set; get; }
        private int _fileSizeProb { set; get; }        
        private string _outputDirectory { set; get; }
        private ErlangDistribution _kDis { set; get; }
        private ErlangDistribution _lambdaDis { set; get; }
        private List<Window> _windows { set; get; }
        private Random _random { set; get; }

        private readonly string[] chrTitles = new string[] {
            "chr1","chr2","chr3","chr4","chr5",
            "chr6","chr7","chr8","chr9","chr10",
            "chr11","chr12", "chr13", "chr14", "chr15",
            "chr16", "chr17", "chr18", "chr19", "chr20",
            "chr21", "chr22", "chrX", "chrY" };

        public void Generate(int sampleCount, int maxICount, int chrCount, ErlangDistribution kDis, ErlangDistribution lambdaDis, int fileSizeProb)
        {
            _kDis = kDis;
            _lambdaDis = lambdaDis;
            _maxICount = maxICount;
            _fileSizeProb = fileSizeProb;
            _chrCount = chrCount;

            string outputPath = _outputDirectory + "count_" + maxICount.ToString();
            Directory.CreateDirectory(outputPath);
            Directory.CreateDirectory(outputPath + Path.DirectorySeparatorChar + "sorted" + Path.DirectorySeparatorChar);
            Directory.CreateDirectory(outputPath + Path.DirectorySeparatorChar + "shuffled" + Path.DirectorySeparatorChar);

            CreateWindows();
            for (int i = 0; i < sampleCount; i++)
            {
                Console.Write("now creating sample {0}", i);
                GenerateSample(outputPath, i);
            }

            #region For Test purpose only
            Dictionary<int, int> llllambda = new Dictionary<int, int>();
            Dictionary<int, int> kkkk = new Dictionary<int, int>();
            foreach (var window in _windows)
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

            using (FileStream fs = new FileStream(outputPath + Path.DirectorySeparatorChar + "WindowParameters__Lambda.txt", FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
                foreach (var window in llllambda)
                    sw.WriteLine(window.Key + "\t" + window.Value);

            using (FileStream fs = new FileStream(outputPath + Path.DirectorySeparatorChar + "WindowParameters__K.txt", FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
                foreach (var window in kkkk)
                    sw.WriteLine(window.Key + "\t" + window.Value);

            #endregion

        }


        private void GenerateSample(string filePath, int sampleNumber)
        {
            int iCreated = 0;
            var intervals = new List<Interval>();
            for (int w = 0; w < _windows.Count; w++)
                if (_random.Next(0, 100) <= _fileSizeProb)
                    intervals.Add(_windows[w].GetInterval(iCreated++));


            if (!Directory.Exists(filePath + Path.DirectorySeparatorChar + "sorted" + Path.DirectorySeparatorChar)) Directory.CreateDirectory(filePath + Path.DirectorySeparatorChar + "sorted" + Path.DirectorySeparatorChar);
            using (FileStream fs = new FileStream(filePath + Path.DirectorySeparatorChar + "sorted" + Path.DirectorySeparatorChar + "Di4SimSample_" + sampleNumber.ToString() + "." + _fileExtension, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
                foreach (var interval in intervals)
                    sw.WriteLine(interval);


            int randomIndex = 0;
            if (!Directory.Exists(filePath + Path.DirectorySeparatorChar + "shuffled" + Path.DirectorySeparatorChar)) Directory.CreateDirectory(filePath + Path.DirectorySeparatorChar + "shuffled" + Path.DirectorySeparatorChar);
            using (FileStream fs = new FileStream(filePath + Path.DirectorySeparatorChar + "shuffled" + Path.DirectorySeparatorChar + "Di4SimSample_" + sampleNumber.ToString() + "." + _fileExtension, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
                while (intervals.Count > 0)
                {
                    randomIndex = _random.Next(0, intervals.Count - 1);
                    sw.WriteLine(intervals[randomIndex]);
                    intervals.RemoveAt(randomIndex);
                }

            Console.WriteLine("\t\tDone!");
        }


        private void CreateWindows()
        {
            int wLeft = 0, wRight = 0;
            _windows = new List<Window>();
            var chrShare = GetChrShare();

            _kDis.minValue = 1;
            _kDis.maxValue = 40;
            _lambdaDis.minValue = 1;
            _lambdaDis.maxValue = 40;

            for (int chr = 0; chr < chrShare.Length; chr++)
            {
                for (int w = 0; w < chrShare[chr]; w++)
                {
                    wLeft = wRight + _random.Next(_minGap, _maxGap);
                    wRight = wLeft + _random.Next(_maxLenght * 4, _maxLenght * 6);

                    if (wLeft > wRight)
                        throw new OverflowException("Reached int32 maximum value when defining windows. Try smaller interval length and/or less interval count");

                    _windows.Add(
                        new Window(
                            chr: chrTitles[chr],
                            left: wLeft,
                            right: wRight,
                            maxIntervalLength: _maxICount,
                            k: _kDis.NextErlang(),
                            lambda: _lambdaDis.NextErlang()));
                }
            }
        }
        private int[] GetChrShare()
        {
            int[] rtv = new int[_chrCount];
            int chrSum = (_chrCount * (_chrCount + 1)) / 2;

            int nDistributedRegions = 0;
            for (int chr = 0; chr < _chrCount; chr++)
            {
                rtv[chr] = (int)Math.Floor((_chrCount - chr) * _maxICount / (double)chrSum);
                nDistributedRegions += rtv[chr];
            }

            if (nDistributedRegions < _maxICount)
                rtv[rtv.Length - 1] += (_maxICount - nDistributedRegions);

            return rtv;
        }
    }
}
