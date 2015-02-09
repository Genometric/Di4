using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGenomics;
using CSharpTest.Net.Collections;

namespace DI3
{
    /// <summary>
    /// Second resolution index.
    /// </summary>
    internal class SingleIndex2R<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal SingleIndex2R(BPlusTree<C, B> di31R,  BPlusTree<BlockKey<C>, BlockValue> di32R, C left, C right)
        {
            _di31R = di31R;
            _di32R = di32R;
            _left = left;
            _right = right;
        }

        private BPlusTree<C, B> _di31R { set; get; }
        private BPlusTree<BlockKey<C>, BlockValue> _di32R { set; get; }
        private C _left { set; get; }
        private C _right { set; get; }


        public void Index()
        {
            int maxAccumulation = 0;
            C currentBlockLeftEnd = _left;
            bool startNewBlock = true;
            Dictionary<uint, bool> presentIntervals = new Dictionary<uint, bool>();
            foreach (var bookmark in _di31R.EnumerateRange(_left, _right))
            {
                maxAccumulation = Math.Max(maxAccumulation, bookmark.Value.lambda.Count);
                foreach (var interval in bookmark.Value.lambda)
                    if (!presentIntervals.ContainsKey(interval.atI))
                        presentIntervals.Add(interval.atI, true);

                if (startNewBlock)
                {
                    currentBlockLeftEnd = bookmark.Key;
                    startNewBlock = false;
                    continue;
                }
                
                if (bookmark.Value.lambda.Count == bookmark.Value.omega)
                {
                    Update(currentBlockLeftEnd, bookmark.Key, maxAccumulation, presentIntervals.Count);
                    maxAccumulation = 0;
                    presentIntervals.Clear();
                    startNewBlock = true;
                }
            }
        }

        private void Update(C leftEnd, C rightEnd, int maxAccumulation, int count)
        {
            /// item is an element of di3 that intersects newKey.
            var newKey = new BlockKey<C>(leftEnd, rightEnd);
            var newValue = new BlockValue(maxAccumulation, count);
            var item = _di32R.EnumerateFrom(newKey).GetEnumerator().Current;

            if (!item.Equals(default(BlockKey<C>)))
            {
                /// The block that is already in di3 covers new interval,
                /// therefore no update is required.
                if (newKey.leftEnd.CompareTo(item.Key.leftEnd) == 1 &&  // newKey.LeftEnd > item.newKey.LeftEnd
                    newKey.rightEnd.CompareTo(item.Key.rightEnd) == -1) // newKey.rightEnd < item.newKey.rightEnd
                    return;

                if (newKey.leftEnd.CompareTo(item.Key.leftEnd) == 1) // newKey.leftEnd > item.newKey.leftEnd
                    newKey = newKey.Update(LeftEnd: item.Key.leftEnd);

                if (newKey.rightEnd.CompareTo(item.Key.rightEnd) == -1) // newKey.rightEnd < item.newKey.rightEnd
                    newKey = newKey.Update(RightEnd: item.Key.rightEnd);

                _di32R.Remove(item.Key);
            }
            _di32R.TryAdd(newKey, newValue);
        }

    }
}
