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
            //TODO: ver quantas variáveis irão existir depois
            var chromosome = new FloatingPointChromosome(
                new double[] { 0 },
                new double[] { 21 },
                new int[] { 18 },
                new int[] { 4 });

            //TODO: implementar
            var population = new Population(maxPop, maxPop, chromosome);

            //TODO: implementar
            var fitness = new BlackjackFitness(games);

            var selection = new TournamentSelection();

            //TODO: ver se vai testar outro método
            var crossover = new UniformCrossover(crossProbability);

            var mutation = new FlipBitMutation();

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
