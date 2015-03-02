using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI3
{
    internal class Map<C, I, M, O>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal Map(Object lockOnMe, BPlusTree<C, B> di3_1R, ICSOutput<C, I, M, O> outputStrategy, List<I> intervals, int start, int stop)
        {
            _lockOnMe = lockOnMe;
            _di3_1R = di3_1R;
            _determinedLambdas = new Dictionary<uint, bool>();
            _intervals = intervals;
            _start = start;
            _stop = stop;
            _outputStrategy = outputStrategy;            
            _reservedRightEnds = new Dictionary<uint, bool>();
            _leftEndsToBeIgnored = new Dictionary<uint, bool>();
        }

        private BPlusTree<C, B> _di3_1R { set; get; }
        private int _start { set; get; }
        private int _stop { set; get; }
        private int _rightEndsToFind { set; get; }
        private bool _startOfIteration { set; get; }
        private List<I> _intervals { set; get; }
        private ICSOutput<C, I, M, O> _outputStrategy { set; get; }
        private Dictionary<UInt32, bool> _determinedLambdas { set; get; }
        private Object _lockOnMe { set; get; }
        private Dictionary<UInt32, bool> _reservedRightEnds { set; get; }
        private Dictionary<UInt32, bool> _leftEndsToBeIgnored { set; get; }
        private I _reference { set; get; }
        private C _processingCoordinate { set; get; }


        internal void Run()
        {
            bool iterated = false;
            _startOfIteration = true;

            for (int i = _start; i < _stop; i++)
            {
                _reference = _intervals[i];

                foreach (var bookmark in _di3_1R.EnumerateFrom(_reference.left))
                {
                    _processingCoordinate = bookmark.Key;

                    if (bookmark.Key.CompareTo(_reference.right) == -1) // bookmark.key < _reference.right
                        UpdateLambdas(bookmark.Value);
                    else if (bookmark.Value.mu != 0 || bookmark.Value.omega != 0)
                    {
                        if (!iterated) _rightEndsToFind = bookmark.Value.mu;
                        Finalize_mu(bookmark.Key);
                        break;
                    }
                    else break;
                    iterated = true;
                }

                _outputStrategy.Output(_reference, new List<UInt32>(_determinedLambdas.Keys), _lockOnMe);

                _determinedLambdas.Clear();
                _reservedRightEnds.Clear();
                _startOfIteration = true;
                iterated = false;
            }
        }
        private void UpdateLambdas(B keyBookmark)
        {
            if (_startOfIteration)
            {
                foreach (var lambda in keyBookmark.lambda)
                    if (lambda.phi == true)
                        _determinedLambdas.Add(lambda.atI, lambda.phi);
                    else if (_reference.left.CompareTo(_processingCoordinate) == -1)
                        _determinedLambdas.Add(lambda.atI, lambda.phi);

                _rightEndsToFind = keyBookmark.mu;
                _startOfIteration = false;
            }
            else
            {
                foreach (var lambda in keyBookmark.lambda)
                {
                    if (_determinedLambdas.ContainsKey(lambda.atI) == false)
                        _determinedLambdas.Add(lambda.atI, lambda.phi);

                    if (lambda.phi == false)
                    {
                        if (_determinedLambdas[lambda.atI] == false)
                            _rightEndsToFind--;

                        _reservedRightEnds.Add(lambda.atI, false);
                    }
                }
            }
        }
        private void Finalize_mu(C enumerationStart)
        {
            _leftEndsToBeIgnored.Clear();

            foreach (var bookmark in _di3_1R.EnumerateFrom(enumerationStart))
            {
                foreach (var lambda in bookmark.Value.lambda)
                {
                    if (lambda.phi == true)
                    {
                        _leftEndsToBeIgnored.Add(lambda.atI, true);
                        continue;
                    }
                    if (_leftEndsToBeIgnored.ContainsKey(lambda.atI))
                    {
                        _leftEndsToBeIgnored.Remove(lambda.atI);
                        continue;
                    }
                    if (!_determinedLambdas.ContainsKey(lambda.atI))
                    {
                        _determinedLambdas.Add(lambda.atI, lambda.phi);
                        _rightEndsToFind--;
                    }
                }
                if (_rightEndsToFind == 0) break;
            }
        }
    }
}
