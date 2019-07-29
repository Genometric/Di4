
namespace Genometric.Di4
{
    internal enum Phi : byte { LeftEnd = 0, Middle = 1, RightEnd = 2 };
    public enum LockMode : byte { WriterOnlyLocking, ReaderWriterLocking, SimpleReadWriteLocking, IgnoreLocking };
    public enum IndexingMode : byte { SinglePass, MultiPass };

    internal enum RegionType : byte { Candidate, Designated, Decomposition };

    public enum CuttingMethod { ZeroThresholding, UniformScalarQuantization, NonUniformScalarQuantization };

    public enum Aggregate { Count, Sum, Maximum, Minimum, Median, Mean, StandardDeviation };
}
