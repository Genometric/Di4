using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using CSharpTest.Net.Synchronization;
using CSharpTest.Net.Threading;
using Genometric.Di4.BasicOperations.IndexFunctions;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Di4 uses the hashkey of intervals as ID; hence two intervals with same ID should exist. 
/// However, Di4 does not control this property, and trusts the input. Di4 does throw an 
/// exception for duplicate IDs. However, the functions may crash is duplications exist.
/// </summary>



namespace Genometric.Di4
{
    /// <summary>
    /// Dynamic intervals inverted index (Genometric.Di4) 
    /// is an indexing system aimed at providing
    /// efficient means of processing the intervals
    /// it indexes for common information retrieval 
    /// tasks.
    /// </summary>
    /// <typeparam name="C">Represents the c/domain
    /// type (e.g,. int, double, Time).</typeparam>
    /// <typeparam name="I">Represents generic type of the interval.
    /// (e.g., time span, interval on natural numbers)
    /// <para>For intervals of possibly different types,
    /// it is recommended to define this generic type
    /// parameter in terms of Lowest Common Denominator.
    /// </para></typeparam>
    /// <typeparam name="M">Represents generic
    /// type of pointer to descriptive atI cooresponding
    /// to the _interval.</typeparam>
    public class Di4<C, I, M> : IDisposable
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        /// <summary>
        /// Dynamic intervals incremental inverted index (Genometric.Di4) 
        /// is an indexing system aimed at providing
        /// efficient means of processing the intervals
        /// it indexes for common information retrieval 
        /// tasks.
        /// </summary>
        /// <param name="CSerializer"></param>
        /// <param name="comparer"></param>
        private Di4(
            string FileName,
            CreatePolicy createPolicy,
            ISerializer<C> CSerializer,
            IComparer<C> comparer,
            int avgKeySize,
            int avgValueSize,
            bool THIS_IS_THE_MOST_USED_ONE)
        {
            //bSerializer = new BSerializer();
            //var options = new BPlusTree<C, B>.OptionsV2(CSerializer, bSerializer, comparer);


            ////counted.CalcBTreeOrder(avgKeySize, avgValueSize); //24);
            //options.CreateFile = createPolicy;
            ////counted.ExistingLogAction = ExistingLogAction.ReplayAndCommit;
            //options.StoragePerformance = StoragePerformance.Fastest;


            //////// Multi-Threading 2nd pass test; was commented-out visa versa
            //counted.CachePolicy = CachePolicy.All;
            //options.CachePolicy = CachePolicy.Recent;


            //counted.FileBlockSize = 512;

            /*counted.MaximumChildNodes = 8;
            counted.MinimumChildNodes = 2;

            counted.MaximumValueNodes = 8;
            counted.MinimumValueNodes = 2;
            */


            /// There three lines added for multi-threading.
            //counted.CallLevelLock = new ReaderWriterLocking();
            /////counted.LockingFactory = new LockFactory<SimpleReadWriteLocking>(); //Test 1
            /////counted.LockingFactory = new LockFactory<WriterOnlyLocking>(); //Test 2
            //counted.LockingFactory = new LockFactory<ReaderWriterLocking>();
            //counted.LockTimeout = 10000;




            //options.MaximumChildNodes = 256;// 100;
            //options.MinimumChildNodes = 2;//2;//10;

            //options.MaximumValueNodes = 256; // 100;
            //options.MinimumValueNodes = 2;//2;//10;

            //if (createPolicy != CreatePolicy.Never)
                //options.FileName = FileName;

            //_di4_1R = new BPlusTree<C, B>(options);
            //SingleIndex = new SingleIndex<C, I, M>(_di4_1R);

            //_di4_1R.DebugSetValidateOnCheckpoint(false);
        }


        public Di4(Di4Options<C> options)
        {
            _options = options;
            _di4_incIdx = new BPlusTree<C, B>(GetIncOptions());
            _di4_2R = new BPlusTree<BlockKey<C>, BlockValue>(Get2ROptions());
            _di4_info = new BPlusTree<string, int>(GetinfoOptions());
            _indexesCardinality = new InfoIndex(_di4_info);

            incBatchIndex = new BatchIndex<C, I, M>(_di4_incIdx);            

            /// Don't enable following commands.
            /// Because, the initialization is linear with data size.
            //_di4_1R.EnableCount();
            //_di4_2R.EnableCount();
        }


