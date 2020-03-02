using GeneticSharp.Domain;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Infrastructure.Framework.Threading;

namespace Runner
{
    public static class BlackjackGA
    {
        public static GeneticAlgorithm SetupGA(int maxPop, int games, float crossProbability, int stagGenNumber, int degreeOfParallelism)
        {
            //TODO: ver quantas variáveis irão existir depois
            var chromosome = new FloatingPointChromosome(
                new double[] { -10, -10, -10, -10 },
                new double[] { 10, 10, 10, 10 },
                new int[] { 64, 64, 64, 64 },
                new int[] { 0, 0, 0, 0 });

            //TODO: implementar
            var population = new Population(maxPop, maxPop, chromosome);

            //TODO: implementar
            var fitness = new BlackjackFitness(games);

            var selection = new TournamentSelection();

            //TODO: ver se vai testar outro método
            var crossover = new UniformCrossover(crossProbability);

#warning Mutação no caso não é bom, então faremos 0% de mutação, Antonio: Nada a ver
            var mutation = new FlipBitMutation();

            var termination = new FitnessStagnationTermination(stagGenNumber);

            var ga = new GeneticAlgorithm(
                population,
                fitness,
                selection,
                crossover,
                mutation);

            ga.Termination = termination;
            ga.TaskExecutor = new ParallelTaskExecutor() { MinThreads = degreeOfParallelism, MaxThreads = degreeOfParallelism };
            return ga;
        }
    }
}
