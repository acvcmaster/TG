using System;
using SM;
using GeneticSharp.Domain;
using System.Diagnostics;
using GeneticSharp.Domain.Chromosomes;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            // Deck deck = new Deck(4);
            // for (; ; )
            // {
            //     Blackjack game = new Blackjack(deck, InteractiveStrategy);
            //     deck.Shuffle();
            //     game.Play();
            // }
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
                Console.WriteLine($"Best value = {best.ToFloatingPoints()[0]}");
                Console.WriteLine($"Best fitness = {best.Fitness}");
                // foreach (FloatingPointChromosome c in algorithm.Population.CurrentGeneration.Chromosomes)
                // {
                //     var vals = c.ToFloatingPoints();
                //     for (int i = 0; i < vals.Length; i++)
                //         Console.Write(vals[i] + " ");
                //     if (c.Equals(best))
                //     {
                //         Console.Write("*");
                //     }
                //     Console.WriteLine();
                // }
                timer.Reset();
                timer.Start();
            };
            Console.WriteLine("Algorithm started.");
            timer.Start();
            ga.Start();

            // for (int sum = 0; sum <= 21; sum++)
            // {
            //     double fitnesstotal = 0;
            //     int n = 10;
            //     for (int i = 0; i < n; i++)
            //     {
            //         double fitness = 0;
            //         for (int j = 0; j < 100000; j++)
            //         {
            //             Blackjack game = new Blackjack(RandomDecks.PickRandom(), (game) => (game.PlayerSum > sum ? BlackjackMove.Stand : BlackjackMove.Hit));
            //             fitness += game.Play();
            //         }
            //         fitnesstotal += fitness;
            //     }
            //     Console.WriteLine($"{sum} => {fitnesstotal/n}");
            // }
        }
        static BlackjackMove InteractiveStrategy(BlackjackInformation info)
        {
            Console.Write("Your cards: ");
            for (int i = 0; i < info.PlayerHandIndex; i++)
                Console.Write($"{info.PlayerHand[i].Name}{ (i + 1 != info.PlayerHandIndex ? ", " : "") }");
            Console.WriteLine();
            Console.WriteLine($"Dealer faceup card: {info.DealerFaceupCard.Name}");

            while (true)
            {
                Console.Write("Your move: ");
                BlackjackMove move;
                bool success = Enum.TryParse<BlackjackMove>(Console.ReadLine(), true, out move);
                if (success) { return move; }
            }
        }
    }
}
