using Util;
using Util.Models;
using GeneticSharp.Domain.Chromosomes;

namespace Runner
{
    public class BlackjackChromosome : ChromosomeBase
    {
        public BlackjackMove[] Moves { get; }
        public BlackjackChromosome() : base(340)
        {
            Moves = new BlackjackMove[340];
            for (int i = 0; i < 340; i++)
                Moves[i] = StaticRandom.NextMove(i >= StrategyMapper.Tables[1]);
            CreateGenes();
        }

        public override IChromosome CreateNew() => new BlackjackChromosome();

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(Moves[geneIndex]);
        }
    }
}