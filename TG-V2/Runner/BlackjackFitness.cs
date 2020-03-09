using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using SM;
using System.Linq;

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
            var values = (chromosome as FloatingPointChromosome).ToFloatingPoints();
            var sum = values.Sum();
            double fitness = 0;
            for (int i = 0; i < Games; i++)
            {
                Blackjack game = new Blackjack(RandomDecks.PickRandom(), (game) => (game.PlayerSum > sum ? BlackjackMove.Stand : BlackjackMove.Hit));
                fitness += game.Play();
            }
            return fitness;
        }
    }
}