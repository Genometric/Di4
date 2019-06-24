using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


// TODO: 
// UPDATE THIS CLASS 
// LOTS OF FUNCTIONS ARE SHARING CODE


namespace Genometric.Di4
{
    internal class BatchIndex2R<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal BatchIndex2R(BPlusTree<C, B> di41R, BPlusTree<BlockKey<C>, BlockValue> di42R, C left, C right, CuttingMethod cuttingMethod, int levels, ConcurrentDictionary<C, int> addedBlocks)
        {
            _di41R = di41R;
            _di42R = di42R;
            _left = left;
            _right = right;
            _addedBlocks = addedBlocks;
            _cuttingMethod = cuttingMethod;
            _levels = levels;
            _bCounter = new BlockCounter();
        }

        private C _left { set; get; }
        private C _right { set; get; }
        private BPlusTree<C, B> _di41R { set; get; }
        private BPlusTree<BlockKey<C>, BlockValue> _di42R { set; get; }
        private ConcurrentDictionary<C, int> _addedBlocks { set; get; }
        private BlockCounter _bCounter { set; get; }
        private AccumulationStats<C, I, M> _accStats { set; get; }
        private CuttingMethod _cuttingMethod { set; get; }
        private int _levels { set; get; }

        private HashSet<uint> _tLambdas { set; get; }
        private uint[] _atI { set; get; }




        public void Run()
        {
            switch (_cuttingMethod)
            {
                case CuttingMethod.ZeroThresholding:
                    ZeroThresholding();
                    break;

                case CuttingMethod.UniformScalarQuantization:
                    //UniformQuantization();
                    Quantization(BoundariesType.Uniform);
                    break;

                case CuttingMethod.NonUniformScalarQuantization:
                    //NonUniformQuantization();
                    Quantization(BoundariesType.PDFOptimized);
                    break;
            }
        }

