﻿using System;
using SM;
using System.Diagnostics;
using System.Threading.Tasks;

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
            int generations = 1;
            float crossover = 0.5f;
            int stagnation = 25;

            var ga = new BlackjackGA();
            ga.SetupGA(population, population, crossover, stagnation);

            Stopwatch timer = new Stopwatch();
            timer.Start();

            float[] fitness = new float[population];
            for (int generation = 1; generation <= generations; generation++)
            {
                Parallel.For(0, games, j =>
                {
                    Deck deck = new Deck(4);
                    deck.Shuffle();
                    float[] fitnessIncrement = new float[population];
                    for (int i = 0; i < population; i++) // games
                    {
                        Blackjack game = new Blackjack(deck, (info) => BlackjackMove.Split);
                        fitnessIncrement[i] += game.Play();
                    }

                    lock (fitness) // thread-safe
                    {
                        for (int i = 0; i < population; i++)
                            fitness[i] += fitnessIncrement[i];
                    }
                });

                Console.WriteLine($"Generation: {generation}");
            }
            timer.Stop();
            Console.WriteLine("ms ellapsed: " + timer.ElapsedMilliseconds);
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
