using GeneticSharp.Domain;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Infrastructure.Framework.Threading;
using Util;

namespace Runner
{
    public static class BlackjackGA
    {
        public static GeneticAlgorithm SetupGA(int populationSize, float mutationProbability, int episodes, float crossoverProbability, int maxGenerations, int parallelism = 16)
        {
            var chromosome = new BlackjackChromosome();
            var population = new Population(populationSize, populationSize, chromosome);
            var fitness = new BlackjackFitness(episodes);
            var selection = new TournamentSelection();
            var crossover = new UniformCrossover(crossoverProbability);
            var mutation = new UniformMutation(true);
            var termination = new GenerationNumberTermination(maxGenerations);

            var ga = new GeneticAlgorithm(
                population,
                fitness,
                selection,
                crossover,
                mutation);

            ga.MutationProbability = mutationProbability;
            ga.Termination = termination;
            ga.TaskExecutor = new ParallelTaskExecutor() { MinThreads = parallelism, MaxThreads = parallelism };
            return ga;
        }
    }
}
