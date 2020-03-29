using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using SM;
using Util;

namespace Runner
{
    public class BlackjackFitness : IFitness
    {
        public BlackjackFitness(int games)
        {
            Games = games;
        }

        public int Games { get; }

        public double Evaluate(IChromosome chromosome)
        {
            var moves = (chromosome as BlackjackChromosome).Moves;

            double fitness = 0;
            for (int i = 0; i < Games; i++)
            {
                Blackjack game = new Blackjack(RandomDecks.PickRandom(), (info) => StrategyMapper.GetMove(info, moves));
                fitness += game.Play();
            }
            return fitness;
        }
    }
}