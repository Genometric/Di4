using System;
using System.Collections.Generic;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using Interfaces;
using CSharpTest.Net.Threading;
using System.Diagnostics;
using CSharpTest.Net.Synchronization;



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
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        private BPlusTree<C, B> di3 { set; get; }
        private BlockSerializer blockSerializer { set; get; }
        private LambdaItemSerializer lambdaItemSerializer { set; get; }
        private LambdaArraySerializer lambdaArraySerializer { set; get; }


        /// <summary>
        /// Is an instance of INDEX class which 
        /// provides efficient means of inserting an 
        /// _interval to DI3; i.e., _di3 indexding.
        /// </summary>
        private INDEX<C, I, M> INDEX { set; get; }

        /// <summary>
        /// Gets the number of blocks contained in DI3.
        /// </summary>
        public int blockCount { private set { } get { return di3.Count; } }


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

            //_di3 = new BPlusTree<C, B>(options);
            //INDEX = new INDEX<C, I, M>(_di3);

            //_di3.DebugSetValidateOnCheckpoint(false);
        }


        public Di3(Di3Options<C> options)
        {
            di3 = new BPlusTree<C, B>(GetTreeOptions(options));
            INDEX = new INDEX<C, I, M>(di3);
        }

        private BPlusTree<C, B>.OptionsV2 GetTreeOptions(Di3Options<C> options)
        {
            //bSerializer = new BSerializer<C, M>();
            lambdaItemSerializer = new LambdaItemSerializer();
            lambdaArraySerializer = new LambdaArraySerializer(lambdaItemSerializer);
            blockSerializer = new BlockSerializer(lambdaArraySerializer);
            var rtv = new BPlusTree<C, B>.OptionsV2(options.CSerializer, blockSerializer, options.Comparer);
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
                rtv.FileName = options.FileName;

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
        { INDEX.Index(interval); }
        public void Add(List<I> intervals, Mode mode)
        {
            Add(intervals, Environment.ProcessorCount, mode);
        }
        public void Add(List<I> intervals, int threads, Mode mode)
        {
            Stopwatch watch = new Stopwatch();

            int start = 0, stop = 0, range = (int)Math.Ceiling(intervals.Count / (double)threads);
            using (WorkQueue work = new WorkQueue(threads))
            {
                for (int i = 0; i < threads; i++)
                {
                    start = i * range;
                    stop = (i + 1) * range;
                    if (stop > intervals.Count) stop = intervals.Count;
                    work.Enqueue(new INDEX<C, I, M>(di3, intervals, start, stop, mode).Index);
                }

                watch.Restart();
                work.Complete(true, -1);
                watch.Stop();
                Console.WriteLine("waited : {0}ms", watch.ElapsedMilliseconds);
            }
        }
        public void SecondPass()
        {
            INDEX.SecondPass();
        }

        public List<O> Cover<O>(ICSOutput<C, I, M, O> OutputStrategy, byte minAccumulation, byte maxAccumulation)
        {
            HigherOrderFuncs<C, I, M, O> SetOps = new HigherOrderFuncs<C, I, M, O>(di3);
            return SetOps.Cover(OutputStrategy, minAccumulation, maxAccumulation);
        }
        public List<O> Summit<O>(ICSOutput<C, I, M, O> OutputStrategy, byte minAccumulation, byte maxAccumulation)
        {
            HigherOrderFuncs<C, I, M, O> SetOps = new HigherOrderFuncs<C, I, M, O>(di3);
            return SetOps.Summit(OutputStrategy, minAccumulation, maxAccumulation);
        }

        public List<O> Map<O>(ICSOutput<C, I, M, O> OutputStrategy, List<I> references)
        {
            HigherOrderFuncs<C, I, M, O> SetOps = new HigherOrderFuncs<C, I, M, O>(di3);
            return SetOps.Map(OutputStrategy, references);
        }
        public List<O> Map<O>(ICSOutput<C, I, M, O> OutputStrategy, List<I> references, int threads)
        {
            Stopwatch watch = new Stopwatch();
            int start = 0, stop = 0, range = (int)Math.Ceiling(references.Count / (double)threads);

            using (WorkQueue work = new WorkQueue(threads))
            {
                for (int i = 0; i < threads; i++)
                {
                    start = i * range;
                    stop = (i + 1) * range;
                    if (stop > references.Count) stop = references.Count;
                    if (start < stop) work.Enqueue(new HigherOrderFuncs<C, I, M, O>(di3, OutputStrategy, references, start, stop).Map);
                    else break;
                }

                watch.Restart();
                work.Complete(true, -1);
                watch.Stop();
                Console.WriteLine("waited : {0}ms", watch.ElapsedMilliseconds);
            }

            //HigherOrderFuncs<C, I, M, O> SetOps = new HigherOrderFuncs<C, I, M, O>(_di3);
            return null;//SetOps.Map(_outputStrategy, references);
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
                di3.Commit();
                di3.Dispose();
            }

            // Free unmanaged objects here. 
            disposed = true;
        }
    }
}