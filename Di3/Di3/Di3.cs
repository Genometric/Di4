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
    /// Dynamic intervals inverted index (DI3) 
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
    /// type of pointer to descriptive metadata cooresponding
    /// to the interval.</typeparam>
    public class Di3<C, I, M> : IDisposable
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : IMetaData/*<C>*/
    {
        private BPlusTree<C, B<C, M>> di3 { set; get; }
        private BSerializer<C, M> bSerializer { set; get; }


        /// <summary>
        /// Is an instance of INDEX class which 
        /// provides efficient means of inserting an 
        /// interval to DI3; i.e., di3 indexding.
        /// </summary>
        private INDEX<C, I, M> INDEX { set; get; }

        /// <summary>
        /// Gets the number of blocks contained in DI3.
        /// </summary>
        public int blockCount { private set { } get { return di3.Count; } }

        public Di3(
            string FileName,
            CreatePolicy createPolicy,
            ISerializer<C> CSerializer,
            IComparer<C> comparer)
        {
            bSerializer = new BSerializer<C, M>();
            var options = new BPlusTree<C, B<C, M>>.OptionsV2(CSerializer, bSerializer, comparer);

            options.CalcBTreeOrder(16, 1400); //24);
            options.CreateFile = createPolicy;
            options.ExistingLogAction = ExistingLogAction.ReplayAndCommit;
            options.StoragePerformance = StoragePerformance.Fastest;

            options.CachePolicy = CachePolicy.All;

            options.FileBlockSize = 512;

            if (createPolicy != CreatePolicy.Never)
                options.FileName = FileName;

            di3 = new BPlusTree<C, B<C, M>>(options);
            INDEX = new INDEX<C, I, M>(di3);
        }


        /// <summary>
        /// Dynamic intervals inverted index (DI3) 
        /// is an indexing system aimed at providing
        /// efficient means of processing the intervals
        /// it indexes for common information retrieval 
        /// tasks.
        /// </summary>
        /// <param name="CSerializer"></param>
        /// <param name="comparer"></param>
        public Di3(
            string FileName,
            CreatePolicy createPolicy,
            ISerializer<C> CSerializer,
            IComparer<C> comparer,
            int avgKeySize,
            int avgValueSize)
        {
            bSerializer = new BSerializer<C, M>();
            var options = new BPlusTree<C, B<C, M>>.OptionsV2(CSerializer, bSerializer, comparer);

            
            //options.CalcBTreeOrder(avgKeySize, avgValueSize); //24);
            options.CreateFile = createPolicy;
            //options.ExistingLogAction = ExistingLogAction.ReplayAndCommit;
            options.StoragePerformance = StoragePerformance.Fastest;

            options.CachePolicy = CachePolicy.All;
            //options.CachePolicy = CachePolicy.Recent;

            //options.FileBlockSize = 512;

            /*options.MaximumChildNodes = 8;
            options.MinimumChildNodes = 2;

            options.MaximumValueNodes = 8;
            options.MinimumValueNodes = 2;
            */


            /// There three lines added for multi-threading.
            //options.CallLevelLock = new ReaderWriterLocking();
            /////options.LockingFactory = new LockFactory<SimpleReadWriteLocking>(); //Test 1
            /////options.LockingFactory = new LockFactory<WriterOnlyLocking>(); //Test 2
            //options.LockingFactory = new LockFactory<ReaderWriterLocking>();
            //options.LockTimeout = 10000;


            options.MaximumChildNodes = 256;// 100;
            options.MinimumChildNodes = 2;//2;//10;

            options.MaximumValueNodes = 256; // 100;
            options.MinimumValueNodes = 2;//2;//10;

            if (createPolicy != CreatePolicy.Never)
                options.FileName = FileName;

            di3 = new BPlusTree<C, B<C, M>>(options);
            INDEX = new INDEX<C, I, M>(di3);

            //di3.DebugSetValidateOnCheckpoint(false);
        }        


        public Di3(
            string FileName,
            CreatePolicy createPolicy,
            ISerializer<C> CSerializer,
            IComparer<C> comparer,
            int maximumChildNodes,
            int minimumChildNodes,
            int maximumValueNodes,
            int minimumValueNodes)
        {
            bSerializer = new BSerializer<C, M>();
            var options = new BPlusTree<C, B<C, M>>.OptionsV2(CSerializer, bSerializer, comparer);

            /// The previous call had:
            //options.CalcBTreeOrder(avgKeySize, avgValueSize);
            
            /// Running version has CachePolicy.Recent
            options.CachePolicy = CachePolicy.All;
            
            options.FileBlockSize = 512;

            options.CreateFile = createPolicy;
            //options.ExistingLogAction = ExistingLogAction.ReplayAndCommit;
            //options.StoragePerformance = StoragePerformance.Fastest;
            options.MaximumChildNodes = maximumChildNodes;
            options.MinimumChildNodes = minimumChildNodes;
            options.MaximumValueNodes = maximumValueNodes;
            options.MinimumValueNodes = minimumValueNodes;            

            if (createPolicy != CreatePolicy.Never)
                options.FileName = FileName;

            di3 = new BPlusTree<C, B<C, M>>(options);
            INDEX = new INDEX<C, I, M>(di3);
        }


        public Di3(
            string FileName,
            CreatePolicy createPolicy,
            ISerializer<C> CSerializer,
            IComparer<C> comparer,
            int maximumChildNodes,
            int minimumChildNodes,
            int maximumValueNodes,
            int minimumValueNodes, 
            int avgKeySize,
            int avgValueSize)
        {
            bSerializer = new BSerializer<C, M>();
            var options = new BPlusTree<C, B<C, M>>.OptionsV2(CSerializer, bSerializer, comparer);

            options.CalcBTreeOrder(avgKeySize, avgValueSize);
            options.CreateFile = createPolicy;
            options.ExistingLogAction = ExistingLogAction.ReplayAndCommit;
            options.StoragePerformance = StoragePerformance.Fastest;
            options.MaximumChildNodes = maximumChildNodes;
            options.MinimumChildNodes = minimumChildNodes;
            options.MaximumValueNodes = maximumValueNodes;
            options.MinimumValueNodes = minimumValueNodes;

            options.CachePolicy = CachePolicy.Recent;

            options.FileBlockSize = 512;

            if (createPolicy != CreatePolicy.Never)
                options.FileName = FileName;

            di3 = new BPlusTree<C, B<C, M>>(options);
            INDEX = new INDEX<C, I, M>(di3);
        }

        public void Add(I interval)
        { }


        public int Add(I interval, int TEST_Sample_Number, int TEST_Region_Number)
        {
            INDEX.TEST_Sample_Number = TEST_Sample_Number;
            INDEX.TEST_Region_Number = TEST_Region_Number;


            /// only test
            /*var originalBlock = new B<C, M>();
            //aBlock.lambda.Add(new Lambda<C, M>('M', default(M))); // this should be error
            //aBlock.lambda.Add(new Lambda<C, M>('L', default(M))); // error
            //aBlock.lambda.Add(new Lambda<C, M>('R', default(M))); // error
            var anotherBlock = originalBlock.Update(new Lambda<C, M>('T', default(M)));
            originalBlock = originalBlock.Update(new Lambda<C, M>('2', default(M)));*/


            return INDEX.Index(interval);
        }

        public void Add(List<I> intervals)
        {
            Add(intervals, Environment.ProcessorCount);
        }
        public void Add(List<I> intervals, int threads)
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
                    work.Enqueue(new INDEX<C, I, M>(di3, intervals, start, stop).Index);
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



        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers. 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here. 
                di3.Commit();
                di3.Dispose();
            }

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }
    }
}