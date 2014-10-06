using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IInterval;
using ICPMD;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System.IO;
using DI3.Interfaces;
using DI3.Di3Serializers;

namespace DI3
{
    /// <summary>
    /// Dynamic intervals inverted index (Di3) 
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
    public sealed class Di3<C, I, M> : Di3DataStructure<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : IMetaData<C>
    {
        /// <summary>
        /// Dynamic intervals inverted index (Di3) 
        /// is an indexing system aimed at providing
        /// efficient means of processing the intervals
        /// it indexes for common information retrieval 
        /// tasks.
        /// </summary>
        //public Di3()
        //{
            //var options = new BPlusTree<C, B<C, M>>.OptionsV2(CoorSeri, BlockSerializer/*, put also the comparer here*/);
            //options.CalcBTreeOrder(16, 24);
            //options.CreateFile = CreatePolicy.Always;
            //options.FileName = Path.GetTempFileName();

            //di3 = new BPlusTree<C, B<C, M>>(options);
            //INDEX = new INDEX<C, I, M>(di3);
            //FIND = new FIND<C, I, M>(di3);
        //}

        /// <summary>
        /// WRITE BETTER DESCRIPTION.
        /// this is used when the coordiante is of primitive type and 
        /// can't be defined using protobuf-net serialization; hence 
        /// a seprate serializer is passed to di3. 
        /// </summary>
        /// <param name="coordinateSerializer"></param>
        public Di3(ISerializer<C> coordinateSerializer, IComparer<C> comparer)
        {
            var options = new BPlusTree<C, B<C, M>>.OptionsV2(coordinateSerializer, BlockSerializer, comparer);
            options.CalcBTreeOrder(16, 24);
            options.CreateFile = CreatePolicy.Always;
            options.FileName = Path.GetTempFileName();

            di3 = new BPlusTree<C, B<C, M>>(options);
            INDEX = new INDEX<C, I, M>(di3);
            FIND = new FIND<C, I, M>(di3);
        }



        /// <summary>
        /// Is an instance of INDEX class which 
        /// provides efficient means of inserting an 
        /// interval to Di3; i.e., di3 indexding.
        /// </summary>
        private INDEX<C, I, M> INDEX { set; get; }


        /// <summary>
        /// Is an instance of FIND class which
        /// provides efficient means for searching for 
        /// a key in di3.
        /// </summary>
        private FIND<C, I, M> FIND { set; get; }


        /// <summary>
        /// Gets the number of blocks contained in Di3.
        /// </summary>
        public int blockCount { private set { } get { return di3.Count; } }


        /// <summary>
        /// Adds the provided interval to di3.
        /// </summary>
        /// <param name="interval">The interval
        /// to be added to the di3.</param>
        public void Add(I interval)
        {
            //di3.Add(interval.left, new B<C, M> { omega = 10 });

            //------------------------------------------------------
            // Following line is commented-out only for test purpose
            //------------------------------------------------------
            //INDEX.Index(interval);


            #region Test Section: AAAA
            /*var options = new BPlusTree<int, B<int, PeakDataClass>>.OptionsV2(PrimitiveSerializer.Int32, TESTBlockSerializer);
            options.CalcBTreeOrder(16, 24);
            options.CreateFile = CreatePolicy.Always;
            options.FileName = Path.GetTempFileName();


            var di3TEST = new BPlusTree<int, B<int, PeakDataClass>>(options);

            for (int i = 0; i < 10000000; i++)
            {
                var data = new PeakDataClass(){ chrNo = 0, hashKey = 100000000, left = i, right = i+2, name = "Hamed", strand = '+', value = 123.456};
                var lambda = new List<Lambda<int, PeakDataClass>>();
                lambda.Add(new Lambda<int, PeakDataClass>('M', data));
                di3TEST.Add(i, new B<int, PeakDataClass>(i) { omega = 0});
            }*/ 
            #endregion
        }

        public void Clean()
        {
            //GC.Collect();
            //GC.SuppressFinalize(this);
            //GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }


        public int FindIndex(C Key)
        {
            return FIND.FindIndex(Key);
        }


        public B<C, M> FindBlock(C Key)
        {
            return null;
        }

        public List<O> Cover<O>(ICSOutput<C, M, O> OutputStrategy, byte minAccumulation, byte maxAccumulation)
        {
            SetOperations<C, M, O> SetOps = new SetOperations<C, M, O>(di3);
            return SetOps.Cover(OutputStrategy, minAccumulation, maxAccumulation);
        }

        public List<O> Summit<O>(ICSOutput<C, M, O> OutputStrategy, byte minAccumulation, byte maxAccumulation)
        {
            SetOperations<C, M, O> SetOps = new SetOperations<C, M, O>(di3);
            return SetOps.Summit(OutputStrategy, minAccumulation, maxAccumulation);
        }
    }
}