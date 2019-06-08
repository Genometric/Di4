using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;

namespace Genometric.Di4.Inv
{

    internal class StatsCalculator<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {

        public StatsCalculator(BPlusTree<C, B> di4_1R, C left, C right, SortedDictionary<int, int> lambdaSizeDistribution, object lockOnMe)
        {
            _di4_1R = di4_1R;
            _left = left;
            _right = right;
            _lockOnMe = lockOnMe;
            _lambdaSizeDistribution = lambdaSizeDistribution;
        }

        private BPlusTree<C, B> _di4_1R { set; get; }
        private C _left { set; get; }
        private C _right { set; get; }
        private object _lockOnMe { set; get; }
        private SortedDictionary<int, int> _lambdaSizeDistribution { set; get; }

        public void LambdaSizeDistribution()
        {
            SortedDictionary<int, int> tLambdaSizeDis = new SortedDictionary<int, int>();
            foreach (var snapshot in _di4_1R.EnumerateRange(_left, _right))
            {
                if (tLambdaSizeDis.ContainsKey(snapshot.Value.lambda.Count))
                    tLambdaSizeDis[snapshot.Value.lambda.Count]++;
                else
                    tLambdaSizeDis.Add(snapshot.Value.lambda.Count, 1);
            }

            lock (_lockOnMe)
            {
                foreach (var item in tLambdaSizeDis)
                    if (_lambdaSizeDistribution.ContainsKey(item.Key))
                        _lambdaSizeDistribution[item.Key] += item.Value;
                    else
                        _lambdaSizeDistribution.Add(item.Key, item.Value);
            }
        }
    }
}
