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
        public static GeneticAlgorithm SetupGA(int maxPop, int games, float crossProbability, float mutationProbability, int maxGenerations, int degreeOfParallelism)
        {
            var chromosome = new BlackjackChromosome();
            var population = new Population(maxPop, maxPop, chromosome);
            var fitness = new BlackjackFitness(games);
            var selection = new TournamentSelection();
            var crossover = new UniformCrossover(crossProbability);
            var mutation = new UniformMutation();
            var termination = new GenerationNumberTermination(maxGenerations);

            var ga = new GeneticAlgorithm(
                population,
                fitness,
                selection,
                crossover,
                mutation);

            ga.MutationProbability = mutationProbability;
            ga.Termination = termination;
            ga.TaskExecutor = new ParallelTaskExecutor() { MinThreads = degreeOfParallelism, MaxThreads = degreeOfParallelism };
            return ga;
        }
    }
}
