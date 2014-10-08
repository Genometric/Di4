using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System.IO;
using ProtoBuf;
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
    public class Di3<C, I, M>
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
        public Di3(string FileName, ISerializer<C> CSerializer, IComparer<C> comparer)
        {
            bSerializer = new BSerializer<C, M>();
            var options = new BPlusTree<C, B<C, M>>.OptionsV2(CSerializer, bSerializer, comparer);
            options.CalcBTreeOrder(16, 24);
            options.CreateFile = CreatePolicy.IfNeeded;
            options.FileName = FileName;

            di3 = new BPlusTree<C, B<C, M>>(options);
            INDEX = new INDEX<C, I, M>(di3);
        }

        public void Add(I interval)
        {
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
    }
}