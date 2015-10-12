using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Concurrent;

namespace Polimi.DEIB.VahidJalili.DI4.Inv
{
    internal class BatchIndex2R<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal BatchIndex2R(BPlusTree<C, B> di41R, BPlusTree<BlockKey<C>, BlockValue> di42R, C left, C right, ConcurrentDictionary<C, int> addedBlocks)
        {
            _di41R = di41R;
            _di42R = di42R;
            _left = left;
            _right = right;
            _addedBlocks = addedBlocks;
            _bCounter = new BlockCounter();
        }

        private C _left { set; get; }
        private C _right { set; get; }
        private BPlusTree<C, B> _di41R { set; get; }
        private BPlusTree<BlockKey<C>, BlockValue> _di42R { set; get; }
        private ConcurrentDictionary<C, int> _addedBlocks { set; get; }
        private BlockCounter _bCounter { set; get; }


        public void Run()
        {
            int maxAccumulation = 0;
            int intervalsCount = 0;
            C currentBlockLeftEnd = _left;
            bool startNewBlock = true;
            foreach (var bookmark in _di41R.EnumerateRange(_left, _right))
            {
                maxAccumulation = Math.Max(maxAccumulation, bookmark.Value.lambda.Count);

                /// Important Note:
                /// The number of contributing intervals is determined based on 
                /// their right-end. However, the pre-requisite for this method
                /// is that the partition determined by _left and _right ends
                /// covers both ends of intervals (in other words, there is no
                /// interval that spans two partitions). This method is required
                /// to be updated upon modification of partitioning concept.
                intervalsCount += bookmark.Value.omega;

                if (startNewBlock)
                {
                    currentBlockLeftEnd = bookmark.Key;
                    startNewBlock = false;
                    continue;
                }

                if (bookmark.Value.lambda.Count == bookmark.Value.omega) //&& bookmark.Value.mu == 0)
                {
                    Update(currentBlockLeftEnd, bookmark.Key, maxAccumulation, intervalsCount);
                    maxAccumulation = 0;
                    intervalsCount = 0;
                    startNewBlock = true;
                }
            }

            _addedBlocks.TryAdd(_left, _bCounter.value);
        }

        private void Update(C leftEnd, C rightEnd, int maxAccumulation, int count)
        {
            /// lambda is an element of di4_1R that intersects newKey.
            var newKey = new BlockKey<C>(leftEnd, rightEnd);
            var newValue = new BlockValue(maxAccumulation, count);

            foreach (var item in _di42R.EnumerateFrom(newKey))
            {
                /// The same key already exist.
                if (newKey.leftEnd.CompareTo(item.Key.leftEnd) == 0 &&
                    newKey.rightEnd.CompareTo(item.Key.rightEnd) == 0)
                    return;

                /// "lambda" occures after "newKey" and does not intersect with it.
                if (newKey.rightEnd.CompareTo(item.Key.leftEnd) == -1) // newKey.rightEnd < lambda.key.start
                    break;

                /// The keyBookmark that is already in di4_1R covers new interval,
                /// therefore no update is required.
                if (newKey.leftEnd.CompareTo(item.Key.leftEnd) == 1 &&  // newKey.LeftEnd > lambda.newKey.LeftEnd
                    newKey.rightEnd.CompareTo(item.Key.rightEnd) == -1) // newKey.rightEnd < lambda.newKey.rightEnd
                    return;

                /// Theoretically, these conditions may not be needed ever !!
                //if (newKey.start.CompareTo(lambda.Key.start) == 1) // newKey.start > lambda.newKey.start
                //newKey = newKey.UpdateLeft(LeftEnd: lambda.Key.start);
                //if (newKey.rightEnd.CompareTo(lambda.Key.rightEnd) == -1) // newKey.rightEnd < lambda.newKey.rightEnd
                //newKey = newKey.UpdateRight(RightEnd: lambda.Key.rightEnd);

                _bCounter.value--;
                _di42R.Remove(item.Key);


                /// yeah, true ;-) process only one lambda. 
                /// maybe there would be a better way to do this. 
                /// possibly using: _di4_2R.EnumerateFrom(newKey).GetEnumerator().Current
                /// we can do this iteration. But GetEnumerator throws an exception when tree
                /// is empty, althougth that can be handled by a try-catch-finally but I guess
                /// this method is more clean ;-)
                break;
            }
            _bCounter.value++;
            _di42R.TryAdd(newKey, newValue);
        }

        private class BlockCounter
        {
            public int value { set; get; }
        }
    }
}