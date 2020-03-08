using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using SM;

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
            double fitness = 0;
            for (int i = 0; i < Games; i++)
            {
                Blackjack game = new Blackjack(RandomDecks.PickRandom(), (game) => (game.PlayerSum > values[0] ? BlackjackMove.Stand : BlackjackMove.Hit));
                fitness += game.Play();
            }
            return fitness;
        }
    }
}