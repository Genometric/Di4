using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.DI4.AuxiliaryComponents.Inc;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI4.Inc
{
    internal class CoverSummit_NEWEST<C, I, M, O>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal CoverSummit_NEWEST(
            object lockOnMe,
            BPlusTree<C, B> di4_1R,
            BPlusTree<BlockKey<C>, BlockValue> di4_2R,
            IOutput<C, I, M, O> outputStrategy,
            BlockKey<C> left,
            BlockKey<C> right,
            int minAcc,
            int maxAcc, 
            int totalBookmarkCount)
        {
            _di4_1R = di4_1R;
            _di4_2R = di4_2R;
            _left = left;
            _right = right;
            _minAcc = minAcc;
            _maxAcc = maxAcc;
            _regionType = RegionType.Candidate;
            _decompositionStack = new DecompositionStack<C, I, M, O>(outputStrategy, lockOnMe);
            _decomposer = new Decomposer<C, I, M, O>(outputStrategy, lockOnMe);

            _totalBookmarkCount = totalBookmarkCount;
        }

        private BPlusTree<C, B> _di4_1R { set; get; }
        private BPlusTree<BlockKey<C>, BlockValue> _di4_2R { set; get; }
        private BlockKey<C> _left { set; get; }
        private BlockKey<C> _right { set; get; }
        private int _minAcc { set; get; }
        private int _maxAcc { set; get; }
        private int _markedAcc { set; get; }
        private int _accumulation { set; get; }
        private int _previousAcc { set; get; }
        private DecompositionStack<C, I, M, O> _decompositionStack { set; get; }
        private Decomposer<C, I, M, O> _decomposer { set; get; }
        private RegionType _regionType { set; get; }



        private int _totalBookmarkCount { set; get; }


        internal void Cover()
        {
            //int tCounter = 0, pCounter = 0;
            //count = 0;
            foreach (var block in _di4_2R.EnumerateRange(_left, _right))
            {
                //tCounter++;
                if (_minAcc <= block.Value.maxAccumulation)
                {
                    _Cover(block.Key.leftEnd, block.Key.rightEnd);
                    //pCounter++;
                }
            }

            //Console.WriteLine("Processed {0} / {1} = {2}% blocks", pCounter, tCounter, Math.Round((pCounter / (double)tCounter) * 100.0, 2));
            //Console.WriteLine("Processed {0} / {1} = {2}% bookmarks", count, _totalBookmarkCount, Math.Round((count / (double)_totalBookmarkCount) * 100.0, 2));
        }

        int count = 0;

        private void _Cover(C left, C right)
        {
            _markedAcc = -1;
            _accumulation = 0;
            _decomposer.Reset();
            //_decompositionStack.Reset();
            //_regionType = RegionType.Decomposition;

            IEnumerator<KeyValuePair<C, B>> enumerator = _di4_1R.EnumerateRange(left, right).GetEnumerator();

            

            while (enumerator.MoveNext())
            {
                count++;
                var bookmark = enumerator.Current;
                _accumulation = bookmark.Value.accumulation;

                if (_markedAcc == -1 &&
                    _accumulation >= _minAcc &&
                    _accumulation <= _maxAcc)
                {
                    _markedAcc = _accumulation;
                    _decomposer.Open(bookmark.Key, bookmark.Value);
                    continue;
                }
                else if (_markedAcc != -1 &&
                    (_accumulation < _minAcc ||
                    _accumulation > _maxAcc))
                {
                    _decomposer.Close(bookmark.Key, bookmark.Value);
                    _markedAcc = -1;
                    continue;
                }

                _decomposer.Update(bookmark.Value);
            }

            while (!_decomposer.CanConclude() && enumerator.MoveNext())
            {
               _decomposer.Update(enumerator.Current.Value);
            }

            /*foreach (var bookmark in _di4_1R.EnumerateRange(left, right))
            {
                _accumulation = bookmark.Value.lambda.Count - bookmark.Value.omega + bookmark.Value.mu;

                if (_markedAcc == -1 &&
                    _accumulation >= _minAcc &&
                    _accumulation <= _maxAcc)
                {
                    _markedAcc = _accumulation;
                    _regionType = RegionType.Designated;
                    //_decompositionStack.Open(bookmark.Key, bookmark.Value);
                    _decomposer.Open(bookmark.Key, bookmark.Value);
                    continue;
                }
                else if (_markedAcc != -1 &&
                    (_accumulation < _minAcc ||
                    _accumulation > _maxAcc))
                {
                    //_decompositionStack.Close(bookmark.Key, bookmark.Value);
                    _decomposer.Close(bookmark.Key, bookmark.Value);
                    _regionType = RegionType.Decomposition;
                    _markedAcc = -1;
                    continue;
                }

                //_decompositionStack.Update(bookmark.Value, _regionType);
                _decomposer.Update(bookmark.Value, _regionType);
            }*/

            //_decompositionStack.Conclude();
        }

        internal void Summit()
        {
            foreach (var block in _di4_2R.EnumerateRange(_left, _right))
                if (_minAcc <= block.Value.maxAccumulation)
                    _Summit(block.Key.leftEnd, block.Key.rightEnd);
        }
        private void _Summit(C left, C right)
        {
            _markedAcc = -1;
            _previousAcc = 0;
            _accumulation = 0;
            _decompositionStack.Reset();
            _regionType = RegionType.Decomposition;

            foreach (var bookmark in _di4_1R.EnumerateRange(left, right))
            {
                _accumulation = bookmark.Value.lambda.Count - bookmark.Value.omega + bookmark.Value.mu;

                if (_previousAcc < _accumulation &&
                    _markedAcc < _accumulation &&
                    _accumulation >= _minAcc &&
                    _accumulation <= _maxAcc)
                {
                    _markedAcc = _accumulation;
                    _regionType = RegionType.Designated;
                    _decompositionStack.Open(bookmark.Key, bookmark.Value);
                    _previousAcc = _accumulation;
                    continue;
                }
                else if (_markedAcc > _accumulation ||
                    (_markedAcc < _accumulation && (
                    _accumulation < _minAcc ||
                    _accumulation > _maxAcc) &&
                    _markedAcc != -1))
                {
                    _decompositionStack.Close(bookmark.Key, bookmark.Value);
                    _regionType = RegionType.Decomposition;
                    _markedAcc = -1;
                    _previousAcc = _accumulation;
                    continue;
                }

                _previousAcc = _accumulation;
                _decompositionStack.Update(bookmark.Value, _regionType);
            }

            _decompositionStack.Conclude();
        }
    }
}
