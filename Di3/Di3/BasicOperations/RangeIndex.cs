﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using CSharpTest.Net.Collections;

namespace DI3
{
    internal class RangeIndex<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal RangeIndex(BPlusTree<C,B> di3)
        {

        }

        /// <summary>
        /// Sets and gets the di3 data structure
        /// to be manipulated. This data structure
        /// is in common between all classes of 
        /// namespace.
        /// </summary>
        private BPlusTree<C, B> _di3 { set; get; }
    }
}
