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

            var ga = BlackjackGA.SetupGA(population, games, crossover, stagnation);
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
