using System;
using System.Threading.Tasks;
using Plotly.NET;
using TG_V3.Blackjack;

namespace TG_V3
{
    partial class Learning
    {
        private static void EstimateWinrate(double[,,] QHardHands, double[,,] QSoftHands, double[,,] QSplit, int maxGames)
        {
            var mutex = new object();

            var playerScore = 0.0;
            var dealerScore = 0.0;
            var draws = 0.0;
            var totalRewards = 0.0;

            Parallel.For(0, maxGames, i => // Nesse caso, como não atualizamos a tabela Q, podemos jogar os jogos em paralelo
            {
                Deck deck = new Deck(4);
                Game game = new Game(ref deck);

                var reward = PlayGame(game, ref deck, QHardHands, QSoftHands, QSplit);

                lock (mutex)
                {
                    if (reward == 0)
                        draws++;
                    else
                    {
                        if (reward > 0)
                            playerScore += reward;
                        else
                            dealerScore -= reward;
                    }

                    totalRewards += reward;
                }
            });

            Console.WriteLine($"Player score: {playerScore}");
            Console.WriteLine($"Dealer score: {dealerScore}");
            Console.WriteLine($"Draws: {draws}");
            Console.WriteLine($"Normalized reward: {totalRewards / maxGames}");

            // double[] x = new double[] { 1, 2 };
            // double[] y = new double[] { 5, 10 };
            // GenericChart.GenericChart chart = Chart.Line<double, double, double>(x: x, y: y);
            // chart
            //     .WithTraceName("Hello from C#", true)
            //     .Show();
        }
    }
}