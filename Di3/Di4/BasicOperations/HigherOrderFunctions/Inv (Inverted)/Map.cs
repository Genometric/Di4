using CSharpTest.Net.Collections;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Polimi.DEIB.VahidJalili.DI4.Inv
{
    internal class Map<C, I, M, O>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        internal Map(
            object lockOnMe,
            BPlusTree<C, B> di4_1R,
            IOutput<C, I, M, O> outputStrategy,
            List<I> intervals,
            int start,
            int stop)
        {
            _stop = stop;
            _start = start;
            _di4_1R = di4_1R;
            _lockOnMe = lockOnMe;
            _intervals = intervals;
            _lambdas = new HashSet<uint>();
            _outputStrategy = outputStrategy;
        }


        private BPlusTree<C, B> _di4_1R { set; get; }
        private BPlusTree<BlockKey<C>, BlockValue> _di4_2R { set; get; }
        private int _start { set; get; }
        private int _stop { set; get; }        
        private List<I> _intervals { set; get; }
        private IOutput<C, I, M, O> _outputStrategy { set; get; }
        internal IOutput<C, I, M, O> outputStrategy { get { return _outputStrategy; } }
        private HashSet<uint> _lambdas { set; get; }
        private object _lockOnMe { set; get; }


        internal void Run()
        {
            I reference;

            for (int i = _start; i < _stop; i++)
            {
                reference = _intervals[i];
                _lambdas.Clear();

                /// Note:
                /// This iteration starts from a bookmark which it's newKey (i.e., coordinate)
                /// is the minimum >= to reference.currentBlockLeftEnd; and goes to the bookmark which the newKey
                /// is maximum less-than-or-equal to reference.right. Of course if no such bookmarks are available
                /// this iteration wont iteratre over anything.               
                foreach (var bookmark in _di4_1R.EnumerateRange(reference.left, reference.right))
                    UpdateLambdas(bookmark.Value.lambda);

                _outputStrategy.Output(interval: reference, intervals: new List<uint>(_lambdas), lockOnMe: _lockOnMe);
            }
        }

        private void UpdateLambdas(ReadOnlyCollection<Lambda> lambdas)
        {
            foreach (var lambda in lambdas)
                if (!_lambdas.Contains(lambda.atI))
                    _lambdas.Add(lambda.atI);
        }
    }
}
