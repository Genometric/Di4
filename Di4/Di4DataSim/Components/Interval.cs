using System;

namespace Polimi.DEIB.VahidJalili.DI4DataSim
{
    internal class Interval : IntervalBase, IComparable<Interval>, IComparable<IntervalBase>, IFormattable
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
}
