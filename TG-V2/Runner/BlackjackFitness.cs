using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace Runner
{
    public class BlackjackFitness : IFitness
    {
        public BlackjackFitness(BlackjackPopulation myPopulation)
        {
            MyPopulation = myPopulation;
        }

        public BlackjackPopulation MyPopulation { get; }

        public double Evaluate(IChromosome chromosome)
        {
            throw new System.NotImplementedException();
        }
    }
}
