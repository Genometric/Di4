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
using ProtoBuf;

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
        public Di3()
        {
            BPlusTree<C, B<C, M>>.OptionsV2 options =
            options = new BPlusTree<C, B<C, M>>.OptionsV2(CoorSeri, BlockSerializer/*, put also the comparer here*/);
            options.CalcBTreeOrder(16, 24);
            options.CreateFile = CreatePolicy.Always;
            options.FileName = Path.GetTempFileName();

            di3 = new BPlusTree<C, B<C, M>>(options);
            INDEX = new INDEX<C, I, M>(di3);
            FIND = new FIND<C, I, M>(di3);


            TESTCLASSSSSS = new TESTCLASSSerializer();
            INTWRAPEEERRR = new INTWRAPPERSERIALZER<int>();
            BPlusTree<INTWRAPPER<int>, TESTCLASS>.OptionsV2 TESToptions =
            TESToptions = new BPlusTree<INTWRAPPER<int>, TESTCLASS>.OptionsV2(INTWRAPEEERRR, TESTCLASSSSSS/*, put also the comparer here*/);
            TESToptions.CalcBTreeOrder(16, 24);
            TESToptions.CreateFile = CreatePolicy.Always;
            TESToptions.FileName = Path.GetTempFileName();

            var TESTdi3 = new BPlusTree<INTWRAPPER<int>, TESTCLASS>(TESToptions);

            for (int i = 0; i < 100000000; i++)
            {
                TESTdi3.Add(new INTWRAPPER<int>() { THEKEY = i }, new TESTCLASS() { TESTVALUE1 = 10, TESTVALUE2 = "HAMEDJALILIVAHIDFARZANEJALILIYAZDANI." });
            }

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
            // testing purpose only: 
            if (!di3.ContainsKey(interval.left))
                di3.Add(interval.left, new B<C, M>() { omega = 1, /*e = interval.left */});

            // uncomment after test
            //INDEX.Index(interval);
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

        public TESTCLASSSerializer TESTCLASSSSSS { set; get; }
        public INTWRAPPERSERIALZER<int> INTWRAPEEERRR { set; get; }
    }


    [ProtoContract]
    public class TESTCLASS
    {
        [ProtoMember(1)]
        public int TESTVALUE1 { set; get; }

        [ProtoMember(2)]
        public string TESTVALUE2 { set; get; }
    }

    [ProtoContract]
    public class INTWRAPPER<C> : IComparable<INTWRAPPER<C>>
        where C : IComparable<C>
    {
        [ProtoMember(1)]
        public C THEKEY { set; get; }

        public int CompareTo(INTWRAPPER<C> other)
        {
            return this.THEKEY.CompareTo(other.THEKEY);
        }
    }



    public class TESTCLASSSerializer : ISerializer<TESTCLASS>
    {
        TESTCLASS ISerializer<TESTCLASS>.ReadFrom(Stream stream)
        {
            return Serializer.Deserialize<TESTCLASS>(stream);
        }
        public void WriteTo(TESTCLASS value, Stream stream)
        {
            Serializer.Serialize<TESTCLASS>(stream, value);
        }
    }

    public class INTWRAPPERSERIALZER<C> : ISerializer<INTWRAPPER<C>>
        where C : IComparable<C>
    {
        INTWRAPPER<C> ISerializer<INTWRAPPER<C>>.ReadFrom(Stream stream)
        {
            try
            {
                return Serializer.Deserialize<INTWRAPPER<C>>(stream);
            }
            catch(Exception exp)
            {

            }
            return null;
        }
        public void WriteTo(INTWRAPPER<C> value, Stream stream)
        {
            Serializer.Serialize<INTWRAPPER<C>>(stream, value);
        }
    }
}