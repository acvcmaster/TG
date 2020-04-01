using System;
using GeneticSharp.Domain;
using System.Diagnostics;
using Util;

namespace Runner
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Setting up.. ");
            var guid = GuidProvider.NewGuid();
            var ga = BlackjackGA.SetupGA();
            RandomDecks.GenerateRandomDecks();
            Stopwatch timer = new Stopwatch();
            Console.WriteLine($"Setup done.");
            Console.WriteLine($"The unique identifier for this run is {guid}.");
            
            double? bestFitness = double.NegativeInfinity;
            int bestFitnessGeneration = -1;
            ga.GenerationRan += (gen, ev) =>
            {
                timer.Stop();
                var millis = timer.ElapsedMilliseconds;
                var algorithm = gen as GeneticAlgorithm;
                var best = algorithm.BestChromosome as BlackjackChromosome;
                if (best.Fitness > bestFitness) {
                    bestFitness = best.Fitness;
                    bestFitnessGeneration = algorithm.GenerationsNumber;
                }
                Console.WriteLine($"Generation {algorithm.GenerationsNumber} ended (took {millis} ms)!");
                Console.WriteLine($"Best fitness = {best.Fitness}");
                Console.Write($"Generating diagram for generation {algorithm.GenerationsNumber}.. ");
                var diagram = Diagrammer.Generate(best, algorithm, bestFitness, bestFitnessGeneration);
                Diagrammer.Save(diagram, algorithm.GenerationsNumber, guid);
                Console.WriteLine("Done.");
                timer.Reset();
                timer.Start();
                // RandomDecks.GenerateRandomDecks();
            };
            Console.WriteLine("Algorithm started.");
            timer.Start();
            ga.Start();
        }
    }
}
