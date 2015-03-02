using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI3
{
    internal class CoverSummit<C, I, M, O>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal CoverSummit(Object lockOnMe, BPlusTree<C, B> di3_1R, BPlusTree<BlockKey<C>, BlockValue> di3_2R, ICSOutput<C, I, M, O> outputStrategy, BlockKey<C> left, BlockKey<C> right, int minAcc, int maxAcc)
        {
            _lockOnMe = lockOnMe;
            _di3_1R = di3_1R;
            _di3_2R = di3_2R;
            _determinedLambdas = new Dictionary<uint, bool>();
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
        private ICSOutput<C, I, M, O> _outputStrategy { set; get; }
        private Dictionary<UInt32, bool> _determinedLambdas { set; get; }
        private Object _lockOnMe { set; get; }
        private bool _reserveRightEnds { set; get; }
        private Dictionary<UInt32, bool> _reservedRightEnds { set; get; }
        private Dictionary<UInt32, bool> _leftEndsToBeIgnored { set; get; }
        

        internal void Cover()
        {
            foreach (var block in _di3_2R.EnumerateRange(_left, _right))
                if (_minAcc <= block.Value.maxAccumulation)
                    _Cover(block.Key.leftEnd, block.Key.rightEnd);
        }
        private void _Cover(C left, C right)
        {
            C markedKey = default(C);
            int markedAcc = -1;
            int accumulation = 0;
            _startOfIteration = true;

            foreach (var bookmark in _di3_1R.EnumerateRange(left, right))
            {
                accumulation = bookmark.Value.lambda.Length - bookmark.Value.omega + bookmark.Value.mu;
                UpdateLambdas(bookmark.Value);

                if (markedAcc == -1 &&
                    accumulation >= _minAcc &&
                    accumulation <= _maxAcc)
                {
                    markedKey = bookmark.Key;
                    markedAcc = accumulation;
                    _reserveRightEnds = true;
                }
                else if (markedAcc != -1 &&
                    (accumulation < _minAcc ||
                    accumulation > _maxAcc))
                {
                    if (_rightEndsToFind > 0)
                        Finalize_mu(right);

                    _outputStrategy.Output(markedKey, bookmark.Key, new List<UInt32>(_determinedLambdas.Keys), _lockOnMe);

                    markedKey = default(C);
                    markedAcc = -1;
                    ExcludeRservedRightEnds();
                    _reserveRightEnds = false;                    
                    _excludeRightEndFromFinalization = true;
                }
            }
        }

        internal void Summit()
        {
            foreach (var block in _di3_2R.EnumerateRange(_left, _right))
                if (_minAcc <= block.Value.maxAccumulation)
                    _Summit(block.Key.leftEnd, block.Key.rightEnd);
        }
        private void _Summit(C left, C right)
        {
            C markedKey = default(C);
            int markedAcc = -1;
            int currentAcc = 0;
            int previousAcc = 0;
            _startOfIteration = true;

            foreach (var bookmark in _di3_1R.EnumerateRange(left, right))
            {
                currentAcc = bookmark.Value.lambda.Length - bookmark.Value.omega + bookmark.Value.mu;
                UpdateLambdas(bookmark.Value);

                if (previousAcc < currentAcc &&
                    markedAcc < currentAcc &&
                    currentAcc >= _minAcc &&
                    currentAcc <= _maxAcc)
                {
                    markedKey = bookmark.Key;
                    markedAcc = currentAcc;
                    _reserveRightEnds = true;
                }
                else if (markedAcc > currentAcc ||
                    (markedAcc < currentAcc && (
                    currentAcc < _minAcc ||
                    currentAcc > _maxAcc) &&
                    markedAcc != -1))
                {
                    if (_rightEndsToFind > 0)
                        Finalize_mu(right);

                    _outputStrategy.Output(markedKey, bookmark.Key, new List<UInt32>(_determinedLambdas.Keys), _lockOnMe);

                    markedKey = default(C);
                    markedAcc = -1;
                    ExcludeRservedRightEnds();
                    _reserveRightEnds = false;
                    _excludeRightEndFromFinalization = true;
                }

                previousAcc = currentAcc;
            }
        }

        private void UpdateLambdas(B keyBookmark)
        {
            if (_startOfIteration)
            {
                foreach (var lambda in keyBookmark.lambda)
                    if (lambda.phi == true)
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
                        /// This condition is highly rare to be met.
                        if (_determinedLambdas[lambda.atI] == false)
                            _rightEndsToFind--;

                        if (_reserveRightEnds)
                            _reservedRightEnds.Add(lambda.atI, false);
                        else
                            _determinedLambdas.Remove(lambda.atI);
                    }
                }
            }
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
        private void ExcludeRservedRightEnds()
        {
            foreach (var lambda in _reservedRightEnds)
                _determinedLambdas.Remove(lambda.Key);
            _reservedRightEnds.Clear();
        }
    }
}
