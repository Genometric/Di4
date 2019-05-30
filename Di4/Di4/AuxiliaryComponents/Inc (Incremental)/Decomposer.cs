using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Polimi.DEIB.VahidJalili.DI4.AuxiliaryComponents.Inc
{
    internal class Decomposer<C, I, M, O>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        public Decomposer(IOutput<C, I, M, O> outputStrategy, object lockOnMe)
        {
            _outputStrategy = outputStrategy;
            _lockOnMe = lockOnMe;
            _decomposedIntervals = new AtomicDictionary<uint, DecomposerValue<C>>();
            _designatedRegions = new List<DesignatedRegionNEW<C>>();
            _tLambdas = new HashSet<uint>();
            _regionType = RegionType.Candidate;
        }

        private AtomicDictionary<uint, DecomposerValue<C>> _decomposedIntervals { set; get; }
        private List<DesignatedRegionNEW<C>> _designatedRegions { set; get; }

        private int _mu { set; get; }

        private object _lockOnMe { set; get; }
        private IOutput<C, I, M, O> _outputStrategy { set; get; }
        private HashSet<uint> _tLambdas { set; get; }
        private RegionType _regionType { set; get; }
        //private int _tDecomposedIntervalsCount { set; get; }
        private bool _updateMu { set; get; }
        


        public void Reset()
        {
            _mu = 0;
            _updateMu = true;
            _tLambdas.Clear();
            _decomposedIntervals.Clear();
            _designatedRegions.Clear();
        }
        public void Open(C leftEnd, DI4.Inc.B keyBookmark)
        {
            //_tDecomposedIntervalsCount = _decomposedIntervals.Count;
            _regionType = RegionType.Designated;
            _designatedRegions.Add(new DesignatedRegionNEW<C>(left: leftEnd, right: leftEnd));

            foreach (var lambda in keyBookmark.lambda)
            {
                if (lambda.phi == Phi.LeftEnd)
                {
                    /// Note:
                    /// If there exist any two (or more) intervals with
                    /// same hash key, it is possible that this call throws
                    /// exception. However, the modification I made on 
                    /// atomic dictionary would not throw an exception, 
                    /// rather replaces old value with the given value.
                    /// Logically, this is wrong! but garbage in, garbag 
                    /// out. If this behavior is not intended, the hash
                    /// keys must be the true unique identifier.
                    _decomposedIntervals.Add(
                        lambda.atI,
                        new DecomposerValue<C>(
                            leftDesignatedRegion: _designatedRegions.Count - 1,
                            rightDesignatedRegion: _designatedRegions.Count - 1));
                }
                else
                {
                    if (!_tLambdas.Remove(lambda.atI) && _designatedRegions.Count > 1)
                    {
                        if (_decomposedIntervals.AddOrUpdate(lambda.atI,
                            new DecomposerValue<C>(
                                leftDesignatedRegion: 0,
                                rightDesignatedRegion: _designatedRegions.Count - 2),
                            RightEndUpdateFunction))
                            _mu--;
                    }
                }
            }

            foreach (var atI in _tLambdas)
            {
                _decomposedIntervals.Add(atI,
                    new DecomposerValue<C>(
                        leftDesignatedRegion: _designatedRegions.Count - 1,
                        rightDesignatedRegion: _designatedRegions.Count - 1));
            }

            if (_updateMu)
            {
                _mu = keyBookmark.mu - _tLambdas.Count;// - _tDecomposedIntervalsCount;
                _updateMu = false;
            }

            _tLambdas.Clear();
        }
        public void Close(C rightEnd, DI4.Inc.B keyBookmark)
        {
            _designatedRegions[_designatedRegions.Count - 1] = _designatedRegions[_designatedRegions.Count - 1].UpdateRight(rightEnd);
            foreach (var lambda in keyBookmark.lambda)
            {
                if (lambda.phi == Phi.LeftEnd)
                    _tLambdas.Add(lambda.atI);
                else
                {
                    if (_decomposedIntervals.AddOrUpdate(lambda.atI,
                            new DecomposerValue<C>(
                                leftDesignatedRegion: 0,
                                rightDesignatedRegion: _designatedRegions.Count - 1),
                            RightEndUpdateFunction))
                        _mu--;
                }
            }

            _regionType = RegionType.Decomposition;

            if (_mu == 0)
                Conclude();
        }
        public void Update(DI4.Inc.B keyBookmark)
        {
            switch (_regionType)
            {
                case RegionType.Candidate:
                    foreach (var lambda in keyBookmark.lambda)
                        if (lambda.phi == Phi.LeftEnd)
                            _tLambdas.Add(lambda.atI);
                        else
                            _tLambdas.Remove(lambda.atI);
                    return;

                case RegionType.Designated:
                    foreach (var lambda in keyBookmark.lambda)
                    {
                        if (lambda.phi == Phi.LeftEnd)
                        {
                            /// Note:
                            /// If there exist any two (or more) intervals with
                            /// same hash key, it is possible that this call throws
                            /// exception. However, the modification I made on 
                            /// atomic dictionary would not throw an exception, 
                            /// rather replaces old value with the given value.
                            /// Logically, this is wrong! but garbage in, garbag 
                            /// out. If this behavior is not intended, the hash
                            /// keys must be the true unique identifier.
                            _decomposedIntervals.Add(lambda.atI,
                                new DecomposerValue<C>(
                                    leftDesignatedRegion: _designatedRegions.Count - 1,
                                    rightDesignatedRegion: _designatedRegions.Count - 1));
                        }
                        else
                        {
                            if (_decomposedIntervals.AddOrUpdate(lambda.atI,
                                new DecomposerValue<C>(
                                    leftDesignatedRegion: _designatedRegions.Count - 1,
                                    rightDesignatedRegion: _designatedRegions.Count - 1),
                                RightEndUpdateFunction))
                                _mu--;
                        }
                    }
                    break;

                case RegionType.Decomposition:
                    foreach (var lambda in keyBookmark.lambda)
                    {
                        if (lambda.phi == Phi.LeftEnd)
                            _tLambdas.Add(lambda.atI);
                        else if (!_tLambdas.Remove(lambda.atI))
                        {
                            if (_decomposedIntervals.AddOrUpdate(lambda.atI,
                                new DecomposerValue<C>(
                                    leftDesignatedRegion: 0,
                                    rightDesignatedRegion: _designatedRegions.Count - 1),
                                RightEndUpdateFunction))
                                _mu--;
                        }
                    }
                    break;
            }
        }
        private void Conclude()
        {
            int i;
            var determinedLambdas = new List<List<uint>>();
            for (i = 0; i < _designatedRegions.Count; i++)
                determinedLambdas.Add(new List<uint>());
            
            foreach(var item in _decomposedIntervals)            
                for (i = item.Value.leftDesignatedRegion; i <= item.Value.rightDesignatedRegion; i++)
                    determinedLambdas[i].Add(item.Key);

            for (i = 0; i < _designatedRegions.Count; i++)
                _outputStrategy.Output(
                    left: _designatedRegions[i].left,
                    right: _designatedRegions[i].right,
                    intervals: determinedLambdas[i],
                    lockOnMe: _lockOnMe);

            _designatedRegions.Clear();

            foreach (var item in _decomposedIntervals.Where(kvp => kvp.Value.lastObservedEnd ==  Phi.RightEnd).ToList())
                _decomposedIntervals.Remove(item.Key);
        }



        public bool CanConclude()
        {
            if (_mu != 0) return false;
            if (_designatedRegions.Count > 0)
                Conclude();
            return true;
        }

        private DecomposerValue<C> RightEndUpdateFunction(DecomposerValue<C> value)
        {
            return value.UpdateRightEnd(_designatedRegions.Count - 1);
        }
    }
}
