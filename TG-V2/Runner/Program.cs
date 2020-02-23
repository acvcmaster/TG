// #define INTERACTIVE

using System;
using SM;
using System.Diagnostics;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
#if INTERACTIVE
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

            Stopwatch timer = new Stopwatch();
            timer.Start();

            Deck deck = new Deck(4);
            float[] fitness = new float[population];
            for (int j = 0; j < games; j++) // games
            {
                deck.Shuffle();
                for (int i = 0; i < population; i++) // individuals
                {
                    Blackjack game = new Blackjack(deck, (game) => BlackjackMove.Stand);
                    game.Play();
                }
            }

            timer.Stop();
            Console.WriteLine("ms ellapsed: " + timer.ElapsedMilliseconds);
#endif


        }

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
    }
}
