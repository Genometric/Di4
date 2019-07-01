
namespace Genometric.Di4
{
    internal enum Phi : byte { LeftEnd = 0, Middle = 1, RightEnd = 2 };
    public enum LockMode : byte { WriterOnlyLocking, ReaderWriterLocking, SimpleReadWriteLocking, IgnoreLocking };
    public enum IndexingMode : byte { SinglePass, MultiPass };
    public enum IndexType : byte
    {
        /// <summary>
        /// Activates both Inverted index and 
        /// Incremental Inverted index.
        /// </summary>
        Both,

        /// <summary>
        /// Activates only Incremental
        /// Inverted index.
        /// </summary>
        OnlyIncremental,

        /// <summary>
        /// Activates only Inverted 
        /// index.
        /// </summary>
        OnlyInverted
    };
    internal enum RegionType : byte { Candidate, Designated, Decomposition };

    public enum CuttingMethod { ZeroThresholding, UniformScalarQuantization, NonUniformScalarQuantization };

    public enum Aggregate { Count, Sum, Maximum, Minimum, Median, Mean, StandardDeviation };
}
