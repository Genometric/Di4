using CSharpTest.Net.Collections;
using Genometric.Di4.AuxiliaryComponents;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;

namespace Genometric.Di4
{
    internal class CoverSummit_mid<C, I, M, O>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal CoverSummit_mid(
            object lockOnMe,
            BPlusTree<C, B> di4_1R,
            BPlusTree<BlockKey<C>, BlockValue> di4_2R,
            IOutput<C, I, M, O> outputStrategy,
            BlockKey<C> left,
            BlockKey<C> right,
            int minAcc,
            int maxAcc)
        {
            _di4_1R = di4_1R;
            _di4_2R = di4_2R;
            _left = left;
            _right = right;
            _minAcc = minAcc;
            _maxAcc = maxAcc;
            _regionType = RegionType.Candidate;
            _decompositionStack = new DecompositionStack<C, I, M, O>(outputStrategy, lockOnMe);
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
        private DecompositionStack<C,I,M,O> _decompositionStack { set; get; }
        private RegionType _regionType { set; get; }


        internal void Cover()
        {
            foreach (var block in _di4_2R.EnumerateRange(_left, _right))
                if (_minAcc <= block.Value.boundariesUpperBound)
                    _Cover(block.Key.leftEnd, block.Key.rightEnd);
        }
        private void _Cover(C left, C right)
        {
            _markedAcc = -1;
            _accumulation = 0;
            _decompositionStack.Reset();
            _regionType = RegionType.Decomposition;

            foreach (var bookmark in _di4_1R.EnumerateRange(left, right))
            {
                _accumulation = bookmark.Value.lambda.Count - bookmark.Value.omega + bookmark.Value.mu;

                if (_markedAcc == -1 &&
                    _accumulation >= _minAcc &&
                    _accumulation <= _maxAcc)
                {
                    _markedAcc = _accumulation;
                    _regionType = RegionType.Designated;
                    _decompositionStack.Open(bookmark.Key, bookmark.Value);
                    continue;
                }
                else if (_markedAcc != -1 &&
                    (_accumulation < _minAcc ||
                    _accumulation > _maxAcc))
                {
                    _decompositionStack.Close(bookmark.Key, bookmark.Value);
                    _regionType = RegionType.Decomposition;
                    _markedAcc = -1;
                    continue;
                }

                _decompositionStack.Update(bookmark.Value, _regionType);
            }

            _decompositionStack.Conclude();
        }

        internal void Summit()
        {
            foreach (var block in _di4_2R.EnumerateRange(_left, _right))
                if (_minAcc <= block.Value.boundariesUpperBound)
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
