using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polimi.DEIB.VahidJalili.DI4.Inc
{
    public enum Aggregate { Count, Sum, Maximum, Minimum, Median, Mean, StandardDeviation };

    public class VCOutputStrategy<C, I, M, O> : IOutput<C, I, M, uint>
         where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        public VCOutputStrategy()
        {
            output = new List<uint>();
            samplesCV = new SortedDictionary<uint, int>();
        }

        public List<uint> output { set; get; }

        /// <summary>
        /// This dictionary contains IDs of samples (key) that have 
        /// a mutation overlapping reference mutation. 
        /// This dictionary helps to count the number of mutations
        /// each sample has in common with the reference sample. 
        /// The value is the count of mutations each sample has in 
        /// common with the reference sample.
        /// </summary>
        public SortedDictionary<uint, int> samplesCV { set; get; }

        public void Output(List<uint[]> intervals, object lockOnMe)
        {
            foreach (var item in intervals)
            {
                if (!samplesCV.ContainsKey(item[1]))
                    samplesCV.Add(item[1], 1);
                else
                    samplesCV[item[1]]++;
            }


        }

        public void Output(I interval, List<uint> intervals, object lockOnMe)
        {
            
        }

        public void Output(C left, C right, List<uint> intervals, object lockOnMe)
        {
            throw new NotImplementedException();
        }
    }

}
