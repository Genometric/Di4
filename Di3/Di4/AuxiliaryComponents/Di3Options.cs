using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System;
using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI4
{
    public class Di4Options<C>
        where C : IComparable<C>
    {
        public Di4Options(
            CreatePolicy CreatePolicy,
            ISerializer<C> CSerializer,
            IComparer<C> Comparer,
            string FileName = "none")
        {
            this.FileName = FileName;
            this.CreatePolicy = CreatePolicy;
            this.CSerializer = CSerializer;
            this.Comparer = Comparer;
            ActiveIndexes = IndexType.Both;
        }

        public bool OpenReadOnly { set; get; }
        internal string FileName { set; get; }
        internal CreatePolicy CreatePolicy { set; get; }
        internal ISerializer<C> CSerializer { set; get; }
        internal IComparer<C> Comparer { set; get; }
        public IndexType ActiveIndexes { set; get; }
        public int FileBlockSize { set; get; }
        public int AverageKeySize { set; get; }
        public int AverageValueSize { set; get; }
        public int MaximumChildNodes { set; get; }
        public int MinimumChildNodes { set; get; }
        public int MaximumValueNodes { set; get; }
        public int MinimumValueNodes { set; get; }
        public LockMode Locking { set; get; }
        public int LockTimeout { set; get; }
        public CacheOptions cacheOptions { set; get; }
        public ExistingLogAction ExistingLogAction { set; get; }
        public StoragePerformance StoragePerformance { set; get; }
    }
}
