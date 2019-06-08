using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;

namespace Genometric.Di4.Di4B
{
    class VariantAnalysisOutputStrategy<C, I, M> : IOutput<C, I, M, Output<C, I, M>>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>, IFormattable, new()
        where M : IMetaData, IFormattable, new()
    {
        public VariantAnalysisOutputStrategy()
        {
            output = new List<Output<C, I, M>>();
            samplesCV = new SortedDictionary<uint, int>();
        }

        public List<Output<C, I, M>> output { set; get; }

        void IOutput<C, I, M, Output<C, I, M>>.Output(C left, C right, List<UInt32> intervals, Object lockOnMe)
        {
            throw new NotImplementedException();
        }
        void IOutput<C, I, M, Output<C, I, M>>.Output(I interval, List<UInt32> intervals, Object lockOnMe)
        {
            throw new NotImplementedException();
        }

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

        public void Output(I interval, int count, object lockOnMe)
        {
            throw new NotImplementedException();
        }
    }

}
