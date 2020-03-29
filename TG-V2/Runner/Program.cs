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
            var date = DateTime.Now.ToString("yyyy-MM-dd HHmmss");
            var ga = BlackjackGA.SetupGA();
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
                Console.Write($"Generating diagram for generation {algorithm.GenerationsNumber}.. ");
                Console.WriteLine("Done.");
                var diagram = Diagrammer.Generate(best);
                Diagrammer.Save(diagram, algorithm.GenerationsNumber, date);
                timer.Reset();
                timer.Start();
            };
            Console.WriteLine("Algorithm started.");
            timer.Start();
            ga.Start();
        }
    }
}
