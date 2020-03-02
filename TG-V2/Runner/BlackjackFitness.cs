using System.Threading.Tasks;
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
            object fitness_mutex = new object();
            double fitness = 0;
            for (int i = 0; i < Games; i++)
            {
                Blackjack game = new Blackjack(RandomDecks.PickRandom(), (game) => BlackjackMove.Stand);
                fitness += game.Play();
            }
            return fitness;
        }
    }
}