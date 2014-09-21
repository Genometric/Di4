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
using DI3.Di3Serializers;

namespace DI3
{
    /// <summary>
    /// Represents main di3 data structure. 
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
    public class Di3DataStructure<C, I, M> : B<C,  M>
        where C : IComparable<C>
        where M : IMetaData<C>
    {
        /// <summary>
        /// Di3 primary data structure; a list of Di3 blocks.
        /// </summary>
        internal BPlusTree<C, B<C, M>> di3 { set; get; }

        //internal BPlusTree<Int32, B<C, M>>.OptionsV2 options { set; get; }

        internal int Comparer(int This, int That)
        {
            return This.CompareTo(That);
        }

        public BlockSerializer<C, M> BlockSerializer { set; get; }

        // but should not be here ! 
        // think better, do I know how to seralize the coordinate ?
        public CoordinateSerializer<C> CoorSeri { set; get; }


        public Di3DataStructure()
        {
            //options = new BPlusTree<int, int>.OptionsV2(PrimitiveSerializer.Int32, PrimitiveSerializer.Int32);
            //options.CalcBTreeOrder(16, 24);
            //options.CreateFile = CreatePolicy.Always;
            //options.FileName = Path.GetTempFileName();
            //di3 = new BPlusTree<C, B<C, M>>(options);
            BlockSerializer = new BlockSerializer<C, M>();
            CoorSeri = new CoordinateSerializer<C>();
        }
    }
}
