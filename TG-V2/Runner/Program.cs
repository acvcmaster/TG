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

            // Deck deck = new Deck(4);
            // float[] fitness = new float[population];
            // for (int j = 0; j < games; j++) // games
            // {
            //     deck.Shuffle();
            //     for (int i = 0; i < population; i++) // individuals
            //     {
            //         Blackjack game = new Blackjack(deck, (game) => BlackjackMove.Stand);
            //         game.Play();
            //     }
            // }

            TestSums();
            timer.Stop();
            Console.WriteLine("ms ellapsed: " + timer.ElapsedMilliseconds);
        }

        static void TestSums()
        {
            for (int i = 0; i < 100000 * 250; i++)
            {
                Card[] cards = new Card[] {
                new Card(FaceValue.Ace, Suit.Hearts),
                new Card(FaceValue.Seven, Suit.Hearts),
                new Card(FaceValue.Ace, Suit.Spades)
                };

                var sums = Blackjack.GetSum(cards, 3);
            }
        }
    }
}
