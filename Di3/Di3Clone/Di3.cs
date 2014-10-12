using System;
using System.Collections.Generic;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using Interfaces;



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
        where M : IMetaData<C>
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
            Console.WriteLine("I'm in di3");

            bSerializer = new BSerializer<C, M>();
            var options = new BPlusTree<C, B<C, M>>.OptionsV2(CSerializer, bSerializer, comparer);

            //Console.WriteLine("I'm at at A");

            options.CalcBTreeOrder(avgKeySize, avgValueSize); //24);
            options.CreateFile = createPolicy;
            options.ExistingLogAction = ExistingLogAction.ReplayAndCommit;
            options.StoragePerformance = StoragePerformance.Fastest;

            //Console.WriteLine("I'm at at B");

            options.CachePolicy = CachePolicy.All;

            options.FileBlockSize = 512;

            if (createPolicy != CreatePolicy.Never)
                options.FileName = FileName;

            //Console.WriteLine("I'm at at C");

            //Console.WriteLine("I'm at at D");
            di3 = new BPlusTree<C, B<C, M>>(options);
            //Console.WriteLine("I'm at at E");
            INDEX = new INDEX<C, I, M>(di3);
            //Console.WriteLine("I'm at at F");

        }


        public Di3(
            string FileName,
            CreatePolicy createPolicy,
            ISerializer<C> CSerializer,
            IComparer<C> comparer)
        {
            Console.WriteLine("I'm in di3");

            bSerializer = new BSerializer<C, M>();
            var options = new BPlusTree<C, B<C, M>>.OptionsV2(CSerializer, bSerializer, comparer);

            //Console.WriteLine("I'm at at A");

            options.CalcBTreeOrder(16, 1400); //24);
            options.CreateFile = createPolicy;
            options.ExistingLogAction = ExistingLogAction.ReplayAndCommit;
            options.StoragePerformance = StoragePerformance.Fastest;

            //Console.WriteLine("I'm at at B");

            options.CachePolicy = CachePolicy.All;

            options.FileBlockSize = 512;

            if (createPolicy != CreatePolicy.Never)
                options.FileName = FileName;

            //Console.WriteLine("I'm at at C");

            //Console.WriteLine("I'm at at D");
            di3 = new BPlusTree<C, B<C, M>>(options);
            //Console.WriteLine("I'm at at E");
            INDEX = new INDEX<C, I, M>(di3);
            //Console.WriteLine("I'm at at F");

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
            //Console.WriteLine("I'm at in di3");

            bSerializer = new BSerializer<C, M>();
            var options = new BPlusTree<C, B<C, M>>.OptionsV2(CSerializer, bSerializer, comparer);

            //Console.WriteLine("I'm at at A");

            options.CalcBTreeOrder(16, 1400); //24);
            options.CreateFile = createPolicy;
            options.ExistingLogAction = ExistingLogAction.ReplayAndCommit;
            options.StoragePerformance = StoragePerformance.Fastest;
            options.MaximumChildNodes = maximumChildNodes;
            options.MinimumChildNodes = minimumChildNodes;
            options.MaximumValueNodes = maximumValueNodes;
            options.MinimumValueNodes = minimumValueNodes;

            //Console.WriteLine("I'm at at B");

            options.CachePolicy = CachePolicy.Recent;

            options.FileBlockSize = 512;

            if (createPolicy != CreatePolicy.Never)
                options.FileName = FileName;

            //Console.WriteLine("I'm at at C");

            //Console.WriteLine("I'm at at D");
            di3 = new BPlusTree<C, B<C, M>>(options);
            //Console.WriteLine("I'm at at E");
            INDEX = new INDEX<C, I, M>(di3);
            //Console.WriteLine("I'm at at F");
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
            //Console.WriteLine("I'm at in di3");

            bSerializer = new BSerializer<C, M>();
            var options = new BPlusTree<C, B<C, M>>.OptionsV2(CSerializer, bSerializer, comparer);

            //Console.WriteLine("I'm at at A");

            options.CalcBTreeOrder(avgKeySize, avgValueSize);
            options.CreateFile = createPolicy;
            options.ExistingLogAction = ExistingLogAction.ReplayAndCommit;
            options.StoragePerformance = StoragePerformance.Fastest;
            options.MaximumChildNodes = maximumChildNodes;
            options.MinimumChildNodes = minimumChildNodes;
            options.MaximumValueNodes = maximumValueNodes;
            options.MinimumValueNodes = minimumValueNodes;

            //Console.WriteLine("I'm at at B");

            options.CachePolicy = CachePolicy.Recent;

            options.FileBlockSize = 512;

            if (createPolicy != CreatePolicy.Never)
                options.FileName = FileName;

            //Console.WriteLine("I'm at at C");

            //Console.WriteLine("I'm at at D");
            di3 = new BPlusTree<C, B<C, M>>(options);
            //Console.WriteLine("I'm at at E");
            INDEX = new INDEX<C, I, M>(di3);
            //Console.WriteLine("I'm at at F");
        }


        public void Add(I interval)
        {
            //Console.WriteLine("I'm at at I");
            INDEX.Index(interval);
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