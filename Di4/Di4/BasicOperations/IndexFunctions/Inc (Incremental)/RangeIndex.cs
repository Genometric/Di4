using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;

namespace Genometric.Di4
{
    internal class RangeIndex<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal RangeIndex(BPlusTree<C, B> di4)
        {

        }

        /// <summary>
        /// Sets and gets the _di4_1R data structure
        /// to be manipulated. This data structure
        /// is in common between all classes of 
        /// namespace.
        /// </summary>
        private BPlusTree<C, B> _di4 { set; get; }
    }
}
