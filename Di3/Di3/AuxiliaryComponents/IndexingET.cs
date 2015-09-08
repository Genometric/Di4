using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polimi.DEIB.VahidJalili.DI3
{
    /// <summary>
    /// Gets indexing elapsed time of both Inverted Index
    /// and Incremental Inverted Index.
    /// </summary>
    public struct IndexingET
    {
        /// <summary>
        /// Gets the elapsed time of indexing inverted index.
        /// </summary>
        public double InvertedIndex { set; get; }

        /// <summary>
        /// Gets the elapsed time of incremental inverted index.
        /// </summary>
        public double IncrementalIndex { set; get; }
    }
}


// NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE
// THIS STRUCT IS FOR TESTING PURPOSE ONLY
// REMOVE IT AND ALL IT'S DEPENDANCIES 
// FOR A FINAL RELEASE.
