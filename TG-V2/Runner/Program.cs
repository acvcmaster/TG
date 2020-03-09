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
            int population = 250;
            int games = 100000;
            float crossover = 0.5f;
            int maxGenerations = 100;
            float mutation = 0.2f;
            int parallelism = 16;

            Console.Write("Setting up.. ");
            var ga = BlackjackGA.SetupGA(population, games, crossover, mutation, maxGenerations, parallelism);
            RandomDecks.GenerateRandomDecks();
            Stopwatch timer = new Stopwatch();
            Console.WriteLine("Setup done.");
            ga.GenerationRan += (gen, ev) =>
            {
                timer.Stop();
                var millis = timer.ElapsedMilliseconds;
                var algorithm = gen as GeneticAlgorithm;
                var best = algorithm.BestChromosome as FloatingPointChromosome;
                Console.WriteLine($"Generation {algorithm.GenerationsNumber} ended (took {millis} ms)!");
                Console.WriteLine($"Best fitness = {best.Fitness}");
                Console.WriteLine($"Best chromosome = ({best.ToFloatingPoints().Print()})");
                Console.WriteLine($"Best phenotype = {best.ToFloatingPoints().Sum()}");
                timer.Reset();
                timer.Start();
            };
            Console.WriteLine("Algorithm started.");
            timer.Start();
            ga.Start();
        }
    }
}
