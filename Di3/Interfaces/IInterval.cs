
using System;
namespace Interfaces
{
    public interface IInterval<C, M>
        where C : IFormattable
    {
        /// <summary>
        /// Gets the left-end of the interval.
        /// </summary>
        C left { set; get; }


        /// <summary>
        /// Gets the right-end of the interval.
        /// </summary>
        C right { set; get; }


        /// <summary>
        /// Sets and Gets the descriptive metadata
        /// of the interval. It could be a reference
        /// to a memory object, or a pointer, or 
        /// an entry ID on database, or etc. 
        /// </summary>
        M metadata { set; get; }
        

        uint hashKey { set; get; }
    }
}
