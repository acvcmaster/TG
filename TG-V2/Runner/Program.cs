using System;
using SM;
using System.Diagnostics;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            Deck deck = new Deck(4);
            for (int j = 0; j < 100000; j++) // games
            {
                deck.Shuffle();
                for (int i = 0; i < 250; i++) // individuals
                {
                    Blackjack game = new Blackjack(deck, (game) => BlackjackMove.Stand);
                    game.Play();
                }
            }

            timer.Stop();
            Console.WriteLine("ms ellapsed: " + timer.ElapsedMilliseconds);
        }
    }
}
