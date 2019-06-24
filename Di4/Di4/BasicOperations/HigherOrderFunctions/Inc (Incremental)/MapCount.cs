using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;

namespace Genometric.Di4
{
    internal class MapCount<C, I, M, O>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {

        internal MapCount(
            object lockOnMe,
            BPlusTree<C, B> di4_1R,
            IOutput<C, I, M, O> outputStrategy,
            List<I> intervals,
            int start,
            int stop,
            C UDF,
            C DDF)
        {
            _UDF = UDF;
            _DDF = DDF;
            _stop = stop;
            _start = start;
            _di4_1R = di4_1R;
            _lockOnMe = lockOnMe;
            _intervals = intervals;
            _outputStrategy = outputStrategy;
            _gapIntervals = new Dictionary<uint, bool>();
            _lambdas = new HashSet<uint>();
        }


        private BPlusTree<C, B> _di4_1R { set; get; }
        private int _start { set; get; }
        private int _stop { set; get; }
        private bool _aMuIsFound { set; get; }
        private int _referenceIndex { set; get; }
        private int _rightEndsToFind { set; get; }
        private I _reference { set; get; }
        private List<I> _intervals { set; get; }
        private object _lockOnMe { set; get; }
        private IOutput<C, I, M, O> _outputStrategy { set; get; }
        private Dictionary<uint, bool> _gapIntervals { set; get; }
        private IEnumerator<KeyValuePair<C, B>> _di4Enumerator { set; get; }
        private HashSet<uint> _lambdas { set; get; }
        private C _nextLeftEnd { set; get; }
        private C _nextRightEnd { set; get; }
        private bool _nextRefExists { set; get; }
        private C _UDF { set; get; }
        private C _DDF { set; get; }


        /// <summary>
        /// the number of intervals whose left and right are between
        /// the left and right end of a reference interval.
        /// </summary>
        private int coveredIntervals;

        internal void Run()
        {
            for (_referenceIndex = _start; _referenceIndex < _stop; _referenceIndex++)
            {
                coveredIntervals = 0;
                _reference = _intervals[_referenceIndex];

                _di4Enumerator = _di4_1R.EnumerateFrom(_reference.left).GetEnumerator();
                _di4Enumerator.MoveNext();

                _rightEndsToFind = _di4Enumerator.Current.Value.mu;
                if (_di4Enumerator.Current.Key.CompareTo(_reference.left) == 0)
                {
                    foreach (var lambda in _di4Enumerator.Current.Value.lambda)
                        if (lambda.phi == Phi.LeftEnd)
                            _lambdas.Add(lambda.atI);
                }
                else
                {
                    switch (_di4Enumerator.Current.Key.CompareTo(_reference.right))
                    {
                        case -1:
                            foreach (var lambda in _di4Enumerator.Current.Value.lambda)
                                if (lambda.phi == Phi.RightEnd)
                                    coveredIntervals++;
                                else
                                    _lambdas.Add(lambda.atI);
                            break;
                        case 0:
                        case 1:
                            foreach (var lambda in _di4Enumerator.Current.Value.lambda)
                                if (lambda.phi == Phi.RightEnd)
                                    coveredIntervals++;
                            break;
                    }
                }

                while (_di4Enumerator.MoveNext())
                {
                    if (_di4Enumerator.Current.Key.CompareTo(_reference.right) != -1) // set it to "== 1" to make right-end inclusive.
                        break;

                    foreach (var lambda in _di4Enumerator.Current.Value.lambda)
                        if (_lambdas.Remove(lambda.atI))
                            coveredIntervals++;
                        else
                        {
                            if (lambda.phi == Phi.RightEnd)
                                _rightEndsToFind--;
                            else
                                _lambdas.Add(lambda.atI);
                        }
                }

                _outputStrategy.Output(_reference, _lambdas.Count + _rightEndsToFind + coveredIntervals, _lockOnMe);
                _lambdas.Clear();
            }
        }

    }
}
