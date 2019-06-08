using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;

namespace Genometric.Di4.Inc
{
    internal class MergeComplement<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal MergeComplement(BPlusTree<BlockKey<C>, BlockValue> di4_2R, BlockKey<C> left, BlockKey<C> right, SortedDictionary<BlockKey<C>,int> blocks, object lockOnMe)
        {
            _di4_2R = di4_2R;
            _left = left;
            _right = right;
            _lockOnMe = lockOnMe;
            _blocks = blocks;
        }

        private BPlusTree<BlockKey<C>, BlockValue> _di4_2R { set; get; }
        private SortedDictionary<BlockKey<C>,int> _blocks { set; get; }
        private BlockKey<C> _left { set; get; }
        private BlockKey<C> _right { set; get; }
        private object _lockOnMe { set; get; }

        internal void Merge()
        {
            /*var tmpDic = new SortedDictionary<BlockKey<C>, int>();
            foreach (var block in _di4_2R.EnumerateRange(_left, _right))
                tmpDic.Add(block.Key, block.Value.intervalCount);
            
            lock (_lockOnMe)
            {
                foreach (var refLambdas in tmpDic)
                    _blocks.Add(refLambdas.Key, refLambdas.Value);
            }*/
        }
        internal void Complement()
        {
            bool doBreak = false;
            var tmpDic = new SortedDictionary<BlockKey<C>, int>();
            C tmpBookmark = default(C);

            /// As usual, this little iteration is for initialization.
            foreach (var block in _di4_2R.EnumerateRange(_left, _right))
            {
                if (doBreak)
                {
                    _left = block.Key;
                    break;
                }
                tmpBookmark = block.Key.rightEnd;
                doBreak = true;
            }
            foreach (var block in _di4_2R.EnumerateRange(_left, _right))
            {
                tmpDic.Add(new BlockKey<C>(tmpBookmark, block.Key.leftEnd), 0);
                tmpBookmark = block.Key.rightEnd;
            }

            lock (_lockOnMe)
            {
                foreach (var item in tmpDic)
                    _blocks.Add(item.Key, 0);
            }
        }
    }
}