        private BPlusTree<C, B> _di4_incIdx { set; get; }
        private BPlusTree<BlockKey<C>, BlockValue> _di4_2R { set; get; }
        private BPlusTree<string, int> _di4_info { set; get; }
        private BSerializer _incBSerializer { set; get; }
        private LambdaItemSerializer _lambdaItemSerializer { set; get; }
        private LambdaArraySerializer _lambdaArraySerializer { set; get; }
        private BlockKeySerializer<C> _blockKeySerializer { set; get; }
        private BlockValueSerializer _blockValueSerializer { set; get; }
        private InfoIndex _indexesCardinality { set; get; }
        private Di4Options<C> _options { set; get; }


        private string _keyCardinalityIncIndx { get { return "INCINDX"; } }
        private string _keyCardinalityInvIndx { get { return "INVINDX"; } }
        private string _keyCardinality2R { get { return "2ndResolution"; } }        


        private BatchIndex<C, I, M> incBatchIndex { set; get; }


        public int bookmarkCount
        {
            get
            {
                int bookmarks = _indexesCardinality.GetValue(_keyCardinalityIncIndx);
                return bookmarks == 0 ? _indexesCardinality.GetValue(_keyCardinalityInvIndx) : bookmarks;
            }
        }
        public int blockCount { get { return _indexesCardinality.GetValue(_keyCardinality2R); } }

