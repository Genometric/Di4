using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System;
using System.Collections.Generic;

namespace DI3
{
    public class Di3Options<C>
        where C : IComparable<C>
    {
        public Di3Options(string FileName,
            CreatePolicy CreatePolicy,
            ISerializer<C> CSerializer,
            IComparer<C> Comparer)
        {
            this.FileName = FileName;
            this.CreatePolicy = CreatePolicy;
            this.CSerializer = CSerializer;
            this.Comparer = Comparer;
        }

        public bool OpenReadOnly { set; get; }
        internal string FileName { set; get; }
        internal CreatePolicy CreatePolicy { set; get; }
        internal ISerializer<C> CSerializer { set; get; }
        internal IComparer<C> Comparer { set; get; }
        public int FileBlockSize { set; get; }
        public int AverageKeySize { set; get; }
        public int AverageValueSize { set; get; }
        public int MaximumChildNodes { set; get; }
        public int MinimumChildNodes { set; get; }
        public int MaximumValueNodes { set; get; }
        public int MinimumValueNodes { set; get; }
        public LockMode Locking { set; get; }
        public int LockTimeout { set; get; }
        public int CacheMaximumHistory { set; get; }
        public int CacheMinimumHistory { set; get; }
        public int CacheKeepAliveTimeOut { set; get; }
        public CachePolicy CachePolicy { set; get; }
        public ExistingLogAction ExistingLogAction { set; get; }
        public StoragePerformance StoragePerformance { set; get; }
    }
}
