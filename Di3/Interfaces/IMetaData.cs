using System;

namespace Interfaces
{
    public interface IMetaData
    {
        UInt32 hashKey { set; get; }
        /// maybe it would be better to change this to sample hashKey. 
    }
}
