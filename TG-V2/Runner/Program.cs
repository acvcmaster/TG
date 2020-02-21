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
            for (int i = 0; i < 100000; i++)
            {
                Blackjack game = new Blackjack(deck);
                game.Play();
            }

            timer.Stop();
            Console.WriteLine("ms ellapsed: " + timer.ElapsedMilliseconds);
        }
    }
}
