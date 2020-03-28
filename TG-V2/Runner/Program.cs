using System;
using SM;
using GeneticSharp.Domain;
using System.Diagnostics;
using GeneticSharp.Domain.Chromosomes;
using Util;
using System.Linq;

namespace Runner
{
    class Program
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
                var best = algorithm.BestChromosome as FloatingPointChromosome;
                var values = best.ToFloatingPoints();
                Console.WriteLine($"Generation {algorithm.GenerationsNumber} ended (took {millis} ms)!");
                Console.WriteLine($"Best fitness = {best.Fitness}");
                Console.WriteLine($"Best chromosome = ({values.Print()})");
                Console.Write("Generating diagram.. ");
                var diagram = Diagrammer.Generate(values);
                Diagrammer.Save(diagram, $"Diagrams/Generation_{algorithm.GenerationsNumber}.png");
                Console.WriteLine($"Done (saved to 'Diagrams/Generation_{algorithm.GenerationsNumber}.png').");
                timer.Reset();
                timer.Start();
            };
            Console.WriteLine("Algorithm started.");
            timer.Start();
            ga.Start();
        }
    }
}
