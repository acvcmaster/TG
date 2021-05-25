using System;
using TG_V3.Blackjack;
using TG_V3.Extensions;
using TG_V3.Util;

namespace TG_V3
{
    class Program
    {
        static void Main(string[] args)
        {
            double learningRate = 0.7;
            double discountFactor = 0.1;
            double explorationFactor = 0.3; // tem que cair durante o aprendizado
            int maxEpisodes = 200000;

            double[,,,] Q = new double[3, 10, 16, 3]; // table, dealer card, sum, move

            // Moves
            // 0 - Stand
            // 1 - Hit
            // 2 - Doubledown

            for (int episode = 0; episode < maxEpisodes; episode++)
            {
                Deck deck = new Deck(4);
                Game game = new Game(deck);

                if (!game.Final)
                {
                    while (!game.Final)
                    {
                        int move = 0;
                        if (GlobalRandom.NextDouble() < explorationFactor)
                        {
                            move = GlobalRandom.Next(3);
                        }
                        else
                        {
                            // exploit
                        }
                    }
                }
            }
        }

        static int GetIndexes(Game game)
        {

        }
    }
}
