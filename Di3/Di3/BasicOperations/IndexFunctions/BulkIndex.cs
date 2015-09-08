using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI3
{
    internal class BulkIndex<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal BulkIndex(BPlusTree<C, IIB> di3, SortedDictionary<C,IIB> bookmarks)
        {
            _di3 = di3;
            _bookmarks = bookmarks;
        }

        /// <summary>
        /// Sets and gets the _di3_1R data structure
        /// to be manipulated. This data structure
        /// is in common between all classes of 
        /// namespace.
        /// </summary>
        private BPlusTree<C, IIB> _di3 { set; get; }

        private SortedDictionary<C, IIB> _bookmarks { set; get; }

        public void Index()
        {

        }
    }
}
