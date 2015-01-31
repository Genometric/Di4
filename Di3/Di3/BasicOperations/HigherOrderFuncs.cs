using CSharpTest.Net.Collections;
using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DI3
{
    internal class HigherOrderFuncs<C, I, M, O>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal HigherOrderFuncs(BPlusTree<C, B> di3)
        {
            _di3 = di3;
            _intervalsKeys = new Hashtable();
            _lambdas = new List<Lambda>();
        }
        internal HigherOrderFuncs(BPlusTree<C, B> di3, ICSOutput<C, I, M, O> outputStrategy, List<I> intervals, int start, int stop)
        {
            _di3 = di3;
            _intervalsKeys = new Hashtable();
            _lambdas = new List<Lambda>();
            _intervals = intervals;
            _start = start;
            _stop = stop;
            _outputStrategy = outputStrategy;
        }
        internal HigherOrderFuncs(BPlusTree<C, B> di3, ICSOutput<C, I, M, O> outputStrategy, C left, C right, int minAcc, int maxAcc)
        {
            _di3 = di3;
            _intervalsKeys = new Hashtable();
            _lambdas = new List<Lambda>();
            _left = left;
            _right = right;
            _minAcc = minAcc;
            _maxAcc = maxAcc;
            _outputStrategy = outputStrategy;
        }

        private BPlusTree<C, B> _di3 { set; get; }
        private int _start { set; get; }
        private int _stop { set; get; }
        private C _left { set; get; }
        private C _right { set; get; }
        private int _minAcc { set; get; }
        private int _maxAcc { set; get; }
        private List<I> _intervals { set; get; }
        private ICSOutput<C, I, M, O> _outputStrategy { set; get; }
        internal ICSOutput<C, I, M, O> outputStrategy { get { return _outputStrategy; } }
        private Hashtable _intervalsKeys { set; get; }
        private List<Lambda> _lambdas { set; get; }

        internal void Cover()
        {
            C markedKey = default(C);
            int markedAcc = -1;
            byte accumulation = 0;
            _lambdas.Clear();
            _intervalsKeys.Clear();

            foreach (var bookmark in _di3.EnumerateRange(_left, _right))
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
            C markedKey = default(C);
            int markedAcc = -1;
            byte accumulation = 0;
            _lambdas.Clear();
            _intervalsKeys.Clear();

            foreach (var bookmark in _di3.EnumerateFrom(_di3.First().Key))
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
                /// This iteration starts from a bookmark which it's key (i.e., coordinate)
                /// is the minimum >= to reference.left; and goes to the bookmark which the key
                /// is maximum <= to reference.right. Of course if no such blocks are available
                /// this iteration wont iteratre over anything. 
                #endregion
                foreach (var block in _di3.EnumerateRange(reference.left, reference.right))
                {
                    UpdateLambdas(block.Value.lambda);
                }

                _outputStrategy.Output(reference, _lambdas);
            }

            //return _outputStrategy.output;
        }

        private void UpdateLambdas(ReadOnlyCollection<Lambda> newLambdas)
        {
            foreach (var item in newLambdas)
            {
                if (//bookmark.phi != 'R' &&
                    !_intervalsKeys.ContainsKey(item.atI))
                {
                    _lambdas.Add(item);
                    _intervalsKeys.Add(item.atI, "Hmd");
                }
            }
        }
    }
}
