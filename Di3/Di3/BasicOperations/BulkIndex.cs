﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using CSharpTest.Net.Collections;

namespace DI3
{
    internal class BulkIndex<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal BulkIndex(BPlusTree<C, B> di3, SortedDictionary<C,B> bookmarks)
        {
            _di3 = di3;
            _bookmarks = bookmarks;
        }

        /// <summary>
        /// Sets and gets the di3 data structure
        /// to be manipulated. This data structure
        /// is in common between all classes of 
        /// namespace.
        /// </summary>
        private BPlusTree<C, B> _di3 { set; get; }

        private SortedDictionary<C, B> _bookmarks { set; get; }

        public void Index()
        {

        }
    }
}
