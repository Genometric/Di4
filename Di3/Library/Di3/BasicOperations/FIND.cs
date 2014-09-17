using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IInterval;
using ICPMD;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System.IO;

namespace DI3
{
    /// <summary>
    /// Provides efficient means for searching for 
    /// a key in di3.
    /// </summary>
    /// <typeparam name="C">Represents the c/domain
    /// type (e.g,. int, double, Time).</typeparam>
    /// <typeparam name="I">Represents generic type of the interval.
    /// (e.g., time span, interval on natural numbers)
    /// <para>For intervals of possibly different types,
    /// it is recommended to define this generic type
    /// parameter in terms of Lowest Common Denominator.
    /// </para></typeparam>
    /// <typeparam name="M">Represents generic
    /// type of pointer to descriptive metadata cooresponding
    /// to the interval.</typeparam>
    internal class FIND<C, I, M> //: Di3DataStructure<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : IMetaData<C>
    {
        /// <summary>
        /// Provides efficient means for searching for 
        /// a key in di3.
        /// </summary>
        /// <param name="di3">The reference di3 to be 
        /// manipulated.</param>
        internal FIND(BPlusTree<C, B<C, M>> di3)
        {
            this.di3 = di3;
        }

        /// <summary>
        /// Sets and gets the di3 data structure
        /// to be manipulated. This data structure
        /// is in common between all classes of 
        /// namespace.
        /// </summary>
        private BPlusTree<C, B<C, M>> di3 { set; get; }

        /// <summary>
        /// The key to run a search for on Di3.
        /// </summary>
        private C key { set; get; }

        /// <summary>
        /// Searches for the key within Di3 and
        /// returns the index of the block which 
        /// it's coordinate matches the key. It 
        /// returns -1 if no block coordinate 
        /// matches the key.
        /// </summary>
        /// <param name="Key">The index of the block which 
        /// it's coordinate matches the key.</param>
        /// <returns></returns>
        internal int FindIndex(C Key)
        {
            key = Key;
            return DichotomicSearch(0, di3.Count);
        }


        /// <summary>
        /// Searches for the key within Di3 and
        /// returns the block which 
        /// it's coordinate matches the key. It 
        /// returns null if no block coordinate 
        /// matches the key.
        /// </summary>
        /// <param name="Key">The index of the block which 
        /// it's coordinate matches the key.</param>
        /// <returns></returns>
        internal B<C, M> FindBlock(C Key)
        {
            key = Key;

            int index = DichotomicSearch(0, di3.Count);

            if (index != -1)
                return di3[index];
            else
                return null;
        }

        private int DichotomicSearch(int left, int right)
        {
            while (left < right)
            {
                int mid = (int)Math.Floor((left + right) / 2.0);

                // code must guarantee the interval is reduced at each iteration
                // assert(imid < imax);
                // note: 0 <= imin < imax implies imid will always be less than imax

                // reduce the search
                if (di3[mid].e.CompareTo(key) == 'L')
                    left = mid + 1;
                else
                    right = mid;
            }

            // At exit of while:
            // if di3[] is empty, then right < left
            // otherwise right == left

            // deferred test for equality
            if ((right == left) && (di3[left].e.CompareTo(key) == 'E'))
                return left;
            else
                return -1;
        }
    }
}
