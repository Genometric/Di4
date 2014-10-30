using System;
using System.Collections.Generic;
using DI3;
using Interfaces;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;


namespace Di3B
{
    public class Chromosome<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : IMetaData/*<C>*/
    {
        /// <summary>
        /// Represents a chromosome with different strands
        /// (i.e., positive/negative/un-stranded).
        /// </summary>
        public Chromosome(string FileName, ISerializer<C> CSerializer, IComparer<C> CComparer)
        {
            //di3PS = new Di3<C, I, M>(CSerializer, CComparer);
            //di3NS = new Di3<C, I, M>(CSerializer, CComparer);
           
            // use options here
            //di3US = new Di3<C, I, M>(FileName, CreatePolicy.Never, CSerializer, CComparer);
        }

        /// <summary>
        /// Dynamic intervals inverted index for Positive Strand.
        /// </summary>
        internal Di3<C, I, M> di3PS { set; get; }

        /// <summary>
        /// Dynamic intervals inverted index for Negative Strand.
        /// </summary>
        internal Di3<C, I, M> di3NS { set; get; }

        /// <summary>
        /// Dynamic intervals inverted index for Un-Stranded.
        /// </summary>
        internal Di3<C, I, M> di3US { set; get; }
    }
}
