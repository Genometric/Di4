using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI4.Inc
{
    internal class AccumulationStats<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal AccumulationStats(BPlusTree<C, B> di4_1R, C left, C right, List<AccEntry<C>> accHistogram, object lockOnMe)
        {
            _di4_1R = di4_1R;
            _left = left;
            _right = right;
            _lockOnMe = lockOnMe;
            _accHistogram = accHistogram;
        }
        internal AccumulationStats(BPlusTree<C, B> di4_1R, C left, C right, SortedDictionary<int, int> accDistribution, object lockOnMe)
        {
            _di4_1R = di4_1R;
            _left = left;
            _right = right;
            _lockOnMe = lockOnMe;
            _accDistribution = accDistribution;
        }
        internal AccumulationStats(BPlusTree<C, B> di4_1R, C left, C right, SortedDictionary<int, double> PDF, object lockOnMe)
        {
            _di4_1R = di4_1R;
            _left = left;
            _right = right;
            _lockOnMe = lockOnMe;
            _PDF = PDF;
            _accDistribution = new SortedDictionary<int, int>();
            _runPDF = true;
        }

        private BPlusTree<C, B> _di4_1R { set; get; }
        private C _left { set; get; }
        private C _right { set; get; }
        private List<AccEntry<C>> _accHistogram { set; get; }
        private SortedDictionary<int, int> _accDistribution { set; get; }
        private SortedDictionary<int, double> _PDF { set; get; }
        private object _lockOnMe { set; get; }
        private bool _runPDF { set; get; }


        internal void AccHistogram()
        {
            C tmp = default(C);
            int tmpAcc = 0;
            bool doBreak = false;
            var _localAccHistogram = new List<AccEntry<C>>();

            /// This is to initeialize tmpDic and tmpAcc. 
            /// It's true that this implementation requires double dichotomic search,
            /// but in long run can perform better than single iteration with condition checks.
            foreach (var bookmark in _di4_1R.EnumerateRange(_left, _right))
            {
                if (doBreak)
                {
                    _left = bookmark.Key;
                    break;
                }
                tmp = bookmark.Key;
                tmpAcc = bookmark.Value.lambda.Count - bookmark.Value.omega + bookmark.Value.mu;
                doBreak = true;
            }

            foreach (var bookmark in _di4_1R.EnumerateRange(_left, _right))
            {
                _localAccHistogram.Add(new AccEntry<C>(tmp, bookmark.Key, tmpAcc));
                tmpAcc = bookmark.Value.lambda.Count - bookmark.Value.omega + bookmark.Value.mu;
                tmp = bookmark.Key;
            }

            lock (_lockOnMe) { _accHistogram.AddRange(_localAccHistogram); }
        }
        internal void AccDistribution()
        {
            int tmpAcc = 0;
            var localDistribution = new Dictionary<int, int>();

            foreach (var bookmark in _di4_1R.EnumerateRange(_left, _right))
            {
                tmpAcc = bookmark.Value.lambda.Count - bookmark.Value.omega + bookmark.Value.mu;
                if (!localDistribution.ContainsKey(tmpAcc))
                {
                    localDistribution.Add(tmpAcc, 1);
                    continue;
                }
                localDistribution[tmpAcc]++;
            }

            lock (_lockOnMe)
            {
                foreach (var item in localDistribution)
                {
                    if (!_accDistribution.ContainsKey(item.Key))
                    {
                        _accDistribution.Add(item.Key, item.Value);
                        continue;
                    }
                    _accDistribution[item.Key] += item.Value;
                }
            }
        }
        internal void PDF()
        {
            AccDistribution();

            lock(_lockOnMe)
            {
                if (!_runPDF) return;

                _runPDF = false;

                double sum = 0;
                foreach (var frequency in _accDistribution)
                    sum += frequency.Value;

                foreach (var kvp in _accDistribution)
                    _PDF.Add(kvp.Key, kvp.Value / sum);
            }
        }
    }
}