        private BPlusTree<C, B>.OptionsV2 GetIncOptions()
        {
            _lambdaItemSerializer = new LambdaItemSerializer();
            _lambdaArraySerializer = new LambdaArraySerializer(_lambdaItemSerializer);
            _incBSerializer = new BSerializer(_lambdaArraySerializer);
            var rtv = new BPlusTree<C, B>.OptionsV2(_options.CSerializer, _incBSerializer, _options.Comparer);
            rtv.ReadOnly = _options.OpenReadOnly;

            if (_options.MaximumChildNodes >= 4 &&
                _options.MinimumChildNodes >= 2 &&
                _options.MaximumValueNodes >= 4 &&
                _options.MinimumValueNodes >= 2)
            {
                rtv.MaximumChildNodes = _options.MaximumChildNodes;
                rtv.MinimumChildNodes = _options.MinimumChildNodes;
                rtv.MaximumValueNodes = _options.MaximumValueNodes;
                rtv.MinimumValueNodes = _options.MinimumValueNodes;
            }

            if (_options.AverageKeySize != 0 && _options.AverageValueSize != 0)
                rtv.CalcBTreeOrder(_options.AverageKeySize, _options.AverageValueSize);

            if (_options.FileBlockSize != 0)
                rtv.FileBlockSize = _options.FileBlockSize;

            rtv.CachePolicy = _options.cacheOptions.CachePolicy;
            if (_options.CreatePolicy != CreatePolicy.Never)
                rtv.FileName = _options.FileName + ".iidx";

            rtv.CreateFile = _options.CreatePolicy;
            rtv.ExistingLogAction = _options.ExistingLogAction;
            rtv.StoragePerformance = _options.StoragePerformance;

            rtv.CallLevelLock = new ReaderWriterLocking();
            if (_options.LockTimeout > 0) rtv.LockTimeout = _options.LockTimeout;

            switch (_options.Locking)
            {
                case LockMode.WriterOnlyLocking:
                    rtv.LockingFactory = new LockFactory<WriterOnlyLocking>();
                    break;

                case LockMode.ReaderWriterLocking:
                    rtv.LockingFactory = new LockFactory<ReaderWriterLocking>();
                    break;

                case LockMode.SimpleReadWriteLocking:
                    rtv.LockingFactory = new LockFactory<SimpleReadWriteLocking>();
                    break;

                case LockMode.IgnoreLocking:
                    rtv.LockingFactory = new IgnoreLockFactory();
                    break;
            }

            if (_options.cacheOptions.CacheMaximumHistory != 0 && _options.cacheOptions.CacheKeepAliveTimeOut != 0)
            {
                rtv.CacheKeepAliveMaximumHistory = _options.cacheOptions.CacheMaximumHistory;
                rtv.CacheKeepAliveMinimumHistory = _options.cacheOptions.CacheMinimumHistory;
                rtv.CacheKeepAliveTimeout = _options.cacheOptions.CacheKeepAliveTimeOut;
            }

            return rtv;
        }
        private BPlusTree<BlockKey<C>, BlockValue>.OptionsV2 Get2ROptions()
        {
            /// TODO
            /// Try to optimize these settings as much as possible.
            
            _blockKeySerializer = new BlockKeySerializer<C>(_options.CSerializer);
            _blockValueSerializer = new BlockValueSerializer();

            var rtv = new BPlusTree<BlockKey<C>, BlockValue>.OptionsV2(_blockKeySerializer, _blockValueSerializer, new BlockKeyComparer<C>());
            rtv.ReadOnly = _options.OpenReadOnly;

            rtv.MinimumChildNodes = 2;
            rtv.MaximumChildNodes = 256;
            rtv.MinimumValueNodes = 2;
            rtv.MaximumValueNodes = 256;

            rtv.FileBlockSize = 8192;

            rtv.StoragePerformance = StoragePerformance.Fastest;

            rtv.CachePolicy = _options.cacheOptions.CachePolicy;
            if (_options.CreatePolicy != CreatePolicy.Never)
                rtv.FileName = _options.FileName + ".idx2R";

            rtv.CreateFile = _options.CreatePolicy;
            rtv.ExistingLogAction = _options.ExistingLogAction;
            rtv.StoragePerformance = _options.StoragePerformance;

            rtv.CallLevelLock = new ReaderWriterLocking();
            if (_options.LockTimeout > 0) rtv.LockTimeout = _options.LockTimeout;

            switch (_options.Locking)
            {
                case LockMode.WriterOnlyLocking:
                    rtv.LockingFactory = new LockFactory<WriterOnlyLocking>();
                    break;

                case LockMode.ReaderWriterLocking:
                    rtv.LockingFactory = new LockFactory<ReaderWriterLocking>();
                    break;

                case LockMode.SimpleReadWriteLocking:
                    rtv.LockingFactory = new LockFactory<SimpleReadWriteLocking>();
                    break;

                case LockMode.IgnoreLocking:
                    rtv.LockingFactory = new IgnoreLockFactory();
                    break;
            }

            if (_options.cacheOptions.CacheMaximumHistory != 0 && _options.cacheOptions.CacheKeepAliveTimeOut != 0)
            {
                rtv.CacheKeepAliveMaximumHistory = _options.cacheOptions.CacheMaximumHistory;
                rtv.CacheKeepAliveMinimumHistory = _options.cacheOptions.CacheMinimumHistory;
                rtv.CacheKeepAliveTimeout = _options.cacheOptions.CacheKeepAliveTimeOut;
            }

            return rtv;
        }
        private BPlusTree<string, int>.OptionsV2 GetinfoOptions()
        {
            var rtv = new BPlusTree<string, int>.OptionsV2(PrimitiveSerializer.String, PrimitiveSerializer.Int32, new Comparers.StringComparer());
            rtv.FileBlockSize = 1024;
            rtv.StoragePerformance = StoragePerformance.Fastest;
            rtv.CachePolicy = _options.cacheOptions.CachePolicy;
            if (_options.CreatePolicy != CreatePolicy.Never)
                rtv.FileName = _options.FileName + ".info";

            rtv.CreateFile = _options.CreatePolicy;
            rtv.ExistingLogAction = _options.ExistingLogAction;
            rtv.StoragePerformance = _options.StoragePerformance;

            rtv.CacheKeepAliveMinimumHistory = 512;
            rtv.CacheKeepAliveMaximumHistory = 4096;
            
            return rtv;
        }



        public void Add(List<I> intervals, IndexingMode mode, uint collectionID)
        {
            Add(intervals, mode, collectionID, Environment.ProcessorCount);
        }
        public void Add(List<I> intervals, IndexingMode mode, uint collectionID, int threads)
        {
            int start = 0, stop = 0, count = 0, range = (int)Math.Ceiling(intervals.Count / (double)threads);
            var addedBookmarks = new ConcurrentDictionary<int, int>();

            if (_options.ActiveIndexes != IndexType.OnlyInverted)
            {
                using (WorkQueue work = new WorkQueue(threads))
                {
                    for (int i = 0; i < threads; i++)
                    {
                        start = i * range;
                        stop = (i + 1) * range;
                        if (stop > intervals.Count) stop = intervals.Count;
                        work.Enqueue(new BatchIndex<C, I, M>(_di4_incIdx, collectionID, intervals, start, stop, mode, addedBookmarks).Run);
                    }

                    work.Complete(true, -1);
                }

                count = 0;
                foreach (var item in addedBookmarks)
                    count += item.Value;
                _indexesCardinality.AddOrUpdate(_keyCardinalityIncIndx, count);
            }
        }
        public void SecondPass()
        {
            if (_options.ActiveIndexes != IndexType.OnlyInverted)            
                incBatchIndex.SecondPass();   
        }


