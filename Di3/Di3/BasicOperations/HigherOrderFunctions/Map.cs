using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace Polimi.DEIB.VahidJalili.DI3
{
    internal class Map<C, I, M, O>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal Map(
            object lockOnMe,
            BPlusTree<C, IIB> di3_1R,
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
            _di3_1R = di3_1R;
            _lockOnMe = lockOnMe;
            _intervals = intervals;
            _outputStrategy = outputStrategy;
            _gapIntervals = new Dictionary<uint, bool>();
            _determinedLambdas = new Dictionary<int, Dictionary<uint, Phi>>();
        }

        private BPlusTree<C, IIB> _di3_1R { set; get; }
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
        private IEnumerator<KeyValuePair<C, IIB>> _di3Enumerator { set; get; }
        private Dictionary<int, Dictionary<uint, Phi>> _determinedLambdas { set; get; }
        private C _nextLeftEnd { set; get; }
        private C _nextRightEnd { set; get; }
        private bool _nextRefExists { set; get; }
        private C _UDF { set; get; }
        private C _DDF { set; get; }
        


        internal void Run()
        {
            for (_referenceIndex = _start; _referenceIndex < _stop; _referenceIndex++)
            {
                _reference = _intervals[_referenceIndex];
                _di3Enumerator = _di3_1R.EnumerateFrom(_reference.left).GetEnumerator();
                _di3Enumerator.MoveNext();

                Initialize();

                while (_di3Enumerator.MoveNext())
                {
                    if (_di3Enumerator.Current.Key.CompareTo(_reference.right) != -1) // set it to "== 1" to make right-end inclusive.
                    {
                        if (_rightEndsToFind == 0)
                            break;
                        if (TraverseGap())
                            break;
                    }

                    ProcessCurrentBookmark();
                }

                foreach (var refLambdas in _determinedLambdas)
                    _outputStrategy.Output(_intervals[refLambdas.Key], new List<UInt32>(refLambdas.Value.Keys), _lockOnMe);
                _determinedLambdas.Clear();
            }
        }
        private void Initialize()
        {
            ///                   Reference :  .....███████████████████.............
            /// Enumerator current position :  .....■........■........■.......■.....
            ///                                    [A]      [B]      [C]     [D]
            /// [A] : overlaps the Left-end of the reference
            /// [B] : falls between Left and Right-ends of the reference
            /// [C] : overlaps the Right-end of the reference
            /// [D] : falls after the Right-end of the reference.

            _determinedLambdas.Add(_referenceIndex, new Dictionary<uint, Phi>());
            _rightEndsToFind = _di3Enumerator.Current.Value.mu;

            // [A]
            if (_di3Enumerator.Current.Key.CompareTo(_reference.left) == 0)
            {
                foreach (var lambda in _di3Enumerator.Current.Value.lambda)
                    if (lambda.phi == Phi.LeftEnd)
                        _determinedLambdas[_referenceIndex].Add(lambda.atI, Phi.LeftEnd);
                return;
            }
            switch (_di3Enumerator.Current.Key.CompareTo(_reference.right))
            {
                case -1: // [B]
                    foreach (var lambda in _di3Enumerator.Current.Value.lambda)
                        _determinedLambdas[_referenceIndex].Add(lambda.atI, lambda.phi);
                    return;

                case 0: // [C]
                case 1: // [D]
                    foreach (var lambda in _di3Enumerator.Current.Value.lambda)
                        if (lambda.phi == Phi.LeftEnd)
                            _gapIntervals.Add(lambda.atI, true);
                        else
                            _determinedLambdas[_referenceIndex].Add(lambda.atI, Phi.RightEnd);
                    return;
            }
        }
        private bool TraverseGap()
        {
            UpdateNextRefEnds();

            do
            {
                while (_di3Enumerator.Current.Key.CompareTo(_nextLeftEnd) != -1 && _nextRefExists)
                {
                    _referenceIndex++;
                    _reference = _intervals[_referenceIndex];
                    _determinedLambdas.Add(_referenceIndex, new Dictionary<uint, Phi>());

                    foreach (var lambda in _gapIntervals) // The gapIntervals contains only leftEnds (i.e., phi = true)
                        _determinedLambdas[_referenceIndex].Add(lambda.Key, Phi.LeftEnd);
                    _gapIntervals.Clear();

                    foreach (var lambda in _determinedLambdas[_referenceIndex - 1])
                        if (lambda.Value == Phi.LeftEnd)
                            _determinedLambdas[_referenceIndex].Add(lambda.Key, lambda.Value);

                    if (_di3Enumerator.Current.Key.CompareTo(_nextRightEnd) == 1)
                        UpdateNextRefEnds();
                    else
                        return false;
                }

                foreach (var lambda in _di3Enumerator.Current.Value.lambda)
                    if (lambda.phi == Phi.LeftEnd)
                        _gapIntervals.Add(lambda.atI, true);
                    else
                    {
                        if (_gapIntervals.Remove(lambda.atI))
                            continue;
                        _aMuIsFound = true;
                        foreach (var refLambdas in _determinedLambdas)
                            if (refLambdas.Value.ContainsKey(lambda.atI))
                            {
                                _aMuIsFound = false;
                                refLambdas.Value[lambda.atI] = Phi.RightEnd;
                            }
                        if (_aMuIsFound)
                        {
                            _rightEndsToFind--;
                            foreach (var refLambdas in _determinedLambdas)
                                if (!refLambdas.Value.ContainsKey(lambda.atI))
                                    refLambdas.Value.Add(lambda.atI, Phi.RightEnd);
                        }
                    }

                if (_rightEndsToFind == 0)
                    return true;

            } while (_di3Enumerator.MoveNext());

            return true;
        }
        private void ProcessCurrentBookmark()
        {
            foreach (var lambda in _di3Enumerator.Current.Value.lambda)
            {
                if (lambda.phi == Phi.LeftEnd)
                {
                    _determinedLambdas[_referenceIndex].Add(lambda.atI, Phi.LeftEnd);
                }
                else
                {
                    _aMuIsFound = true;
                    foreach (var refLambdas in _determinedLambdas)
                        if (refLambdas.Value.ContainsKey(lambda.atI))
                        {
                            _aMuIsFound = false;
                            refLambdas.Value[lambda.atI] = Phi.RightEnd;
                        }

                    if (_aMuIsFound)
                    {
                        _rightEndsToFind--;
                        foreach (var refLambdas in _determinedLambdas)
                            if (!refLambdas.Value.ContainsKey(lambda.atI))
                                refLambdas.Value.Add(lambda.atI, Phi.RightEnd);
                    }
                }
            }
        }

        private void UpdateNextRefEnds()
        {
            if (_referenceIndex + 1 < _stop &&
                _referenceIndex + 1 < _intervals.Count)
            {
                _nextLeftEnd = _intervals[_referenceIndex + 1].left;
                _nextRightEnd = _intervals[_referenceIndex + 1].right;
                _nextRefExists = true;
            }
            else
            {
                _nextRefExists = false;
            }
        }
    }
}
