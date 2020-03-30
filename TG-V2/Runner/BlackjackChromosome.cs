using System;
using System.Linq;
using Util;
using Util.Models;
using GeneticSharp.Domain.Chromosomes;
using System.Diagnostics.CodeAnalysis;

namespace Runner
{
    public class BlackjackChromosome : IChromosome
    {
        public BlackjackMove[] Moves { get; set; }
        public double? Fitness { get; set; }

        public BlackjackChromosome(BlackjackMove[] gene = null)
        {
            if (gene != null)
                Moves = gene.Clone<BlackjackMove>();
            else
            {
                Moves = new BlackjackMove[340];
                for (int i = 0; i < 340; i++)
                    Moves[i] = StaticRandom.NextMove(i >= StrategyMapper.Tables[1]);
            }
        }

        public int Length => Moves != null ? Moves.Length : 0;

        public IChromosome Clone() => new BlackjackChromosome(Moves);

        public int CompareTo([AllowNull] IChromosome other)
        {
            throw new NotImplementedException();
        }

        public IChromosome CreateNew() => new BlackjackChromosome();

        public Gene GenerateGene(int geneIndex) => new Gene(Moves[geneIndex]);

        public Gene GetGene(int index) => new Gene(Moves[index]);

        public Gene[] GetGenes() => Moves.Select(item => new Gene(item)).ToArray();

        public void ReplaceGene(int index, Gene gene) => Moves[index] = (BlackjackMove)gene.Value;

        public void ReplaceGenes(int startIndex, Gene[] genes)
        {
            for (int i = startIndex; i < startIndex + genes.Length; i++)
                ReplaceGene(i, genes[i - startIndex]);
        }
        public void Resize(int newLength) => throw new InvalidOperationException();
    }
}