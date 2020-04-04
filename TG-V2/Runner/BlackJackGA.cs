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
        public static GeneticAlgorithm SetupGA()
        {
            var chromosome = new BlackjackChromosome();
            var population = new Population(Global.Population, Global.Population, chromosome);
            var fitness = new BlackjackFitness();
            var selection = new TournamentSelection();
            var crossover = new UniformCrossover(Global.Crossover);
            var mutation = new UniformMutation(true);
            var termination = new GenerationNumberTermination(Global.MaxGenerations);

            var ga = new GeneticAlgorithm(
                population,
                fitness,
                selection,
                crossover,
                mutation);

            ga.MutationProbability = Global.Mutation;
            ga.Termination = termination;
            ga.TaskExecutor = new ParallelTaskExecutor() { MinThreads = Global.Parallelism, MaxThreads = Global.Parallelism };
            return ga;
        }
    }
}
