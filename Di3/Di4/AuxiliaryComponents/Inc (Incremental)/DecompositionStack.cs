using Polimi.DEIB.VahidJalili.DI4;
using Polimi.DEIB.VahidJalili.IGenomics;
using System;
using System.Collections.Generic;

namespace Polimi.DEIB.VahidJalili.DI4.AuxiliaryComponents.Inc
{
    internal class DecompositionStack<C, I, M, O>
        where C : IComparable<C>, IFormattable
        where I : IInterval<C, M>
        where M : IMetaData, new()
    {
        public DecompositionStack(IOutput<C, I, M, O> outputStrategy, object lockOnMe)
        {
            _lockOnMe = lockOnMe;
            _outputStrategy = outputStrategy;
            _designatedRegions = new List<DesignatedRegion<C>>();
            _tLambdas = new Dictionary<uint, Phi>();
        }

        private object _lockOnMe { set; get; }
        private IOutput<C, I, M, O> _outputStrategy { set; get; }
        private List<DesignatedRegion<C>> _designatedRegions { set; get; }
        private Dictionary<uint, Phi> _tLambdas { set; get; }
        private int _dUpperLimit { set; get; }
        private int _i { set; get; }

        public void Reset()
        {
            _designatedRegions.Clear();
            _tLambdas.Clear();
        }
        public void OpenDesignatedRegion(C leftEnd, DI4.Inc.B keyBookmark)
        {
            var newDesignatedRegion = new DesignatedRegion<C>();
            newDesignatedRegion.leftEnd = leftEnd;
            newDesignatedRegion.mu = keyBookmark.mu - _tLambdas.Count;

            foreach (var lambda in _tLambdas)
                newDesignatedRegion.lambdas.Add(lambda.Key, Phi.LeftEnd);
            _tLambdas.Clear();

            foreach (var lambda in keyBookmark.lambda)
                if (lambda.phi == Phi.LeftEnd)
                    newDesignatedRegion.lambdas.Add(lambda.atI, Phi.LeftEnd);
                else
                    UpdateDesignatedRegions(lambda.atI);

            if (_designatedRegions.Count > 0)
            {
                foreach (var lambda in _designatedRegions[_designatedRegions.Count - 1].lambdas)
                    if (lambda.Value == Phi.LeftEnd)
                    {
                        newDesignatedRegion.lambdas.Add(lambda.Key, Phi.LeftEnd);
                        newDesignatedRegion.mu--;
                    }
                Conclude();
            }
            
            _designatedRegions.Add(newDesignatedRegion);
        }
        public void CloseDesignatedRegion(C rightEnd, DI4.Inc.B keyBookmark)
        {
            _designatedRegions[_designatedRegions.Count - 1].rightEnd = rightEnd;
            foreach (var lambda in keyBookmark.lambda)
                if (lambda.phi == Phi.LeftEnd)
                    _tLambdas.Add(lambda.atI, Phi.LeftEnd);
                else
                    UpdateDesignatedRegions(lambda.atI);
        }
        public void Update(DI4.Inc.B keyBookmark, RegionType regionType)
        {
            switch (regionType)
            {
                case RegionType.Candidate: break;

                case RegionType.Designated:
                    foreach (var lambda in keyBookmark.lambda)
                        if (lambda.phi == Phi.LeftEnd)
                            _designatedRegions[_designatedRegions.Count - 1].lambdas.Add(lambda.atI, Phi.LeftEnd);
                        else
                            UpdateDesignatedRegions(lambda.atI);
                    break;

                case RegionType.Decomposition:
                    foreach (var lambda in keyBookmark.lambda)
                        if (lambda.phi == Phi.LeftEnd)
                            _tLambdas.Add(lambda.atI, Phi.LeftEnd);
                        else if (!_tLambdas.Remove(lambda.atI))
                            UpdateDesignatedRegions(lambda.atI);
                    break;
            }
        }
        private void UpdateDesignatedRegions(uint atI)
        {
            for (_i = _designatedRegions.Count - 1; _i >= 0; _i--)
                if (_designatedRegions[_i].lambdas.ContainsKey(atI))
                    _designatedRegions[_i].lambdas[atI] = Phi.RightEnd;
                else
                {
                    _designatedRegions[_i].lambdas.Add(atI, Phi.RightEnd);
                    _designatedRegions[_i].mu--;
                }
        }
        public void Conclude()
        {
            _dUpperLimit = _designatedRegions.Count;
            for (int i = 0; i < _dUpperLimit; i++)
                if (_designatedRegions[i].mu == 0)
                {
                    _outputStrategy.Output(_designatedRegions[i].leftEnd, _designatedRegions[i].rightEnd, new List<uint>(_designatedRegions[i].lambdas.Keys), _lockOnMe);
                    _designatedRegions.RemoveAt(i);
                    _dUpperLimit--;
                    i--;
                    continue;
                }
                else
                    return;
        }
    }
}