        public void SecondResolutionIndex()
        {
            SecondResolutionIndex(CuttingMethod.ZeroThresholding, 0, Environment.ProcessorCount);
        }
        public void SecondResolutionIndex(CuttingMethod cuttingMethod, int binCount, int nThreads)
        {
            // TODO: change first resolution options here to be readonly and readonly lock.

            var addedBlocks = new ConcurrentDictionary<C, int>();

            //Partition<C>[] partitions = Partition_1RInc(nThreads);

            KeyValuePair<C, B> firstElement;
            _di4_incIdx.TryGetFirst(out firstElement);

            KeyValuePair<C, B> lastElement;
            _di4_incIdx.TryGetLast(out lastElement);

            nThreads = 1;

            using (WorkQueue work = new WorkQueue(nThreads))
            {
                for (int i = 0; i < nThreads; i++)
                    work.Enqueue(
                        new BatchIndex2R<C, I, M>(
                            _di4_incIdx,
                            _di4_2R,
                            firstElement.Key,//partitions[i].left,
                            lastElement.Key,//partitions[i].right,
                            cuttingMethod,
                            binCount,
                            addedBlocks).Run);

                work.Complete(true, -1);
            }

            int counted = 0;
            foreach (var item in addedBlocks)
                counted += item.Value;
            _indexesCardinality.AddOrUpdate(_keyCardinality2R, counted);
        }


        public void Cover<O>(IOutput<C, I, M, O> outputStrategy, int minAccumulation, int maxAccumulation)
        {
            Cover<O>(outputStrategy, minAccumulation, maxAccumulation, Environment.ProcessorCount);
        }
        public void Cover<O>(IOutput<C, I, M, O> outputStrategy, int minAccumulation, int maxAccumulation, int nThreads)
        {
            if (_indexesCardinality.GetValue(_keyCardinality2R) == 0)
                throw new InvalidOperationException("The second-resolution index is required, which is not populated yet.");

            object lockOnMe = new object();
            PartitionBlock<C>[] partitions = Partition_2R(nThreads);

            using (WorkQueue work = new WorkQueue(nThreads))
            {
                for (int i = 0; i < nThreads; i++)
                    work.Enqueue(
                        new CoverSummit<C, I, M, O>(
                            lockOnMe,
                            _di4_incIdx,
                            _di4_2R,
                            outputStrategy,
                            partitions[i].left,
                            partitions[i].right,
                            minAccumulation,
                            maxAccumulation//,
                                           //_indexesCardinality.GetValue(_keyCardinalityIncIndx)
                            ).Cover);

                work.Complete(true, -1);
            }
        }
        public void Summit<O>(IOutput<C, I, M, O> outputStrategy, int minAccumulation, int maxAccumulation)
        {
            Summit<O>(outputStrategy, minAccumulation, maxAccumulation, Environment.ProcessorCount);
        }
        public void Summit<O>(IOutput<C, I, M, O> outputStrategy, int minAccumulation, int maxAccumulation, int nThreads)
        {
            if (_indexesCardinality.GetValue(_keyCardinality2R) == 0)
                throw new InvalidOperationException("The second-resolution index is required, which is not populated yet.");

            object lockOnMe = new object();
            PartitionBlock<C>[] partitions = Partition_2R(nThreads);

            using (WorkQueue work = new WorkQueue(nThreads))
            {
                for (int i = 0; i < nThreads; i++)
                    work.Enqueue(
                        new CoverSummit<C, I, M, O>(
                            lockOnMe,
                            _di4_incIdx,
                            _di4_2R,
                            outputStrategy,
                            partitions[i].left,
                            partitions[i].right,
                            minAccumulation,
                            maxAccumulation//,
                                           //_indexesCardinality.GetValue(_keyCardinalityIncIndx)
                            ).Summit);

                work.Complete(true, -1);
            }
        }

