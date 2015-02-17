using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Polimi.DEIB.VahidJalili.DI3
{
    internal class FirstOrderFuncs<C, I, M, O>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal FirstOrderFuncs(BPlusTree<C, B> di3_1R, C left, C right, ConcurrentDictionary<C[], int> results)
        {
            _di3_1R = di3_1R;
            _left = left;
            _right = right;
            _results = results;
        }

        private BPlusTree<C, B> _di3_1R { set; get; }
        private C _left { set; get; }
        private C _right { set; get; }
        private ConcurrentDictionary<C[], int> _results { set; get; }
        

        internal void AccumulationHistogram()
        {
            C[] previous = new C[2];
            C[] current = new C[2];

            bool pingPong = true;
            C tmp = default(C);
            int tmpAcc = 0;

            foreach(var bookmark in _di3_1R.EnumerateRange(_left, _right))
            {
                switch(pingPong)
                {
                    case true:
                        tmp = bookmark.Key;
                        tmpAcc = bookmark.Value.lambda.Count - bookmark.Value.omega;
                        pingPong = false;
                        break;

                    case false:
                        _results.TryAdd(new C[] { tmp, bookmark.Key }, tmpAcc);
                        pingPong = true;
                        break;
                }

            }
        }
    }
}
