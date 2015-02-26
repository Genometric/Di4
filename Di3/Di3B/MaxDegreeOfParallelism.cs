using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polimi.DEIB.VahidJalili.DI3.DI3B
{
    public struct MaxDegreeOfParallelism
    {
        public MaxDegreeOfParallelism(int chrDegree, int di3Degree)
            : this()
        {
            this.chrDegree = chrDegree;
            this.di3Degree = di3Degree;
        }

        /// <summary>
        /// Gets the maximum degree of parallelism at chromosome level 
        /// (i.e., maximum number of chromosomes to be executed concurrently).
        /// </summary>
        public int chrDegree { private set; get; }

        /// <summary>
        /// Get the maximum degree of parallelism at di3 level
        /// (i.e., maximum number of threads reading/writting from/to di3 concurrently).
        /// </summary>
        public int di3Degree { private set; get; }
    }
}
