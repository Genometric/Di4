using System;

namespace Interfaces
{
    public interface IExtMetaData/*<C>*/ : IMetaData
        //where C : IFormattable
    {
        string name { set; get; }
        //C left { set; get; }
        //C right { set; get; }
        double value { set; get; }
    }
}