        public void Map<O>(ref IOutput<C, I, M, O> outputStrategy, List<I> references)
        {
            Map<O>(ref outputStrategy, references, Environment.ProcessorCount, default(C), default(C));
        }
        
        public void Map<O>(ref IOutput<C, I, M, O> outputStrategy, List<I> references, int nThreads)
        {
            Map<O>(ref outputStrategy, references, nThreads, default(C), default(C));
        }
        public void Map<O>(ref IOutput<C, I, M, O> outputStrategy, List<I> references, int nThreads, C UDF, C DDF)
        {
            object lockOnMe = new object();
            int start = 0, stop = 0, range = (int)Math.Ceiling(references.Count / (double)nThreads);

            using (WorkQueue work = new WorkQueue(nThreads))
            {
                for (int i = 0; i < nThreads; i++)
                {
                    start = i * range;
                    stop = (i + 1) * range;
                    if (stop > references.Count) stop = references.Count;
                    if (start < stop) work.Enqueue(
                        new MapCount<C, I, M, O>(
                            lockOnMe, _di4_incIdx,
                            outputStrategy,
                            references,
                            start, stop, UDF, DDF).Run);
                    else break;
                }

                work.Complete(true, -1);
            }
        }


        public void VariantAnalysis<O>(ref IOutput<C, I, M, O> outputStrategy, List<I> references, int nThreads)
        {
            VariantAnalysis<O>(ref outputStrategy, references, nThreads, default(C), default(C));
        }
        public void VariantAnalysis<O>(ref IOutput<C, I, M, O> outputStrategy, List<I> references, int nThreads, C UDF, C DDF)
        {
            object lockOnMe = new object();
            /*int start = 0, stop = 0, range = (int)Math.Ceiling(references.Count / (double)nThreads);

            using (WorkQueue work = new WorkQueue(nThreads))
            {
                for (int i = 0; i < nThreads; i++)
                {
                    start = i * range;
                    stop = (i + 1) * range;
                    if (stop > references.Count) stop = references.Count;
                    if (start < stop) work.Enqueue(
                        new Inc.VariationAnalysis<C, I, M, O>(
                            lockOnMe, _di4_incIdx,
                            outputStrategy,
                            references,
                            start, stop, UDF, DDF).Run);
                    else break;
                }

                work.Complete(true, -1);
            }*/

            var tmp = new Tmp__Map__for__VA<C, I, M, O>(
                lockOnMe,
                _di4_incIdx,
                outputStrategy,
                references,
                0,
                references.Count, UDF, DDF);

            /*var tmp = new Inc.VariationAnalysis<C, I, M, O>(
                            lockOnMe, _di4_incIdx,
                            outputStrategy,
                            references,
                            0, references.Count, UDF, DDF, results);*/

            tmp.Run();
        }

        public SortedDictionary<int, int> LambdaSizeDis()
        {
            object lockOnMe = new object();
            var results = new SortedDictionary<int, int>();

            var statsClass = new StatsCalculator<C, I, M>(_di4_incIdx, _di4_incIdx.First().Key, _di4_incIdx.Last().Key, results, lockOnMe);
            statsClass.LambdaSizeDistribution();

            return results;
        }

        public List<AccEntry<C>> AccumulationHistogram()
        {
            return AccumulationHistogram(Environment.ProcessorCount);
        }
        public List<AccEntry<C>> AccumulationHistogram(int nThreads)
        {
            object lockOnMe = new object();
            var results = new List<AccEntry<C>>();

            var partitions = Partition_1RInc(nThreads);
            using (WorkQueue work = new WorkQueue(nThreads))
            {
                for (int i = 0; i < nThreads; i++)
                    work.Enqueue(new AccumulationStats<C, I, M>(_di4_incIdx, partitions[i].left, partitions[i].right, results, lockOnMe).AccHistogram);
                work.Complete(true, -1);
            }

            /// Partitions define dichotomies, hence iterating over ranges it would not be 
            /// possible to see inter-dichotomies gaps. Therefore, there will be #nThreads 
            /// gaps that are not reported in output. The following loop addresses the point.
            for (int i = 0; i < nThreads - 1; i++)
                results.Add(new AccEntry<C>(partitions[i].right, partitions[i + 1].left, 0));

            return results;
        }

