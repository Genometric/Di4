
namespace Polimi.DEIB.VahidJalili.DI3
{
    internal enum IntersectionCondition : byte { LeftEnd, Middle, RightEnd };
    public enum LockMode { WriterOnlyLocking, ReaderWriterLocking, SimpleReadWriteLocking, IgnoreLocking };
    public enum IndexingMode { SinglePass, MultiPass };
}
