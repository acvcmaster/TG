using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace Runner
{
    public class BlackjackFitness : IFitness
    {
        public BlackjackFitness(BlackjackPopulation population)
        {
            this.Population = population;
        }

        public BlackjackPopulation Population { get; }

        public double Evaluate(IChromosome chromosome)
        {
            int hash = Population.GetChromosomeHash(chromosome);
            return Population.GenerationFitness[hash];
        }
    }
}