        public SortedDictionary<int, int> AccumulationDistribution()
        {
            return AccumulationDistribution(Environment.ProcessorCount);
        }
        public SortedDictionary<int, int> AccumulationDistribution(int nThreads)
        {
            object lockOnMe = new object();
            var results = new SortedDictionary<int, int>();
            var partitions = Partition_1RInc(nThreads);
            using (WorkQueue work = new WorkQueue(nThreads))
            {
                for (int i = 0; i < nThreads; i++)
                    work.Enqueue(new AccumulationStats<C, I, M>(_di4_incIdx, partitions[i].left, partitions[i].right, results, lockOnMe).AccDistribution);
                work.Complete(true, -1);
            }

            /// These three lines are to make sure no currentAcc is 
            /// skipped. For instance, lets say results have 1, 2, and 4 
            /// as keys. Then 3 is skipped, and these lines will add 3 with 
            /// a value of 0 to the results.
            int maxValue = results.ElementAt(results.Count - 1).Key;
            for (int i = 0; i < maxValue; i++)
                if (!results.ContainsKey(i)) results.Add(i, 0);
            return results;
        }

        public ICollection<BlockKey<C>> Merge()
        {
            return Merge(Environment.ProcessorCount);
        }
        public ICollection<BlockKey<C>> Merge(int nThreads)
        {
            if (_indexesCardinality.GetValue(_keyCardinality2R) == 0)
                throw new InvalidOperationException("The second-resolution index is required, which is not populated yet.");

            /*Object lockOnMe = new Object();
            PartitionBlock<C>[] partitions = Fragment_2R(nThreads);
            var blocks = new SortedDictionary<BlockKey<C>, int>();
            using (WorkQueue work = new WorkQueue(nThreads))
            {
                for (int i = 0; i < nThreads; i++)
                    work.Enqueue(
                        new MergeComplement<C, I, M>(
                            _di4_2R,
                            partitions[i].left,
                            partitions[i].right,
                            blocks,
                            lockOnMe).Merge);

                work.Complete(true, -1);
            }
            return blocks;*/

            return _di4_2R.Keys;
        }

        public ICollection<BlockKey<C>> Complement()
        {
            return Complement(Environment.ProcessorCount);
        }
        public ICollection<BlockKey<C>> Complement(int nThreads)
        {
            if (_indexesCardinality.GetValue(_keyCardinality2R) == 0)
                throw new InvalidOperationException("The second-resolution index is required, which is not populated yet.");

            object lockOnMe = new object();
            PartitionBlock<C>[] partitions = Partition_2R(nThreads);
            var blocks = new SortedDictionary<BlockKey<C>, int>();
            using (WorkQueue work = new WorkQueue(nThreads))
            {
                for (int i = 0; i < nThreads; i++)
                    work.Enqueue(
                        new MergeComplement<C, I, M>(
                            _di4_2R,
                            partitions[i].left,
                            partitions[i].right,
                            blocks,
                            lockOnMe).Complement);

                work.Complete(true, -1);
            }

            for (int i = 0; i < nThreads - 1; i++)
            {
                var partitionsBlock = new BlockKey<C>(partitions[i].right.rightEnd, partitions[i + 1].left.leftEnd);
                if (!blocks.ContainsKey(partitionsBlock))
                    blocks.Add(partitionsBlock, 0);
            }

            return blocks.Keys;
        }

        public ICollection<BlockKey<C>> Dichotomies()
        {
            if (_indexesCardinality.GetValue(_keyCardinality2R) == 0)
                throw new InvalidOperationException("The second-resolution index is required, which is not populated yet.");

            ICollection<C> keys = _di4_incIdx.Keys;
            var results = new SortedDictionary<BlockKey<C>, bool>();
            int keysCount = bookmarkCount;

            for (int key = 0; key < keysCount - 1; key++)
                results.Add(new BlockKey<C>(LeftEnd: keys.ElementAt(key), RightEnd: keys.ElementAt(key + 1)), true);

            var keys2R = _di4_2R.Keys;
            for (int key = 0; key < blockCount - 1; key++)
                results.Remove(new BlockKey<C>(LeftEnd: keys2R.ElementAt(key).rightEnd, RightEnd: keys2R.ElementAt(key + 1).leftEnd));

            return results.Keys;
        }

