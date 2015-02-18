﻿using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Polimi.DEIB.VahidJalili.DI3
{
    internal class FirstOrderFuncs<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal FirstOrderFuncs(BPlusTree<C, B> di3_1R, C left, C right, List<AccEntry<C>> accHistogram, Object lockOnMe)
        {
            _di3_1R = di3_1R;
            _left = left;
            _right = right;
            _lockOnMe = lockOnMe;
            _accHistogram = accHistogram;
        }
        internal FirstOrderFuncs(BPlusTree<C, B> di3_1R, C left, C right, SortedDictionary<int, int> accDistribution, Object lockOnMe)
        {
            _di3_1R = di3_1R;
            _left = left;
            _right = right;
            _lockOnMe = lockOnMe;
            _accDistribution = accDistribution;
        }

        private BPlusTree<C, B> _di3_1R { set; get; }
        private C _left { set; get; }
        private C _right { set; get; }
        private List<AccEntry<C>> _accHistogram { set; get; }
        private SortedDictionary<int, int> _accDistribution { set; get; }        
        private Object _lockOnMe { set; get; }


        internal void AccumulationHistogram()
        {
            C tmp = default(C);
            int tmpAcc = 0;
            bool doBreak = false;
            var _localAccHistogram = new List<AccEntry<C>>();

            /// This is to initeialize tmp and tmpAcc. 
            /// It's true that this implementation requires double dichotomic search,
            /// but in long run can perform better than single iteration with condition checks.
            foreach (var bookmark in _di3_1R.EnumerateFrom(_left))
            {
                if (doBreak)
                {
                    _left = bookmark.Key;
                    break;
                }
                tmp = bookmark.Key;
                tmpAcc = bookmark.Value.lambda.Count - bookmark.Value.omega;
                doBreak = true;
            }

            foreach (var bookmark in _di3_1R.EnumerateRange(_left, _right))
            {
                _localAccHistogram.Add(new AccEntry<C>(tmp, bookmark.Key, tmpAcc));
                tmpAcc = bookmark.Value.lambda.Count - bookmark.Value.omega;
                tmp = bookmark.Key;
            }

            lock(_lockOnMe)
            {
                /*if (_accHistogram == null) _accHistogram = _localAccHistogram;
                else _accHistogram = */_accHistogram.AddRange(_localAccHistogram);
            }
        }
        internal void AccumulationDistribution()
        {
            int tmpAcc = 0;
            var localDistribution = new Dictionary<int, int>();

            foreach (var bookmark in _di3_1R.EnumerateRange(_left, _right))
            {
                tmpAcc = bookmark.Value.lambda.Count - bookmark.Value.omega;
                if (!localDistribution.ContainsKey(tmpAcc))
                {
                    localDistribution.Add(tmpAcc, 0);
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
    }



    public class MyClass
    {
        int A = 0;
        public MyClass con1(int a, int b)
        {
            A = a;
            return new MyClass();
        }
    }
}
