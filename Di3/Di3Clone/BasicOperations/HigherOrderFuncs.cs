using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System.IO;
using System.Collections;
using Interfaces;

namespace DI3
{
    internal class HigherOrderFuncs<C, I, M, O>
        where C : IComparable<C>
        where I : IInterval<C, M>
        where M : IMetaData<C>
    {
        internal HigherOrderFuncs(BPlusTree<C, B<C, M>> di3)
        {
            this.di3 = di3;
            intervalsKeys = new Hashtable();
            lambdas = new List<Lambda<C, M>>();
        }

        private BPlusTree<C, B<C, M>> di3 { set; get; }

        private Hashtable intervalsKeys { set; get; }
        private List<Lambda<C, M>> lambdas { set; get; }

        internal List<O> Cover(ICSOutput<C, I, M, O> OutputStrategy, byte minAcc, byte maxAcc)
        {
            C markedKey = default(C);
            int markedAcc = -1;
            byte accumulation = 0;
            lambdas.Clear();
            intervalsKeys.Clear();

            foreach (var block in di3.EnumerateFrom(di3.First().Key))
            {
                accumulation = (byte)(block.Value.lambda.Count - block.Value.omega);

                if (markedAcc == -1 &&
                    accumulation >= minAcc &&
                    accumulation <= maxAcc)
                {
                    markedKey = block.Key;
                    markedAcc = accumulation;
                    UpdateLambdas(block.Value.lambda);
                }
                else if (markedAcc != -1)
                {
                    if (accumulation < minAcc ||
                        accumulation > maxAcc)
                    {
                        UpdateLambdas(block.Value.lambda);
                        //OutputStrategy.Output(di3[markedKey].e, di3[block.Key].e, lambdas);
                        OutputStrategy.Output(markedKey, block.Key, lambdas);

                        markedKey = default(C);
                        markedAcc = -1;
                        lambdas.Clear();
                        intervalsKeys.Clear();
                    }
                    else if (accumulation >= minAcc &&
                        accumulation <= maxAcc)
                    {
                        UpdateLambdas(block.Value.lambda);
                    }
                }
            }

            return OutputStrategy.output;
        }

        internal List<O> Summit(ICSOutput<C, I, M, O> OutputStrategy, byte minAcc, byte maxAcc)
        {
            C markedKey = default(C);
            int markedAcc = -1;
            byte accumulation = 0;
            lambdas.Clear();
            intervalsKeys.Clear();

            foreach (var block in di3.EnumerateFrom(di3.First().Key))
            {
                accumulation = (byte)(block.Value.lambda.Count - block.Value.omega);

                if (markedAcc < accumulation &&
                    accumulation >= minAcc &&
                    accumulation <= maxAcc)
                {
                    markedKey = block.Key;
                    markedAcc = accumulation;
                    UpdateLambdas(block.Value.lambda);
                }
                else if (markedAcc > accumulation ||
                    (markedAcc < accumulation && (
                    accumulation < minAcc ||
                    accumulation > maxAcc) &&
                    markedAcc != -1))
                {
                    UpdateLambdas(block.Value.lambda);
                    //OutputStrategy.Output(di3[markedKey].e, di3[block.Key].e, lambdas);
                    OutputStrategy.Output(markedKey, block.Key, lambdas);

                    markedKey = default(C);
                    markedAcc = -1;
                    lambdas.Clear();
                    intervalsKeys.Clear();
                }
                else if (accumulation >= minAcc &&
                    accumulation <= maxAcc &&
                    markedAcc != -1)
                {
                    UpdateLambdas(block.Value.lambda);
                }
            }

            return OutputStrategy.output;
        }

        internal List<O> Map(ICSOutput<C, I, M, O> OutputStrategy, List<I> references)
        {
            int i = 0;
            foreach (var reference in references)
            {
                Console.Write("\r ... processing regions: {0} / {1}", ++i, references.Count);
                lambdas.Clear();
                intervalsKeys.Clear();

                #region .::.     a quick note     .::.
                /// This iteration starts from a block which it's key (i.e., coordinate)
                /// is the minimum >= to reference.left; and goes to the block which the key
                /// is maximum <= to reference.right. Of course if no such blocks are available
                /// this iteration wont iteratre over anything. 
                #endregion
                foreach (var block in di3.EnumerateRange(reference.left, reference.right))
                {
                    UpdateLambdas(block.Value.lambda);
                }

                OutputStrategy.Output(reference, lambdas);
            }

            return OutputStrategy.output;
        }

        private void UpdateLambdas(List<Lambda<C, M>> newLambdas)
        {
            foreach (var item in newLambdas)
            {
                if (//item.tau != 'R' &&
                    !intervalsKeys.ContainsKey(item.atI.hashKey))
                {
                    lambdas.Add(item);
                    intervalsKeys.Add(item.atI.hashKey, "Hmd");
                }
            }
        }
    }
}
