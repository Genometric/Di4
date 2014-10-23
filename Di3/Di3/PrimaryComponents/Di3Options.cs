using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        enum LockMode { WriterOnlyLocking, ReaderWriterLocking, SimpleReadWriteLocking };
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
        public int CacheTimeOut { set; get; }
        public CachePolicy CachePolicy { set; get; }
    }
}
