﻿using System;
using System.Collections.Generic;
using Interfaces;
using CSharpTest.Net.Serialization;
using Di3B.Logging;


namespace Di3B
{
    public class Di3B<C, I, M>
        where C : IComparable<C>
        where I : IInterval<C, M>, new()
        where M : IMetaData, new()
    {        
        public Di3B(string workingDirectory, Memory Memory, HDDPerformance hddPerformance, ISerializer<C> CSerializer, IComparer<C> CComparer)
        {
            this.CSerializer = CSerializer;
            this.CComparer = CComparer;

            genome = new Genome<C, I, M>(workingDirectory, Memory, hddPerformance, CSerializer, CComparer);
        }

        private ISerializer<C> CSerializer { set; get; }
        private IComparer<C> CComparer { set; get; }
        private Genome<C, I, M> genome { set; get; }

        public ExecutionReport Add(Dictionary<string, Dictionary<char, List<I>>> peaks)
        {
            return genome.Add(peaks, '*');
        }
        public ExecutionReport Cover(CoverVariation coverVariation, char strand, byte minAcc, byte maxAcc, Aggregate aggregate, out FunctionOutput<Output<C, I, M>> result)
        {
            return genome.Cover(coverVariation, strand, minAcc, maxAcc, aggregate, out result);
        }
        public ExecutionReport Map(char strand, Dictionary<string, Dictionary<char, List<I>>> references, Aggregate aggregate, out FunctionOutput<Output<C, I, M>> result)
        {
            return genome.Map(references, strand, aggregate, out result);
        }
    }
}
