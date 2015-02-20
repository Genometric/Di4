using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polimi.DEIB.VahidJalili.IGenomics;
using CSharpTest.Net.Collections;

namespace Polimi.DEIB.VahidJalili.DI3
{
    /// <summary>
    /// Second resolution index.
    /// </summary>
    internal class SingleIndex2R<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal SingleIndex2R(BPlusTree<C, B> di31R, BPlusTree<BlockKey<C>, BlockValue> di32R, C left, C right)
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
                //foreach (var interval in bookmark.Value.lambda)
                    //if (!presentIntervals.ContainsKey(interval.atI))
                        //presentIntervals.Add(interval.atI, true);

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
            /// lambda is an element of di3_1R that intersects newKey.
            var newKey = new BlockKey<C>(leftEnd, rightEnd);
            var newValue = new BlockValue(maxAccumulation, count);

            foreach (var item in _di32R.EnumerateFrom(newKey))
            {
                /// The same key already exist.
                if (newKey.leftEnd.CompareTo(item.Key.leftEnd) == 0 &&
                    newKey.rightEnd.CompareTo(item.Key.rightEnd) == 0)
                    return;

                /// "lambda" occures after "newKey" and does not intersect with it.
                if (newKey.rightEnd.CompareTo(item.Key.leftEnd) == -1) // newKey.rightEnd < lambda.key.leftEnd
                    break;

                /// The bookmark that is already in di3_1R covers new interval,
                /// therefore no update is required.
                if (newKey.leftEnd.CompareTo(item.Key.leftEnd) == 1 &&  // newKey.LeftEnd > lambda.newKey.LeftEnd
                    newKey.rightEnd.CompareTo(item.Key.rightEnd) == -1) // newKey.rightEnd < lambda.newKey.rightEnd
                    return;

                /// Theoretically, these conditions may not be needed ever !!
                //if (newKey.leftEnd.CompareTo(lambda.Key.leftEnd) == 1) // newKey.leftEnd > lambda.newKey.leftEnd
                    //newKey = newKey.UpdateLeft(LeftEnd: lambda.Key.leftEnd);
                //if (newKey.rightEnd.CompareTo(lambda.Key.rightEnd) == -1) // newKey.rightEnd < lambda.newKey.rightEnd
                    //newKey = newKey.UpdateRight(RightEnd: lambda.Key.rightEnd);

                _di32R.Remove(item.Key);


                /// yeah, true ;-) process only one lambda. 
                /// maybe there would be a better way to do this. 
                /// possibly using: _di3_2R.EnumerateFrom(newKey).GetEnumerator().Current
                /// we can do this iteration. But GetEnumerator throws an exception when tree
                /// is empty, althougth that can be handled by a try-catch-finally but I guess
                /// this method is more clean ;-)
                break;
            }
            _di32R.TryAdd(newKey, newValue);
        }
    }
}
