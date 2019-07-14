using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;

namespace Genometric.Di4
{
    internal class CoverSummit<C, I, M, O>
    where C : IComparable<C>, IFormattable
    where I : IInterval<C, M>
    where M : IMetaData, new()
    {
        internal CoverSummit(
            object lockOnMe,
            BPlusTree<C, B> di3_1R,
            BPlusTree<BlockKey<C>, BlockValue> di3_2R,
            IOutput<C, I, M, O> outputStrategy,
            BlockKey<C> left,
            BlockKey<C> right,
            int minAcc,
            int maxAcc)
        {
            _lockOnMe = lockOnMe;
            _di3_1R = di3_1R;
            _di3_2R = di3_2R;
            _determinedLambdas = new Dictionary<uint, Phi>();
            _left = left;
            _right = right;
            _minAcc = minAcc;
            _maxAcc = maxAcc;
            _outputStrategy = outputStrategy;
            _reservedRightEnds = new Dictionary<uint, bool>();
            _leftEndsToBeIgnored = new Dictionary<uint, bool>();
        }

        private BPlusTree<C, B> _di3_1R { set; get; }
        private BPlusTree<BlockKey<C>, BlockValue> _di3_2R { set; get; }
        private BlockKey<C> _left { set; get; }
        private BlockKey<C> _right { set; get; }
        private int _minAcc { set; get; }
        private int _maxAcc { set; get; }
        private int _rightEndsToFind { set; get; }
        private bool _startOfIteration { set; get; }
        private bool _excludeRightEndFromFinalization { set; get; }
        private IOutput<C, I, M, O> _outputStrategy { set; get; }
        private Dictionary<uint, Phi> _determinedLambdas { set; get; }
        private object _lockOnMe { set; get; }
        private bool _reserveRightEnds { set; get; }
        private Dictionary<uint, bool> _reservedRightEnds { set; get; }
        private Dictionary<uint, bool> _leftEndsToBeIgnored { set; get; }

        private C _markedKey { set; get; }
        private int _markedAcc { set; get; }
        private int _accumulation { set; get; }
        private int _currentAcc { set; get; }
        private int _previousAcc { set; get; }



        internal void Cover()
        {
            foreach (var block in _di3_2R.EnumerateRange(_left, _right))
                if (!(_maxAcc < block.Value.boundariesLowerBound || block.Value.boundariesUpperBound < _minAcc)) //_minAcc <= block.Value.boundariesUpperBound)
                {
                    // I'm not sure if this is correct. 
                    _determinedLambdas.Clear();

                    foreach (var atI in block.Value.atI)
                        _determinedLambdas.Add(atI, Phi.LeftEnd);

                    _Cover(block.Key.leftEnd, block.Key.rightEnd);
                }
        }
        private void _Cover(C left, C right)
        {
            _markedKey = default(C);
            _markedAcc = -1;
            _accumulation = 0;
            ////_startOfIteration = true;

            bool itIsOpen = false;

            foreach (var bookmark in _di3_1R.EnumerateRange(left, right))
            {
                _accumulation = bookmark.Value.accumulation;// bookmark.Value.lambda.Count - bookmark.Value.omega + bookmark.Value.mu;
                UpdateLambdas(bookmark.Value);

                if (_markedAcc == -1 &&
                    _accumulation >= _minAcc &&
                    _accumulation <= _maxAcc)
                {
                    _markedKey = bookmark.Key;
                    _markedAcc = _accumulation;
                    _reserveRightEnds = true;
                    itIsOpen = true;
                }
                else if (_markedAcc != -1 &&
                    (_accumulation < _minAcc ||
                    _accumulation > _maxAcc))
                {
                    ////if (_rightEndsToFind > 0)
                    ////    Finalize_mu(right);

                    _outputStrategy.Output(_markedKey, bookmark.Key, new List<uint>(_determinedLambdas.Keys), _lockOnMe);

                    _markedKey = default(C);
                    _markedAcc = -1;
                    ExcludeRservedRightEnds();
                    _reserveRightEnds = false;
                    _excludeRightEndFromFinalization = true;
                    itIsOpen = false;
                }
            }

            if (itIsOpen)
            {
                _outputStrategy.Output(_markedKey, right, new List<uint>(_determinedLambdas.Keys), _lockOnMe);
                ExcludeRservedRightEnds();
                _reserveRightEnds = false;
                _excludeRightEndFromFinalization = true;
            }
        }

        internal void Summit()
        {
            foreach (var block in _di3_2R.EnumerateRange(_left, _right))
                if (!(_maxAcc < block.Value.boundariesLowerBound || block.Value.boundariesUpperBound < _minAcc)) //_minAcc <= block.Value.boundariesUpperBound)
                {
                    // I'm not sure if this is correct. 
                    _determinedLambdas.Clear();

                    foreach (var atI in block.Value.atI)
                        _determinedLambdas.Add(atI, Phi.LeftEnd);

                    _Summit(block.Key.leftEnd, block.Key.rightEnd);
                }
        }
        private void _Summit(C left, C right)
        {
            _markedKey = default(C);
            _markedAcc = -1;
            _currentAcc = 0;
            _previousAcc = 0;
            _startOfIteration = true;

            bool itIsOpen = false;

            foreach (var bookmark in _di3_1R.EnumerateRange(left, right))
            {
                _currentAcc = bookmark.Value.accumulation;// bookmark.Value.lambda.Count - bookmark.Value.omega + bookmark.Value.mu;
                UpdateLambdas(bookmark.Value);

                if (_previousAcc < _currentAcc &&
                    _markedAcc < _currentAcc &&
                    _currentAcc >= _minAcc &&
                    _currentAcc <= _maxAcc)
                {
                    _markedKey = bookmark.Key;
                    _markedAcc = _currentAcc;
                    _reserveRightEnds = true;
                    itIsOpen = true;
                }
                else if (_markedAcc > _currentAcc ||
                    (_markedAcc < _currentAcc && (
                    _currentAcc < _minAcc ||
                    _currentAcc > _maxAcc) &&
                    _markedAcc != -1))
                {
                    //if (_rightEndsToFind > 0)
                    //    Finalize_mu(right);

                    _outputStrategy.Output(_markedKey, bookmark.Key, new List<uint>(_determinedLambdas.Keys), _lockOnMe);

                    _markedKey = default(C);
                    _markedAcc = -1;
                    ExcludeRservedRightEnds();
                    _reserveRightEnds = false;
                    _excludeRightEndFromFinalization = true;
                    itIsOpen = false;
                }

                _previousAcc = _currentAcc;
            }

            if (itIsOpen)
            {
                _outputStrategy.Output(_markedKey, right, new List<uint>(_determinedLambdas.Keys), _lockOnMe);
                ExcludeRservedRightEnds();
                _reserveRightEnds = false;
                _excludeRightEndFromFinalization = true;
            }
        }

        private void UpdateLambdas(B keyBookmark)
        {
            ////if (_startOfIteration)
            ////{
            ////    foreach (var lambda in keyBookmark.lambda)
            ////        if (lambda.phi == Phi.LeftEnd)
            ////            _determinedLambdas.Add(lambda.atI, lambda.phi);
            ////
            ////    _rightEndsToFind = keyBookmark.mu;
            ////    _startOfIteration = false;
            ////}
            ////else
            ////{
                foreach (var lambda in keyBookmark.lambda)
                {
                    if (_determinedLambdas.ContainsKey(lambda.atI) == false)
                        _determinedLambdas.Add(lambda.atI, lambda.phi);

                    if (lambda.phi == Phi.RightEnd)
                    {
                        //if (_determinedLambdas[lambda.atI] == Phi.RightEnd)
           ////             _rightEndsToFind--;

                        if (_reserveRightEnds)
                            _reservedRightEnds.Add(lambda.atI, false);
                        else
                            _determinedLambdas.Remove(lambda.atI);
                    }
                }
            ////}
        }
        private void Finalize_mu(C enumerationStart)
        {
            _leftEndsToBeIgnored.Clear();

            foreach (var bookmark in _di3_1R.EnumerateFrom(enumerationStart))
            {
                if (_excludeRightEndFromFinalization)
                {
                    _excludeRightEndFromFinalization = false;
                    continue;
                }

                foreach (var lambda in bookmark.Value.lambda)
                {
                    if (lambda.phi == Phi.LeftEnd)
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
        private void ExcludeRservedRightEnds()
        {
            foreach (var lambda in _reservedRightEnds)
                _determinedLambdas.Remove(lambda.Key);
            _reservedRightEnds.Clear();
        }
    }
}
