using System;

namespace Polimi.DEIB.VahidJalili.DI4DataSim
{
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
}