        private void ZeroThresholding()
        {
            int maxAccumulation = 0;
            int intervalsCount = 0;
            C currentBlockLeftEnd = _left;
            bool startNewBlock = true;
            _atI = new uint[0];
            foreach (var bookmark in _di41R.EnumerateRange(_left, _right))
            {
                maxAccumulation = Math.Max(maxAccumulation, bookmark.Value.mu + bookmark.Value.lambda.Count - bookmark.Value.omega);
                intervalsCount += bookmark.Value.lambda.Count - bookmark.Value.omega;

                if (startNewBlock)
                {
                    currentBlockLeftEnd = bookmark.Key;
                    startNewBlock = false;
                    continue;
                }

                if (bookmark.Value.lambda.Count == bookmark.Value.omega && bookmark.Value.mu == 0)
                {
                    Update(leftEnd: currentBlockLeftEnd, rightEnd: bookmark.Key, minAccumulation: 0, maxAccumulation: maxAccumulation, count: intervalsCount);
                    maxAccumulation = 0;
                    intervalsCount = 0;
                    startNewBlock = true;
                }
            }

            _addedBlocks.TryAdd(_left, _bCounter.value);
        }
        private void UniformQuantization()
        {
            _tLambdas = new HashSet<uint>();
            //KeyValuePair<C, B> firstItem;
            //_di41R.TryGetFirst(out firstItem);

            var boundaries = Boundaries(BoundariesType.Uniform);
            var partitions = new Dictionary<int, int>();
            for (int i = 0; i < boundaries.Length - 1; i++)
                for (int j = (int)Math.Round(boundaries[i]); j < Math.Round(boundaries[i + 1]); j++)
                    partitions.Add(j, i);
            partitions.Add((int)boundaries[boundaries.Length - 1], boundaries.Length - 2);

            /*KeyValuePair<C, B> lastItem;
            _di41R.TryGetLast(out lastItem);

            var accDis = new SortedDictionary<int, int>();

            object lockOnMe = new object();
            _accStats = new AccumulationStats<C, I, M>(_di41R, firstItem.Key, lastItem.Key, accDis, lockOnMe);
            _accStats.AccDistribution();

            int binLenght = (int)Math.Ceiling(accDis.Last().Key / (double)_levels);*/

            int currentBin = 0, previousBin;
            int currentAccumulation;

            int maxAccumulation = 0;
            int intervalsCount = 0;
            C currentBlockLeftEnd = _left;
            bool startNewBlock = true;


            foreach (var snapshot in _di41R.EnumerateRange(_left, _right))
            {
                foreach (var lambda in snapshot.Value.lambda)
                    if (!_tLambdas.Remove(lambda.atI))
                        _tLambdas.Add(lambda.atI);

                currentAccumulation = snapshot.Value.accumulation;
                maxAccumulation = Math.Max(maxAccumulation, currentAccumulation);

                previousBin = currentBin;
                // Removed for test, it must be included: currentBin = (int)Math.Floor(currentAccumulation / (double)binLenght);

                if (startNewBlock)
                {
                    currentBlockLeftEnd = snapshot.Key;
                    startNewBlock = false;
                    intervalsCount = _tLambdas.Count;
                    _atI = new uint[_tLambdas.Count];
                    _tLambdas.CopyTo(_atI);
                    continue;
                }

                intervalsCount += snapshot.Value.lambda.Count - snapshot.Value.omega;

                if (currentBin != previousBin)
                {
                    //Update(currentBlockLeftEnd, snapshot.Key, maxAccumulation, intervalsCount);
                    maxAccumulation = 0;
                    startNewBlock = true;
                }
            }

            //Update(currentBlockLeftEnd, _right, maxAccumulation, intervalsCount);

            _addedBlocks.TryAdd(_left, _bCounter.value);
        }
        private void NonUniformQuantization()
        {
            // PDF-optimized Lloyd-Max algorithm.
            _tLambdas = new HashSet<uint>();
            //KeyValuePair<C, B> firstItem;
            //_di41R.TryGetFirst(out firstItem);

            var boundaries = Boundaries(BoundariesType.PDFOptimized);
            var partitions = new Dictionary<int, int>();
            for (int i = 0; i < boundaries.Length - 1; i++)
                for (int j = (int)Math.Round(boundaries[i]); j < Math.Round(boundaries[i + 1]); j++)
                    partitions.Add(j, i);
            partitions.Add((int)boundaries[boundaries.Length - 1], boundaries.Length - 2);


            /*KeyValuePair<C, B> lastItem;
            _di41R.TryGetLast(out lastItem);

            var PDF = new SortedDictionary<int, double>();

            object lockOnMe = new object();
            _accStats = new AccumulationStats<C, I, M>(_di41R, firstItem.Key, lastItem.Key, PDF, lockOnMe);
            _accStats.PDF();*/

            /*int maxValue = PDF.Last().Key;
            if (accToCut == -1)
                accToCut = maxValue + (int)Math.Ceiling(maxValue * 0.25);*/


            /*double tSum = 0;
            int tBin = 0;
            Dictionary<int, int> partitions = new Dictionary<int, int>();
            for (int i = 0; i < PDF.Count; i++)
            {
                if (tSum + PDF[i] <= 1)//accToCut)
                {
                    partitions.Add(i, tBin);
                    tSum += PDF[i];
                }
                else
                {
                    partitions.Add(i, ++tBin);
                    tSum = PDF[i];
                }
            }*/

            int currentPartition = 0, previousPartition;
            int currentAccumulation;

            int maxAccumulation = 0;
            int intervalsCount = 0;
            C currentBlockLeftEnd = _left;
            bool startNewBlock = true;


            foreach (var snapshot in _di41R.EnumerateRange(_left, _right))
            {
                foreach (var lambda in snapshot.Value.lambda)
                    if (!_tLambdas.Remove(lambda.atI))
                        _tLambdas.Add(lambda.atI);

                currentAccumulation = snapshot.Value.mu + snapshot.Value.lambda.Count - snapshot.Value.omega;
                maxAccumulation = Math.Max(maxAccumulation, currentAccumulation);

                previousPartition = currentPartition;
                currentPartition = partitions[currentAccumulation];

                if (startNewBlock)
                {
                    currentBlockLeftEnd = snapshot.Key;
                    startNewBlock = false;
                    intervalsCount = _tLambdas.Count;
                    _atI = new uint[_tLambdas.Count];
                    _tLambdas.CopyTo(_atI);
                    continue;
                }

                intervalsCount += snapshot.Value.lambda.Count - snapshot.Value.omega;

                if (currentPartition != previousPartition)
                {
                    //Update(currentBlockLeftEnd, snapshot.Key, maxAccumulation, intervalsCount);
                    maxAccumulation = 0;
                    startNewBlock = true;
                }
            }

            //Update(currentBlockLeftEnd, _right, maxAccumulation, intervalsCount);

            _addedBlocks.TryAdd(_left, _bCounter.value);
        }
        private void Quantization(BoundariesType type)
        {
            var boundaries = Boundaries(type);
            var partitions = new Dictionary<int, int>();
            for (int i = 0; i < boundaries.Length - 1; i++)
                for (int j = (int)Math.Round(boundaries[i]); j < Math.Round(boundaries[i + 1]); j++)
                    partitions.Add(j, i);
            partitions.Add((int)boundaries[boundaries.Length - 1], boundaries.Length - 2);

            int currentPartition = 0;
            int previousPartition = 0;
            //int currentAccumulation = 0;
            int minAccumulation = int.MaxValue;
            int maxAccumulation = 0;
            int intervalsCount = 0;
            //bool startNewBlock = true;
            C currentBlockLeftEnd = _left;
            _tLambdas = new HashSet<uint>();
            C currentBlockRightEnd = default(C);

            var enumerator = _di41R.EnumerateRange(_left, _right).GetEnumerator();

            if (!enumerator.MoveNext())
                return; // i.e., no snapshot is available in given range to create second resolution. 

            currentBlockLeftEnd = enumerator.Current.Key;
            currentBlockRightEnd = enumerator.Current.Key;
            foreach (var lambda in enumerator.Current.Value.lambda)
                if (!_tLambdas.Remove(lambda.atI))
                    _tLambdas.Add(lambda.atI);

            intervalsCount = _tLambdas.Count;
            _atI = new uint[_tLambdas.Count];
            _tLambdas.CopyTo(_atI);

            maxAccumulation = enumerator.Current.Value.accumulation;
            minAccumulation = enumerator.Current.Value.accumulation;

            previousPartition = partitions[enumerator.Current.Value.accumulation];

            while (enumerator.MoveNext())
            {
                foreach (var lambda in enumerator.Current.Value.lambda)
                    if (!_tLambdas.Remove(lambda.atI))
                        _tLambdas.Add(lambda.atI);

                currentPartition = partitions[enumerator.Current.Value.accumulation];
                if (currentPartition != previousPartition)
                {
                    Update(leftEnd: currentBlockLeftEnd, rightEnd: currentBlockRightEnd /*snapshot.Key*/, minAccumulation: minAccumulation, maxAccumulation: maxAccumulation, count: intervalsCount);
                    maxAccumulation = 0;
                    minAccumulation = int.MaxValue;
                    intervalsCount = _tLambdas.Count;
                    currentBlockLeftEnd = enumerator.Current.Key;
                    _atI = new uint[_tLambdas.Count];
                    _tLambdas.CopyTo(_atI);
                }
                else
                {
                    intervalsCount += enumerator.Current.Value.lambda.Count - enumerator.Current.Value.omega;
                }

                maxAccumulation = Math.Max(maxAccumulation, enumerator.Current.Value.accumulation);
                minAccumulation = Math.Min(minAccumulation, enumerator.Current.Value.accumulation);
                previousPartition = currentPartition;
                currentBlockRightEnd = enumerator.Current.Key;
            }

            /*foreach (var snapshot in _di41R.EnumerateRange(_left, _right))
            {
                foreach (var lambda in snapshot.Value.lambda)
                    if (!_tLambdas.Remove(lambda.atI))
                        _tLambdas.Add(lambda.atI);

                currentAccumulation = snapshot.Value.accumulation;
                currentPartition = partitions[currentAccumulation];

                if (startNewBlock)
                {
                    currentBlockLeftEnd = snapshot.Key;
                    startNewBlock = false;
                    intervalsCount = _tLambdas.Count;
                    _atI = new uint[_tLambdas.Count];
                    _tLambdas.CopyTo(_atI);
                }
                else
                {
                    if (currentPartition != previousPartition)
                    {
                        Update(leftEnd: currentBlockLeftEnd, rightEnd: currentBlockRightEnd snapshot.Key, minAccumulation: minAccumulation, maxAccumulation: maxAccumulation, count: intervalsCount);
                        maxAccumulation = 0;
                        minAccumulation = int.MaxValue;
                        startNewBlock = true;
                    }

                    intervalsCount += snapshot.Value.lambda.Count - snapshot.Value.omega;
                }
                
                maxAccumulation = Math.Max(maxAccumulation, currentAccumulation);
                minAccumulation = Math.Min(minAccumulation, currentAccumulation);
                previousPartition = currentPartition;
                currentBlockRightEnd = snapshot.Key;
            }*/


            // make sure I need this:
            Update(leftEnd: currentBlockLeftEnd, rightEnd: _right, minAccumulation: minAccumulation, maxAccumulation: maxAccumulation, count: intervalsCount);

            _addedBlocks.TryAdd(_left, _bCounter.value);
        }

