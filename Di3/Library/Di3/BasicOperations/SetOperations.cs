using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IInterval;
using System.Collections;
using ICPMD;


namespace DI3
{
    internal class SetOperations<C, M, O>// : Di3DataStructure<C, I, M>
        where C : IComparable<C>
        //where I : IInterval<C, M>
        where M : IMetaData<C>
    {
        internal SetOperations(List<B<C, M>> di3)
        {
            this.di3 = di3;
            intervalsKeys = new Hashtable();
        }

        //private ICSOutput<C, I, M, O> IOutput { set; get; }

        private List<B<C, M>> di3 { set; get; }

        private Hashtable intervalsKeys { set; get; }

        internal List<O> Cover(ICSOutput<C, M, O> OutputStrategy, byte minAcc, byte maxAcc)
        {
            // 0 : Marked block index
            // 1 : Marked block accumulation
            int[] markedRegion = new int[] { -1, -1 };

            byte accumulation = 0;
            int di3Cardinality = di3.Count;

            for (int b = 0; b < di3Cardinality; b++)
            {
                accumulation = (byte)(di3[b].lambda.Count - di3[b].omega);

                if (markedRegion[1] == -1 &&
                    accumulation >= minAcc &&
                    accumulation <= maxAcc)
                {
                    markedRegion[0] = b;
                    markedRegion[1] = accumulation;
                }
                else if (markedRegion[1] != -1 && (
                    accumulation < minAcc ||
                    accumulation > maxAcc))
                {
                    OutputStrategy.Output(di3[markedRegion[0]].e, di3[b].e, GetLambdaSet(markedRegion[0], b));

                    markedRegion[0] = -1;
                    markedRegion[1] = -1;
                }
            }

            return OutputStrategy.output;
        }

        internal List<O> Summit(ICSOutput<C, M, O> OutputStrategy, byte minAcc, byte maxAcc)
        {
            // 0 : Marked block index
            // 1 : Marked block accumulation
            int[] markedRegion = new int[] { -1, -1 };

            byte accumulation = 0;

            int di3Cardinality = di3.Count;

            for (int b = 0; b < di3Cardinality; b++)
            {
                accumulation = (byte)(di3[b].lambda.Count - di3[b].omega);

                if (markedRegion[1] < accumulation &&
                    accumulation >= minAcc &&
                    accumulation <= maxAcc)
                {
                    markedRegion[0] = b;
                    markedRegion[1] = accumulation;
                }
                else if (markedRegion[1] > accumulation ||
                    (markedRegion[1] < accumulation && (
                    accumulation < minAcc ||
                    accumulation > maxAcc) &&
                    markedRegion[1] != -1))
                {
                    OutputStrategy.Output(di3[markedRegion[0]].e, di3[b].e, GetLambdaSet(markedRegion[0], b));

                    markedRegion[0] = -1;
                    markedRegion[1] = -1;
                }
            }

            return OutputStrategy.output;
        }

        private List<Lambda<C, M>> GetLambdaSet(int left, int right)
        {
            List<Lambda<C, M>> rtv = new List<Lambda<C, M>>();

            intervalsKeys.Clear();

            for (int b = left; b <= right; b++)
            {
                foreach (var lItem in di3[b].lambda)
                {
                    if (lItem.tau != 'R' &&
                        intervalsKeys.ContainsKey(lItem.atI.GetHashKey()) == false)
                    {
                        rtv.Add(lItem);
                        intervalsKeys.Add(lItem.atI.GetHashKey(), "Hamed");
                    }
                }
            }

            return rtv;
        }
    }
}
