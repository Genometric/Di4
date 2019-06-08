namespace Genometric.Di4.Di4B
{
    public struct MaxDegreeOfParallelism
    {
        public MaxDegreeOfParallelism(int chrDegree, int di4Degree)
            : this()
        {
            this.chrDegree = chrDegree;
            this.di4Degree = di4Degree;
        }

        /// <summary>
        /// Gets the maximum degree of parallelism at chromosome level 
        /// (i.e., maximum number of chromosomes to be executed concurrently).
        /// </summary>
        public int chrDegree { private set; get; }

        /// <summary>
        /// Get the maximum degree of parallelism at di4 level
        /// (i.e., maximum number of threads reading/writting from/to di4 concurrently).
        /// </summary>
        public int di4Degree { private set; get; }
    }
}