        private double[] Boundaries(BoundariesType type)
        {
            var boundaries = new double[_levels + 1];
            var reconstructionPoints = new double[_levels];

            KeyValuePair<C, B> firstItem;
            _di41R.TryGetFirst(out firstItem);

            KeyValuePair<C, B> lastItem;
            _di41R.TryGetLast(out lastItem);

            var accDis = new SortedDictionary<int, int>();
            var lockOnMe = new object();
            _accStats = new AccumulationStats<C, I, M>(_di41R, firstItem.Key, lastItem.Key, accDis, lockOnMe);
            _accStats.AccDistribution();

            // TODO: this could be empty: no last item
            double r = accDis.Last().Key;
            double delta = r / _levels;

            boundaries[0] = 0;
            boundaries[boundaries.Length - 1] = accDis.Last().Key;
            // in other words: calculates decision threshold
            for (int i = 1; i < boundaries.Length - 1; i++)
                boundaries[i] = boundaries[i - 1] + delta;

            if (type == BoundariesType.Uniform)
                return boundaries;

            bool distortionReductionRequired = true;
            while (distortionReductionRequired)
            {
                // in other words: calculates new representitive levels
                double sum = 0;
                int count = 0;
                for (int i = 0; i < reconstructionPoints.Length; i++)
                {
                    sum = 0;
                    count = 0;

                    foreach (var item in accDis)
                        if (boundaries[i] <= item.Key && item.Key < boundaries[i + 1])
                        {
                            sum += item.Key * item.Value;
                            count += item.Value;
                        }

                    if (count == 0)
                        reconstructionPoints[i] = (boundaries[i + 1] + boundaries[i]) / 2;
                    else
                        reconstructionPoints[i] = sum / count;
                }
                reconstructionPoints[reconstructionPoints.Length - 1] = (sum + accDis.Keys.Last()) / (count + 1); // this should be: (count + laskey.value) !!

                double t;
                distortionReductionRequired = false;
                for (int i = 1; i < boundaries.Length - 1; i++)
                {
                    t = (reconstructionPoints[i - 1] + reconstructionPoints[i]) / 2;

                    if (t != boundaries[i])
                        distortionReductionRequired = true;

                    boundaries[i] = t;
                }
            }

            return boundaries;
        }

        private enum BoundariesType { Uniform, PDFOptimized };


        private void Update(C leftEnd, C rightEnd, int minAccumulation, int maxAccumulation, int count)
        {
            /// lambda is an element of di4_1R that intersects newKey.
            var newKey = new BlockKey<C>(leftEnd, rightEnd);
            var newValue = new BlockValue(BoundariesLowerBound: minAccumulation, BoundariesUpperBound: maxAccumulation, IntervalCount: count, atI: _atI);

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
