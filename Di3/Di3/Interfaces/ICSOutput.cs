using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI3
{
    /// <summary>
    /// Interface for Cover/Summit Output.
    /// </summary>
    /// <typeparam name="C">Represents the c/domain
    /// type (e.g,. int, double, Time).</typeparam>
    /// <typeparam name="I">Represents generic type of the _interval.
    /// (e.g., time span, _interval on natural numbers)
    /// <para>For _intervals of possibly different types,
    /// it is recommended to define this generic type
    /// parameter in terms of Lowest Common Denominator.
    /// </para></typeparam>
    /// <typeparam name="M">Represents generic
    /// type of pointer to descriptive atI cooresponding
    /// to the _interval.</typeparam>
    /// <typeparam name="O"></typeparam>
    public interface ICSOutput<C, I, M, O>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        void Output(C left, C right, List<Lambda> intervals);

        void Output(I interval, List<Lambda> intervals);

        ConcurrentBag<O> output { set; get; }
    }
}
