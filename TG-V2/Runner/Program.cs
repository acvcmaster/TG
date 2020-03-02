using System;
using SM;
using GeneticSharp.Domain;
using System.Diagnostics;

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
            int stagnation = 25;
            int parallelism = 16;

            Console.Write("Setting up.. ");
            var ga = BlackjackGA.SetupGA(population, games, crossover, stagnation, parallelism);
            RandomDecks.GenerateRandomDecks();
            Stopwatch timer = new Stopwatch();
            Console.WriteLine("Setup done.");
            ga.GenerationRan += (gen, ev) =>
            {
                timer.Stop();
                var millis = timer.ElapsedMilliseconds;
                var algorithm = gen as GeneticAlgorithm;
                Console.WriteLine($"Generation {algorithm.GenerationsNumber} ended (took {millis} ms)!");
                timer.Reset();
                timer.Start();
            };
            Console.WriteLine("Algorithm started.");
            timer.Start();
            ga.Start();
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
