using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Di3
{
    static internal class Modifiers<C, I> where I : IInterval<C>
    {
        public static void Insert(I interval)
        {
            e = interval.left;
        }

        /// <summary>
        /// Represents the coordinate (right or left-end) of interval being
        /// inserted to Di3.
        /// </summary>
        private static C e { set; get; }

        /// <summary>
        /// Represents the index of Bi
        /// </summary>
        private static int b { set; get; }

        public static List<B<C, I>> bi { set; get; }


        private void Find_Start_Insert_Index()
        {
            int s = 0;
            int biCardinality = bi.Count;
            e = Block_count;
            mid = 0;
            insert_index = 0;

            while (true)
            {
                mid = (int)Math.Floor((s + e) / 2.0);

                if (mid == e)
                {
                    insert_index = mid;
                    break;
                }
                else if (_position < index[mid].index)
                {
                    if (mid == 0)
                    {
                        insert_index = 0;
                        break;
                    }
                    else
                    {
                        e = mid;
                    }
                }
                else if (_position == index[mid].index)
                {
                    insert_index = mid;
                    break;
                }
                else
                {
                    s = mid;

                    if (s == e - 1)
                    {
                        if (_position < index[s].index)
                        {
                            insert_index = s;
                            break;
                        }
                        else
                        {
                            if (e < Block_count)
                            {
                                if (_position < index[e].index)
                                {
                                    insert_index = e;
                                    break;
                                }
                                else
                                {
                                    insert_index = e + 1;
                                    break;
                                }
                            }
                            else
                            {
                                insert_index = e;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
