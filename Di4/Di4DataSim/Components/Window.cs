using System;

namespace Polimi.DEIB.VahidJalili.DI4DataSim
{
    internal class Window
    {
        public Window(string chr, int left, int right, int maxIntervalLength, int k, int lambda)
        {
            _chr = chr;
            _left = left;
            _right = right;
            _length = _right - _left;

            this.k = k;
            this.lambda = lambda;

            _ErlangDistribution = new ErlangDistribution(k, lambda, left, right);

            _nameAlphabet = new char[] { 'H', 'A', 'M', 'E', 'D', 'V', 'I', '5', '3', '0', '1' };

            _random = new Random();
            _maxIntervalLength = maxIntervalLength;
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
        private Random _random { set; get; }
        private int _maxIntervalLength { set; get; }


        public Interval GetInterval(int count)
        {
            var ibase = new IntervalBase();
            ibase.left = _ErlangDistribution.NextErlang(_left, _right - 2);
            ibase.right = _ErlangDistribution.NextErlang(ibase.left + 1, Math.Min(ibase.left + 1 + _maxIntervalLength, _right));
            return new Interval(_chr, ibase, GetRandomName(count), Math.Round(_random.NextDouble(), 5));
        }
        private string GetRandomName(int count)
        {
            string rtv = "Di4SIM" +
                count.ToString("X"); // HEX value of counter

            while (rtv.Length < 18)
                rtv += _nameAlphabet[_random.Next(0, _nameAlphabet.Length - 1)];

            return rtv;
        }
    }
}
