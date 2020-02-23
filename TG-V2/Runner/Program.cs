using System;
using SM;
using System.Diagnostics;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
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
        }
    }
}
