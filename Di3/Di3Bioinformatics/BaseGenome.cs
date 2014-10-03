using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI3;
using DI3.Interfaces;
using IInterval;
using ICPMD;
using CSharpTest.Net.Serialization;

namespace Di3Bioinformatics
{
    /// <summary>
    /// Represents a Genome with multiple chromosomes and strands.
    /// </summary>
    /// <typeparam name="I">Represents generic type of the interval.</typeparam>
    /// <typeparam name="M">Represents generic type of pointer to
    /// descriptive metadata cooresponding to the interval.</typeparam>
    internal class BaseGenome<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : ICPMetadata<C>, IMetaData<C>
    {
        /// <summary>
        /// Represents a Genome with multiple chromosomes and strands.
        /// </summary>
        /// <param name="chrCount">The number of possible chromosomes
        /// of the genome.</param>
        public BaseGenome()
        {
            chrs = new Dictionary<string, Chromosome>();
        }

        /// <summary>
        /// Sets and Gets all chromosomes of the genome.
        /// </summary>
        internal Dictionary<string, Chromosome> chrs { set; get; }

        /// <summary>
        /// Represents a chromosome with different strands
        /// (i.e., positive/negative/un-stranded).
        /// </summary>
        internal class Chromosome
        {
            /// <summary>
            /// Represents a chromosome with different strands
            /// (i.e., positive/negative/un-stranded).
            /// </summary>
            internal Chromosome(ISerializer<C> coordinateSerializer)
            {
                di3PositiveStrand = new Di3<C, I, M>(coordinateSerializer);
                di3NegativeStrand = new Di3<C, I, M>(coordinateSerializer);
                di3Unstranded = new Di3<C, I, M>(coordinateSerializer);
            }

            /// <summary>
            /// Dynamic intervals inverted index for Positive Strand.
            /// </summary>
            internal Di3<C, I, M> di3PositiveStrand { set; get; }

            /// <summary>
            /// Dynamic intervals inverted index for Negative Strand.
            /// </summary>
            internal Di3<C, I, M> di3NegativeStrand { set; get; }

            /// <summary>
            /// Dynamic intervals inverted index for Un-Stranded.
            /// </summary>
            internal Di3<C, I, M> di3Unstranded { set; get; }
        }

        internal ISerializer<C> coordinateSerializer { set; get; }

        public void AddChromosome(string chr)
        {
            if (!chrs.ContainsKey(chr))
                chrs.Add(chr, new Chromosome(coordinateSerializer));
        }
    }
}
