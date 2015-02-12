using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using CSharpTest.Net.Synchronization;
using CSharpTest.Net.Threading;
using IGenomics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI3
{
    /// <summary>
    /// Dynamic _intervals inverted index (DI3) 
    /// is an indexing system aimed at providing
    /// efficient means of processing the _intervals
    /// it indexes for common information retrieval 
    /// tasks.
    /// </summary>
    /// <typeparam name="C">Represents the c/domain
    /// type (e.g,. int, double, Time).</typeparam>
    /// <typeparam name="I">Represents generic type of the _interval.
    /// (e.g., time span, _interval on natural numbers)
    /// <para>For _intervals of possibly different types,
    /// it is recommended to define this generic type
    /// parameter in terms of Lowest Common Denominator.
    /// </para></typeparam>
    /// <typeparam name="M">Represents generic
    /// type of pointer to descriptive hashKey cooresponding
    /// to the _interval.</typeparam>
    public class Di3<C, I, M> : IDisposable
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        /// <summary>
        /// Dynamic _intervals inverted index (DI3) 
        /// is an indexing system aimed at providing
        /// efficient means of processing the _intervals
        /// it indexes for common information retrieval 
        /// tasks.
        /// </summary>
        /// <param name="CSerializer"></param>
        /// <param name="comparer"></param>
        private Di3(
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


            ////rtv.CalcBTreeOrder(avgKeySize, avgValueSize); //24);
            //options.CreateFile = createPolicy;
            ////rtv.ExistingLogAction = ExistingLogAction.ReplayAndCommit;
            //options.StoragePerformance = StoragePerformance.Fastest;


            //////// Multi-Threading 2nd pass test; was commented-out visa versa
            //rtv.CachePolicy = CachePolicy.All;
            //options.CachePolicy = CachePolicy.Recent;


            //rtv.FileBlockSize = 512;

            /*rtv.MaximumChildNodes = 8;
            rtv.MinimumChildNodes = 2;

            rtv.MaximumValueNodes = 8;
            rtv.MinimumValueNodes = 2;
            */


            /// There three lines added for multi-threading.
            //rtv.CallLevelLock = new ReaderWriterLocking();
            /////rtv.LockingFactory = new LockFactory<SimpleReadWriteLocking>(); //Test 1
            /////rtv.LockingFactory = new LockFactory<WriterOnlyLocking>(); //Test 2
            //rtv.LockingFactory = new LockFactory<ReaderWriterLocking>();
            //rtv.LockTimeout = 10000;




            //options.MaximumChildNodes = 256;// 100;
            //options.MinimumChildNodes = 2;//2;//10;

            //options.MaximumValueNodes = 256; // 100;
            //options.MinimumValueNodes = 2;//2;//10;

            //if (createPolicy != CreatePolicy.Never)
                //options.FileName = FileName;

            //_di3_1R = new BPlusTree<C, B>(options);
            //SingleIndex = new SingleIndex<C, I, M>(_di3_1R);

            //_di3_1R.DebugSetValidateOnCheckpoint(false);
        }


        public Di3(Di3Options<C> options)
        {
            _di3_1R = new BPlusTree<C, B>(Get1ROptions(options));
            _di3_2R = new BPlusTree<BlockKey<C>, BlockValue>(Get2ROptions(options));
            INDEX = new SingleIndex<C, I, M>(_di3_1R);

            /// This might slow-down the Add, Delete, and Update procedures.
            /// TODO: Test initialization with and without this command. 
            _di3_1R.EnableCount();
            _di3_2R.EnableCount();
        }

        /// <summary>
        /// Di3 First resolution
        /// </summary>
        private BPlusTree<C, B> _di3_1R { set; get; }
        private BPlusTree<BlockKey<C>, BlockValue> _di3_2R { set; get; }
        private BookmarkSerializer _bookmarkSerializer { set; get; }
        private LambdaItemSerializer _lambdaItemSerializer { set; get; }
        private LambdaArraySerializer _lambdaArraySerializer { set; get; }
        private BlockKeySerializer<C> _blockKeySerializer { set; get; }
        private BlockValueSerializer _blockValueSerializer { set; get; }

        /// <summary>
        /// Is an instance of SingleIndex class which 
        /// provides efficient means of inserting an 
        /// _interval to DI3; i.e., _di3_1R indexding.
        /// </summary>
        private SingleIndex<C, I, M> INDEX { set; get; }

        /// <summary>
        /// Gets the number of blocks contained in DI3.
        /// </summary>
        public int blockCount { private set { } get { return _di3_1R.Count; } }

        private BPlusTree<C, B>.OptionsV2 Get1ROptions(Di3Options<C> options)
        {
            _lambdaItemSerializer = new LambdaItemSerializer();
            _lambdaArraySerializer = new LambdaArraySerializer(_lambdaItemSerializer);
            _bookmarkSerializer = new BookmarkSerializer(_lambdaArraySerializer);
            var rtv = new BPlusTree<C, B>.OptionsV2(options.CSerializer, _bookmarkSerializer, options.Comparer);
            rtv.ReadOnly = options.OpenReadOnly;

            if (options.MaximumChildNodes >= 4 &&
                options.MinimumChildNodes >= 2 &&
                options.MaximumValueNodes >= 4 &&
                options.MinimumValueNodes >= 2)
            {
                rtv.MaximumChildNodes = options.MaximumChildNodes;
                rtv.MinimumChildNodes = options.MinimumChildNodes;
                rtv.MaximumValueNodes = options.MaximumValueNodes;
                rtv.MinimumValueNodes = options.MinimumValueNodes;
            }

            if (options.AverageKeySize != 0 && options.AverageValueSize != 0)
                rtv.CalcBTreeOrder(options.AverageKeySize, options.AverageValueSize);

            if (options.FileBlockSize != 0)
                rtv.FileBlockSize = options.FileBlockSize;

            rtv.CachePolicy = options.CachePolicy;
            if (options.CreatePolicy != CreatePolicy.Never)
                rtv.FileName = options.FileName + ".idx1R";

            rtv.CreateFile = options.CreatePolicy;
            rtv.ExistingLogAction = options.ExistingLogAction;
            rtv.StoragePerformance = options.StoragePerformance;

            rtv.CallLevelLock = new ReaderWriterLocking();
            if (options.LockTimeout > 0) rtv.LockTimeout = options.LockTimeout;

            switch (options.Locking)
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

            if (options.CacheMaximumHistory != 0 && options.CacheKeepAliveTimeOut != 0)
            {
                rtv.CacheKeepAliveMaximumHistory = options.CacheMaximumHistory;
                rtv.CacheKeepAliveMinimumHistory = options.CacheMinimumHistory;
                rtv.CacheKeepAliveTimeout = options.CacheKeepAliveTimeOut;
            }

            return rtv;
        }
        private BPlusTree<BlockKey<C>, BlockValue>.OptionsV2 Get2ROptions(Di3Options<C> options)
        {
            /// TODO
            /// Try to optimize these settings as much as possible.
            
            _blockKeySerializer = new BlockKeySerializer<C>(options.CSerializer);
            _blockValueSerializer = new BlockValueSerializer();

            var rtv = new BPlusTree<BlockKey<C>, BlockValue>.OptionsV2(_blockKeySerializer, _blockValueSerializer, new BlockKeyComparer<C>());
            rtv.ReadOnly = options.OpenReadOnly;

            rtv.MinimumChildNodes = 2;
            rtv.MaximumChildNodes = 256;
            rtv.MinimumValueNodes = 2;
            rtv.MaximumValueNodes = 256;

            rtv.FileBlockSize = 8192;

            rtv.CachePolicy = CachePolicy.Recent;

            rtv.StoragePerformance = StoragePerformance.Fastest;

            rtv.CachePolicy = options.CachePolicy;
            if (options.CreatePolicy != CreatePolicy.Never)
                rtv.FileName = options.FileName + ".idx2R";

            rtv.CreateFile = options.CreatePolicy;
            rtv.ExistingLogAction = options.ExistingLogAction;
            rtv.StoragePerformance = options.StoragePerformance;

            rtv.CallLevelLock = new ReaderWriterLocking();
            if (options.LockTimeout > 0) rtv.LockTimeout = options.LockTimeout;

            switch (options.Locking)
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

            if (options.CacheMaximumHistory != 0 && options.CacheKeepAliveTimeOut != 0)
            {
                rtv.CacheKeepAliveMaximumHistory = options.CacheMaximumHistory;
                rtv.CacheKeepAliveMinimumHistory = options.CacheMinimumHistory;
                rtv.CacheKeepAliveTimeout = options.CacheKeepAliveTimeOut;
            }

            return rtv;
        }



        public void Add(I interval)
        {
            INDEX.Index(interval);
        }
        public void Add(List<I> intervals, IndexingMode mode)
        {
            Add(intervals, mode, Environment.ProcessorCount);
        }
        public void Add(List<I> intervals, IndexingMode mode, int threads)
        {
            int start = 0, stop = 0, range = (int)Math.Ceiling(intervals.Count / (double)threads);
            using (WorkQueue work = new WorkQueue(threads))
            {
                for (int i = 0; i < threads; i++)
                {
                    start = i * range;
                    stop = (i + 1) * range;
                    if (stop > intervals.Count) stop = intervals.Count;
                    work.Enqueue(new SingleIndex<C, I, M>(_di3_1R, intervals, start, stop, mode).Index);
                }

                work.Complete(true, -1);
            }
        }
        public void SecondPass()
        {
            INDEX.SecondPass();
        }


        public int SecondResolutionIndex()
        {
            return SecondResolutionIndex(Environment.ProcessorCount);
        }
        public int SecondResolutionIndex(int nThreads)
        {
            // change first resolution options here to be readonly and readonly lock.

            Partition<C>[] partitions = Fragment_1R(nThreads);
            using (WorkQueue work = new WorkQueue(nThreads))
            {
                for (int i = 0; i < nThreads; i++)
                    work.Enqueue(
                        new SingleIndex2R<C, I, M>(
                            _di3_1R,
                            _di3_2R,
                            partitions[i].left,
                            partitions[i].right).Index);

                work.Complete(true, -1);
            }
            return _di3_2R.Count; // change this number.
        }


        public void Cover<O>(ref ICSOutput<C, I, M, O> outputStrategy, byte minAccumulation, byte maxAccumulation)
        {
            Cover<O>(ref outputStrategy, minAccumulation, maxAccumulation, Environment.ProcessorCount);
        }
        public void Cover<O>(ref ICSOutput<C, I, M, O> outputStrategy, byte minAccumulation, byte maxAccumulation, int nThreads)
        {
            PartitionBlock<C>[] partitions = Fragment_2R(nThreads);
            using (WorkQueue work = new WorkQueue(nThreads))
            {
                for (int i = 0; i < nThreads; i++)
                    work.Enqueue(
                        new HigherOrderFuncs<C, I, M, O>(
                            _di3_1R,
                            _di3_2R,
                            outputStrategy,
                            partitions[i].left,
                            partitions[i].right,
                            minAccumulation,
                            maxAccumulation).Cover);

                work.Complete(true, -1);
            }
        }
        public void Summit<O>(ref ICSOutput<C, I, M, O> outputStrategy, byte minAccumulation, byte maxAccumulation)
        {
            Summit<O>(ref outputStrategy, minAccumulation, maxAccumulation, Environment.ProcessorCount);
        }
        public void Summit<O>(ref ICSOutput<C, I, M, O> outputStrategy, byte minAccumulation, byte maxAccumulation, int nThreads)
        {
            PartitionBlock<C>[] partitions = Fragment_2R(nThreads);
            using (WorkQueue work = new WorkQueue(nThreads))
            {
                for (int i = 0; i < nThreads; i++)
                    work.Enqueue(
                        new HigherOrderFuncs<C, I, M, O>(
                            _di3_1R,
                            _di3_2R,
                            outputStrategy,
                            partitions[i].left,
                            partitions[i].right,
                            minAccumulation,
                            maxAccumulation).Summit);

                work.Complete(true, -1);
            }
        }

        public void Map<O>(ref ICSOutput<C, I, M, O> outputStrategy, List<I> references)
        {
            Map<O>(ref outputStrategy, references, Environment.ProcessorCount);
        }
        public void Map<O>(ref ICSOutput<C, I, M, O> outputStrategy, List<I> references, int nThreads)
        {
            //Stopwatch watch = new Stopwatch();
            int start = 0, stop = 0, range = (int)Math.Ceiling(references.Count / (double)nThreads);

            using (WorkQueue work = new WorkQueue(nThreads))
            {
                for (int i = 0; i < nThreads; i++)
                {
                    start = i * range;
                    stop = (i + 1) * range;
                    if (stop > references.Count) stop = references.Count;
                    if (start < stop) work.Enqueue(new HigherOrderFuncs<C, I, M, O>(_di3_1R, outputStrategy, references, start, stop).Map);
                    else break;
                }

                //watch.Restart();
                work.Complete(true, -1);
                //watch.Stop();
                //Console.WriteLine("waited : {0}ms", watch.ElapsedMilliseconds);
            }
        }

        private Partition<C>[] Fragment_1R(int fCount)
        {
            int range = Convert.ToInt32(Math.Floor((double)_di3_1R.Count / (double)fCount));

            /// Initialization
            Partition<C>[] partitions = new Partition<C>[fCount];
            for (int i = 0; i < fCount; i++)
            {
                partitions[i].left = _di3_1R.ElementAtOrDefault((i * range) + 1).Key;
                partitions[i].right = _di3_1R.ElementAtOrDefault((i + 1) * range).Key;
            }
            partitions[0].left = _di3_1R.First().Key;
            partitions[fCount - 1].right = _di3_1R.Last().Key;

            /// Refinement
            bool incrementRight = true;
            fCount--;
            for (int i = 0; i < fCount; i++)
            {
                foreach (var bookmark in _di3_1R.EnumerateFrom(partitions[i].right))
                {
                    if (incrementRight)
                    {
                        partitions[i].right = bookmark.Key;
                        if (bookmark.Value.omega == bookmark.Value.lambda.Count)
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
        private PartitionBlock<C>[] Fragment_2R(int fCount)
        {
            int range = Convert.ToInt32(Math.Floor((double)_di3_2R.Count / (double)fCount));

            /// Initialization
            PartitionBlock<C>[] partitions = new PartitionBlock<C>[fCount];
            for (int i = 0; i < fCount; i++)
            {
                partitions[i].left = _di3_2R.ElementAtOrDefault((i * range) + 1).Key;
                partitions[i].right = _di3_2R.ElementAtOrDefault((i + 1) * range).Key;
            }
            partitions[0].left = _di3_2R.First().Key;
            partitions[fCount - 1].right = _di3_2R.Last().Key;

            /// Refinement -------- Check if we need Refinement.
            /*bool incrementRight = true;
            fCount--;
            for (int i = 0; i < fCount; i++)
            {
                foreach (var bookmark in _di3_2R.EnumerateFrom(partitions[i].right))
                {
                    if (incrementRight)
                    {
                        partitions[i].right = bookmark.Key;
                        if (bookmark.Value.omega == bookmark.Value.lambda.Count)
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
            }*/

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
                _di3_1R.Commit();
                _di3_1R.Dispose();
            }

            // Free unmanaged objects here. 
            disposed = true;
        }
    }
}


/// Note 1:
/// Cover, Summit, and Map manipulate the reference of outputStrategy. 
/// During this process, outputStrategy is possibly modified outside the scope of these functions. 
/// To avoid any issues, a possible scenario could be initializing a brand-new 
/// instance of outputStrategy in each of the functions, and when finished, 
/// replace the referenced type by the new instance. 