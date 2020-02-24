using System;
using SM;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            Deck deck = new Deck(4);
            for (; ; )
            {
                Blackjack game = new Blackjack(deck, InteractiveStrategy);
                deck.Shuffle();
                game.Play();
            }
#else
            int population = 250;
            int games = 100000;
            int generations = 1;

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
                        Blackjack game = new Blackjack(deck, (game) => BlackjackMove.Stand);
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
            // for (int i = 0; i < fitness.Length; i++)
            //     Console.Write(fitness[i] + ", ");
#endif
        }

#if DEBUG
        static BlackjackMove InteractiveStrategy(Blackjack game)
        {
            Console.Write("Your cards: ");
            for (int i = 0; i < game.PlayerHandIndex; i++)
                Console.Write($"{game.PlayerHand[i].Name}{ (i + 1 != game.PlayerHandIndex ? ", " : "") }");
            Console.WriteLine();
            Console.WriteLine($"Dealer faceup card: {game.DealerFaceupCard.Name}");

            while (true)
            {
                Console.Write("Your move: ");
                BlackjackMove move;
                bool success = Enum.TryParse<BlackjackMove>(Console.ReadLine(), true, out move);
                if (success) { return move; }
            }
        }
#endif
    }
}
