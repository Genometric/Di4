using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Polimi.DEIB.VahidJalili.DI3
{
    internal class HigherOrderFuncs<C, I, M, O>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal HigherOrderFuncs(Object lockOnMe, BPlusTree<C, B> di3)
        {
            _lockOnMe = lockOnMe;
            _di3_1R = di3;
            _determinedLambdas = new Dictionary<uint, bool>();
            _reservedRightEnds = new Dictionary<uint, bool>();
        }
        internal HigherOrderFuncs(Object lockOnMe, BPlusTree<C, B> di3_1R, ICSOutput<C, I, M, O> outputStrategy, List<I> intervals, int start, int stop)
        {
            _lockOnMe = lockOnMe;
            _di3_1R = di3_1R;
            _determinedLambdas = new Dictionary<uint, bool>();
            _intervals = intervals;
            _start = start;
            _stop = stop;
            _outputStrategy = outputStrategy;
            _reservedRightEnds = new Dictionary<uint, bool>();
        }
        internal HigherOrderFuncs(Object lockOnMe, BPlusTree<C, B> di3_1R, BPlusTree<BlockKey<C>, BlockValue> di3_2R, ICSOutput<C, I, M, O> outputStrategy, BlockKey<C> left, BlockKey<C> right, int minAcc, int maxAcc)
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
        }

        private BPlusTree<C, B> _di3_1R { set; get; }
        private BPlusTree<BlockKey<C>, BlockValue> _di3_2R { set; get; }
        private int _start { set; get; }
        private int _stop { set; get; }
        private BlockKey<C> _left { set; get; }
        private BlockKey<C> _right { set; get; }
        private int _minAcc { set; get; }
        private int _maxAcc { set; get; }
        private List<I> _intervals { set; get; }
        private ICSOutput<C, I, M, O> _outputStrategy { set; get; }
        private Dictionary<UInt32, bool> _determinedLambdas { set; get; }
        private Object _lockOnMe { set; get; }
        private bool _reserveRightEnds { set; get; }
        private Dictionary<UInt32, bool> _reservedRightEnds { set; get; }

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

            foreach (var bookmark in _di3_1R.EnumerateRange(left, right))
            {
                accumulation = bookmark.Value.lambda.Count - bookmark.Value.omega + bookmark.Value.mu;
                UpdateLambdas(bookmark.Key, bookmark.Value);

                if (markedAcc == -1 &&
                    accumulation >= _minAcc &&
                    accumulation <= _maxAcc)
                {
                    markedKey = bookmark.Key;
                    markedAcc = accumulation;
                    _reserveRightEnds = true;

                    //UpdateLambdas(keyBookmark.Value.lambda);
                    //UpdateLambdas(bookmark.Key, bookmark.Value);
                }
                else if (markedAcc != -1)
                {
                    if (accumulation < _minAcc ||
                        accumulation > _maxAcc)
                    {
                        //UpdateLambdas(keyBookmark.Value.lambda);
                        //UpdateLambdas(bookmark.Key, bookmark.Value);
                        _outputStrategy.Output(markedKey, bookmark.Key, new List<UInt32>(_determinedLambdas.Keys), _lockOnMe);

                        markedKey = default(C);
                        markedAcc = -1;
                        //_lambdas.Clear();
                        //_determinedLambdas.Clear();
                        ExcludeRservedRightEnds();
                        _reserveRightEnds = false;
                    }
                    //else if (currentAcc >= _minAcc &&
                       // currentAcc <= _maxAcc)
                    //{
                        //UpdateLambdas(keyBookmark.Value.lambda);
                        //UpdateLambdas(bookmark.Key, bookmark.Value);
                    //}
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

            foreach (var bookmark in _di3_1R.EnumerateRange(left, right))
            {
                currentAcc = bookmark.Value.lambda.Count - bookmark.Value.omega + bookmark.Value.mu;
                UpdateLambdas(bookmark.Key, bookmark.Value);

                if (previousAcc < currentAcc &&
                    markedAcc < currentAcc &&
                    currentAcc >= _minAcc &&
                    currentAcc <= _maxAcc)
                {
                    markedKey = bookmark.Key;
                    markedAcc = currentAcc;
                    //UpdateLambdas(keyBookmark.Value.lambda);
                    //UpdateLambdas(bookmark.Key, bookmark.Value);
                    _reserveRightEnds = true;

                    /// This section helps avoiding intervals determined previously and 
                    /// ended at current start of summit region. This can happen when an 
                    /// interval's left-end is colocallized with another interval's right-end.
                    //if (bookmark.Value.omega > 0)
                        //foreach (var lambda in bookmark.Value.lambda)
                            //if (lambda.phi == false)
                                //_determinedLambdas.Remove(lambda.atI);
                }
                else if (markedAcc > currentAcc ||
                    (markedAcc < currentAcc && (
                    currentAcc < _minAcc ||
                    currentAcc > _maxAcc) &&
                    markedAcc != -1))
                {
                    //UpdateLambdas(keyBookmark.Value.lambda);
                    //UpdateLambdas(bookmark.Key, bookmark.Value);
                    _outputStrategy.Output(markedKey, bookmark.Key, new List<UInt32>(_determinedLambdas.Keys), _lockOnMe);

                    markedKey = default(C);
                    markedAcc = -1;
                    //_lambdas.Clear();
                    //_determinedLambdas.Clear();
                    ExcludeRservedRightEnds();
                    _reserveRightEnds = false;
                }
                /*else if (currentAcc >= _minAcc &&
                    currentAcc <= _maxAcc &&
                    markedAcc != -1)
                {
                    //UpdateLambdas(keyBookmark.Value.lambda);
                    //UpdateLambdas(bookmark.Key, bookmark.Value);
                }*/

                previousAcc = currentAcc;
            }
        }
        internal void Map()
        {
            int i = 0;
            I reference;
            _reserveRightEnds = true;

            for (i = _start; i < _stop; i++)
            {
                reference = _intervals[i];

                #region .::.     a quick note     .::.
                /// This iteration starts from a keyBookmark which it's newKey (i.e., coordinate)
                /// is the minimum >= to reference.currentBlockLeftEnd; and goes to the keyBookmark which the newKey
                /// is maximum <= to reference.right. Of course if no such blocks are available
                /// this iteration wont iteratre over anything. 
                #endregion
                foreach (var bookmark in _di3_1R.EnumerateRange(reference.left, reference.right))
                    //UpdateLambdas(keyBookmark.Value.lambda);
                    UpdateLambdas(bookmark.Key, bookmark.Value);

                _outputStrategy.Output(reference, new List<UInt32>(_determinedLambdas.Keys), _lockOnMe);

                _determinedLambdas.Clear();
                _reservedRightEnds.Clear();

            }
        }

        private void UpdateLambdas(C coordinate, B keyBookmark) //ReadOnlyCollection<Lambda> lambdas)
        {
            if (_determinedLambdas.Count == 0)
            {
                foreach (var lambda in keyBookmark.lambda)
                    if (lambda.phi == true)
                        _determinedLambdas.Add(lambda.atI, lambda.phi);

                /// It's quite rare for this condition to be met under Cover/Summit functions, 
                /// however, it's highly probable with Map operation.
                if (keyBookmark.mu > 0)
                {
                    int mu = keyBookmark.mu;
                    bool skipFirst = true;
                    var tmpLambdas = new Dictionary<UInt32, bool>();
                    foreach (var bookmark in _di3_1R.EnumerateFrom(coordinate))
                    {
                        if (skipFirst)
                        {
                            skipFirst = false;
                            continue;
                        }

                        foreach (var lambda in bookmark.Value.lambda)
                        {
                            if (lambda.phi == true)
                            {
                                tmpLambdas.Add(lambda.atI, true);
                                continue;
                            }

                            if (tmpLambdas.ContainsKey(lambda.atI))
                            {
                                tmpLambdas.Remove(lambda.atI);
                                continue;
                            }

                            if (!_determinedLambdas.ContainsKey(lambda.atI))
                            {
                                _determinedLambdas.Add(lambda.atI, lambda.phi);
                                mu--;
                            }
                        }

                        if (mu == 0) break;
                    }
                }
            }
            else
            {
                foreach (var lambda in keyBookmark.lambda)
                    if (lambda.phi) // Do I really need following condition? : && _determinedLambdas.ContainsKey(lambda.atI) == false)
                        _determinedLambdas.Add(lambda.atI, lambda.phi);
                    else if (_reserveRightEnds)
                        _reservedRightEnds.Add(lambda.atI, false);
                    else
                        _determinedLambdas.Remove(lambda.atI);

                /*
                if (keyBookmark.omega > 0)
                {
                    if (_reserveRightEnds)
                    {
                        foreach (var lambda in keyBookmark.lambda)
                            if (!lambda.phi)
                                _reservedRightEnds.Add(lambda.atI, false);
                    }
                    else
                    {
                        foreach (var lambda in keyBookmark.lambda)
                            if (!lambda.phi)
                                _determinedLambdas.Remove(lambda.atI);
                    }
                }*/
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
