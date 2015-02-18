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
            _intervalsKeys = new Hashtable();
            _lambdas = new List<Lambda>();
        }
        internal HigherOrderFuncs(Object lockOnMe, BPlusTree<C, B> di3_1R, ICSOutput<C, I, M, O> outputStrategy, List<I> intervals, int start, int stop)
        {
            _lockOnMe = lockOnMe;
            _di3_1R = di3_1R;
            _intervalsKeys = new Hashtable();
            _lambdas = new List<Lambda>();
            _intervals = intervals;
            _start = start;
            _stop = stop;
            _outputStrategy = outputStrategy;
        }
        internal HigherOrderFuncs(Object lockOnMe, BPlusTree<C, B> di3_1R, BPlusTree<BlockKey<C>, BlockValue> di3_2R, ICSOutput<C, I, M, O> outputStrategy, BlockKey<C> left, BlockKey<C> right, int minAcc, int maxAcc)
        {
            _lockOnMe = lockOnMe;
            _di3_1R = di3_1R;
            _di3_2R = di3_2R;
            _intervalsKeys = new Hashtable();
            _lambdas = new List<Lambda>();
            _left = left;
            _right = right;
            _minAcc = minAcc;
            _maxAcc = maxAcc;
            _outputStrategy = outputStrategy;
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
        internal ICSOutput<C, I, M, O> outputStrategy { get { return _outputStrategy; } }
        private Hashtable _intervalsKeys { set; get; }
        private List<Lambda> _lambdas { set; get; }
        private Object _lockOnMe { set; get; }

        internal void Cover()
        {
            foreach(var block in _di3_2R.EnumerateRange(_left, _right))
                if(_minAcc <= block.Value.maxAccumulation)                
                    _Cover(block.Key.leftEnd, block.Key.rightEnd);
        }
        private void _Cover(C left, C right)
        {
            C markedKey = default(C);
            int markedAcc = -1;
            byte accumulation = 0;
            _lambdas.Clear();
            _intervalsKeys.Clear();

            foreach (var bookmark in _di3_1R.EnumerateRange(left, right))
            {
                accumulation = (byte)(bookmark.Value.lambda.Count - bookmark.Value.omega);

                if (markedAcc == -1 &&
                    accumulation >= _minAcc &&
                    accumulation <= _maxAcc)
                {
                    markedKey = bookmark.Key;
                    markedAcc = accumulation;
                    UpdateLambdas(bookmark.Value.lambda);
                }
                else if (markedAcc != -1)
                {
                    if (accumulation < _minAcc ||
                        accumulation > _maxAcc)
                    {
                        UpdateLambdas(bookmark.Value.lambda);
                        _outputStrategy.Output(markedKey, bookmark.Key, _lambdas);

                        markedKey = default(C);
                        markedAcc = -1;
                        _lambdas.Clear();
                        _intervalsKeys.Clear();
                    }
                    else if (accumulation >= _minAcc &&
                        accumulation <= _maxAcc)
                    {
                        UpdateLambdas(bookmark.Value.lambda);
                    }
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
            byte accumulation = 0;
            _lambdas.Clear();
            _intervalsKeys.Clear();

            foreach (var bookmark in _di3_1R.EnumerateRange(left, right))
            {
                accumulation = (byte)(bookmark.Value.lambda.Count - bookmark.Value.omega);

                if (markedAcc < accumulation &&
                    accumulation >= _minAcc &&
                    accumulation <= _maxAcc)
                {
                    markedKey = bookmark.Key;
                    markedAcc = accumulation;
                    UpdateLambdas(bookmark.Value.lambda);
                }
                else if (markedAcc > accumulation ||
                    (markedAcc < accumulation && (
                    accumulation < _minAcc ||
                    accumulation > _maxAcc) &&
                    markedAcc != -1))
                {
                    UpdateLambdas(bookmark.Value.lambda);
                    _outputStrategy.Output(markedKey, bookmark.Key, _lambdas);

                    markedKey = default(C);
                    markedAcc = -1;
                    _lambdas.Clear();
                    _intervalsKeys.Clear();
                }
                else if (accumulation >= _minAcc &&
                    accumulation <= _maxAcc &&
                    markedAcc != -1)
                {
                    UpdateLambdas(bookmark.Value.lambda);
                }
            }
        }
        internal void Map()
        {
            int i = 0;
            I reference;

            for (i = _start; i < _stop; i++)
            {
                reference = _intervals[i];
                _lambdas.Clear();
                _intervalsKeys.Clear();

                #region .::.     a quick note     .::.
                /// This iteration starts from a bookmark which it's newKey (i.e., coordinate)
                /// is the minimum >= to reference.currentBlockLeftEnd; and goes to the bookmark which the newKey
                /// is maximum <= to reference.right. Of course if no such blocks are available
                /// this iteration wont iteratre over anything. 
                #endregion
                foreach (var bookmark in _di3_1R.EnumerateRange(reference.left, reference.right))
                    UpdateLambdas(bookmark.Value.lambda);

                _outputStrategy.Output(reference, _lambdas);
            }
        }

        private void UpdateLambdas(ReadOnlyCollection<Lambda> lambdas)
        {
            foreach (var lambda in lambdas)            
                if (!_intervalsKeys.ContainsKey(lambda.atI))
                {
                    _lambdas.Add(lambda);
                    _intervalsKeys.Add(lambda.atI, "Hmd");
                }
        }
    }
}