        public BlockInfoDis BlockInfoDistributions()
        {
            if (_indexesCardinality.GetValue(_keyCardinality2R) == 0)
                throw new InvalidOperationException("The second-resolution index is required, which is not populated yet.");

            var rtv = new BlockInfoDis();

            foreach(var block in _di4_2R.EnumerateFrom(_di4_2R.First().Key))
            {
                if (rtv.intervalCountDis.ContainsKey(block.Value.intervalCount))
                    rtv.intervalCountDis[block.Value.intervalCount]++;
                else
                    rtv.intervalCountDis.Add(block.Value.intervalCount, 1);

                if (rtv.maxAccDis.ContainsKey(block.Value.boundariesUpperBound))
                    rtv.maxAccDis[block.Value.boundariesUpperBound]++;
                else
                    rtv.maxAccDis.Add(block.Value.boundariesUpperBound, 1);
            }

            return rtv;
        }

        public Stats Statistics()
        {
            if (_options.ActiveIndexes == IndexType.Both || _options.ActiveIndexes == IndexType.OnlyIncremental)
                return new Stats(0, _indexesCardinality.GetValue(_keyCardinalityIncIndx), _indexesCardinality.GetValue(_keyCardinality2R));
            else
                return new Stats(0, _indexesCardinality.GetValue(_keyCardinalityInvIndx), _indexesCardinality.GetValue(_keyCardinality2R));
        }
        

        private Partition<C>[] Partition_1RInc(int fCount)
        {
            int range = Convert.ToInt32(Math.Floor(_indexesCardinality.GetValue(_keyCardinalityIncIndx) / (double)fCount));

            /// Initialization
            Partition<C>[] partitions = new Partition<C>[fCount];
            for (int i = 0; i < fCount; i++)
            {
                partitions[i].left = _di4_incIdx.ElementAtOrDefault((i * range) + 1).Key;
                partitions[i].right = _di4_incIdx.ElementAtOrDefault((i + 1) * range).Key;
            }
            partitions[0].left = _di4_incIdx.First().Key;
            partitions[fCount - 1].right = _di4_incIdx.Last().Key;

            /// Refinement
            bool incrementRight = true;
            fCount--;
            for (int i = 0; i < fCount; i++)
            {
                foreach (var bookmark in _di4_incIdx.EnumerateFrom(partitions[i].right))
                {
                    if (incrementRight)
                    {
                        partitions[i].right = bookmark.Key;
                        if (bookmark.Value.lambda.Count - bookmark.Value.omega + bookmark.Value.mu == 0)
                            incrementRight = false;
                        continue;
                    }
                    else
                    {
                        partitions[i + 1].left = bookmark.Key;
                        break;
                    }                    
                }

                if (partitions[i + 1].left.CompareTo(partitions[i + 1].right) == 1)
                    partitions[i + 1].right = partitions[i + 1].left;
                incrementRight = true;
            }

            return partitions;
        }
        private PartitionBlock<C>[] Partition_2R(int fCount)
        {
            // does this gets correct value ?
            int range = Convert.ToInt32(Math.Floor(_indexesCardinality.GetValue(_keyCardinality2R) / (double)fCount));

            /// Initialization
            PartitionBlock<C>[] partitions = new PartitionBlock<C>[fCount];
            for (int i = 0; i < fCount; i++)
            {
                partitions[i].left = _di4_2R.ElementAtOrDefault((i * range) ).Key;
                partitions[i].right = _di4_2R.ElementAtOrDefault((i ) * range).Key;
            }
            partitions[0].left = _di4_2R.First().Key;
            partitions[fCount - 1].right = _di4_2R.Last().Key;

            return partitions;
        }

        bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {// Protected implementation of Dispose pattern. 
            if (disposed)
                return;

            if (disposing)
            {
                // Free managed objects here. 
                _di4_incIdx.Commit();
                _di4_incIdx.Dispose();                
                _di4_2R.Commit();
                _di4_2R.Dispose();
                _di4_info.Commit();
                _di4_info.Dispose();
            }

            // Free unmanaged objects here. 
            disposed = true;
        }

        public void Commit()
        {
            _di4_incIdx.Commit();            
            _di4_2R.Commit();
            _di4_info.Commit();
        }
    }
}