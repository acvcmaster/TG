using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;

namespace Runner
{
    public class BlackjackGA
    {
        public GeneticAlgorithm SetupGA(int minPop, int maxPop, float crossProbability, int stagGenNumber)
        {
            //TODO: ver quantas variáveis irão existir depois
            var chromosome = new FloatingPointChromosome(
                new double[] { -10, -10, -10, -10 },
                new double[] { 10, 10, 10, 10 },
                new int[] { 5, 5, 5, 5 },
                new int[] { 0, 0, 0, 0 });

            //TODO: implementar
            var population = new BlackjackPopulation(minPop, maxPop, chromosome);

            //TODO: implementar
            var fitness = new BlackjackFitness(population);

            var selection = new TournamentSelection();

            //TODO: ver se vai testar outro método
            var crossover = new UniformCrossover(crossProbability);

            //Mutação no caso não é bom, então faremos 0% de mutação
            var mutation = new BlackjackMutation();

            var termination = new FitnessStagnationTermination(stagGenNumber);

            var ga = new GeneticAlgorithm(
                population,
                fitness,
                selection,
                crossover,
                mutation);

            ga.Termination = termination;

            return ga;
        }
    }
}
