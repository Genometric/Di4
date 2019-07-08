using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;

namespace Genometric.Di4
{
    internal class VariationAnalysis<C, I, M, O>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal VariationAnalysis(
            object lockOnMe,
            BPlusTree<C, B> di4_1R,
            IOutput<C, I, M, O> outputStrategy,
            List<I> intervals,
            int start,
            int stop,
            C UDF,
            C DDF,
            SortedDictionary<uint, int> result)
        {
            _UDF = UDF;
            _DDF = DDF;
            _stop = stop;
            _start = start;
            _di4_1R = di4_1R;
            _lockOnMe = lockOnMe;
            _intervals = intervals;
            _outputStrategy = outputStrategy;

            _result = result;
        }

        private BPlusTree<C, B> _di4_1R { set; get; }
        private int _start { set; get; }
        private int _stop { set; get; }
        private object _lockOnMe { set; get; }
        private List<I> _intervals { set; get; }
        private IOutput<C, I, M, O> _outputStrategy { set; get; }
        private C _UDF { set; get; }
        private C _DDF { set; get; }
        private SortedDictionary<uint, int> _result { set; get; }

        internal void Run()
        {
            var voutput = new VCOutputStrategy<C, I, M, uint>();
            new Tmp__Map__for__VA<C, I, M, uint>(_lockOnMe, _di4_1R, voutput, _intervals, _start, _stop, _UDF, _DDF).Run();

            _result = voutput.samplesCV;


            
        }
    }



    public class VOUTPUT<C, I, M> : IOutput<C, I, M, uint>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        public List<uint> output { set; get; }

        public void Output(List<uint[]> intervals, object lockOnMe)
        {
            throw new NotImplementedException();
        }

        public void Output(I interval, List<uint> intervals, object lockOnMe)
        {
            throw new NotImplementedException();
        }

        public void Output(C left, C right, List<uint> intervals, object lockOnMe)
        {
            throw new NotImplementedException();
        }

        public void Output(I interval, int count, object lockOnMe)
        {
            throw new NotImplementedException();
        }
    }
}
