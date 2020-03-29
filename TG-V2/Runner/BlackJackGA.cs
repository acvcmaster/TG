using GeneticSharp.Domain;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Infrastructure.Framework.Threading;
using Util;
using System.Linq;

namespace Runner
{
    public static class BlackjackGA
    {
        public static GeneticAlgorithm SetupGA(int maxPop, int games, float crossProbability, float mutationProbability, int maxGenerations, int degreeOfParallelism)
        {
            var low = Enumerable.Repeat(Global.GeneticRangeLow, Global.WeightCount).ToArray();
            var high = Enumerable.Repeat(Global.GeneticRangeHigh, Global.WeightCount).ToArray();
            var bits = Enumerable.Repeat(Global.GenomeBits, Global.WeightCount).ToArray();
            var digits = Enumerable.Repeat(Global.FractionDigits, Global.WeightCount).ToArray();

            var chromosome = new FloatingPointChromosome(low, high, bits, digits);

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
