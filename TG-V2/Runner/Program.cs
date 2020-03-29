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
            var ga = BlackjackGA.SetupGA(Global.Population, Global.Games, Global.Crossover, Global.Mutation, Global.MaxGenerations, Global.Parallelism);
            RandomDecks.GenerateRandomDecks();
            Stopwatch timer = new Stopwatch();
            Console.WriteLine("Setup done.");
            ga.GenerationRan += (gen, ev) =>
            {
                timer.Stop();
                var millis = timer.ElapsedMilliseconds;
                var algorithm = gen as GeneticAlgorithm;
                var best = algorithm.BestChromosome as BlackjackChromosome;
                Console.WriteLine($"Generation {algorithm.GenerationsNumber} ended (took {millis} ms)!");
                Console.WriteLine($"Best fitness = {best.Fitness}");
                Console.Write("Generating diagram.. ");
                var diagram = Diagrammer.Generate(best);
                Console.WriteLine("Done.");
                Diagrammer.Save(diagram, $"Diagrams/{algorithm.GenerationsNumber}.png");
                timer.Reset();
                timer.Start();
            };
            Console.WriteLine("Algorithm started.");
            timer.Start();
            ga.Start();
        }
    }
}
