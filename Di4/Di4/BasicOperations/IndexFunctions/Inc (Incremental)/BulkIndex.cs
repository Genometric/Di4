using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;

namespace Genometric.Di4.Inc
{
    internal class BulkIndex<C, I, M>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal BulkIndex(BPlusTree<C, B> di4, SortedDictionary<C, B> bookmarks)
        {
            _di4 = di4;
            _bookmarks = bookmarks;
        }

        /// <summary>
        /// Sets and gets the _di4_1R data structure
        /// to be manipulated. This data structure
        /// is in common between all classes of 
        /// namespace.
        /// </summary>
        private BPlusTree<C, B> _di4 { set; get; }

        private SortedDictionary<C, B> _bookmarks { set; get; }

        public void Index()
        {

        }
    }
}
