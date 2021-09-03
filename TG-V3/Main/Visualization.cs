using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plotly.NET;
using TG_V3.Blackjack;
using TG_V3.Extensions;

namespace TG_V3
{
    partial class Visualization
    {
        public static void Main()
        {
            var MAX_GAMES = 100000; // jogos usados para estimar taxa de vitórias (por amotra)
            var SAMPLES = 50; // número de amostras

            // var baseline = Learning.GetBaselineModel();
            // EstimateWinrate(baseline, MAX_GAMES, SAMPLES);

            EstimateRandomWinrate(MAX_GAMES, SAMPLES);



            // QHardHands = GetOptimalPolicy(QHardHands, 10, 16, new int[] { 0, 1, 2 }),
            // QSoftHands = GetOptimalPolicy(QSoftHands, 10, 8, new int[] { 0, 1, 2 }),
            // QSplit = GetOptimalPolicy(QSplit, 10, 10, new int[] { 0, 1, 2, 3 })

            // double[] x = new double[] { 1, 2 };
            // double[] y = new double[] { 5, 10 };
            // GenericChart.GenericChart chart = Chart.Line<double, double, double>(x: x, y: y);
            // chart
            //     .WithTraceName("Hello from C#", true)
            //     .Show();
        }

        private static void EstimateWinrate(QLearningModel model, int maxGames, int samples)
        {
            var mutex = new object();
            var totalPlayerScores = new List<double>();
            var totalDealerScores = new List<double>();
            var totalDraws = new List<double>();
            var totalRewards = new List<double>();

            Parallel.For(0, samples, j =>
            {
                var playerScore = 0.0;
                var dealerScore = 0.0;
                var draws = 0.0;
                var rewards = 0.0;

                for (var i = 0; i < maxGames; i++)// Nesse caso, como não atualizamos a tabela Q, podemos jogar os jogos em paralelo
                {
                    Deck deck = new Deck(4);
                    Game game = new Game(ref deck);

                    var reward = Learning.PlayGame(game, ref deck, model);

                    // atualizar pontuação (thread safe)
                    if (reward == 0)
                        draws++;
                    else
                    {
                        if (reward > 0)
                            playerScore += reward;
                        else
                            dealerScore -= reward;
                    }

                    rewards += reward;
                }

                lock (mutex)
                {
                    totalPlayerScores.Add(playerScore);
                    totalDealerScores.Add(dealerScore);
                    totalDraws.Add(draws);
                    totalRewards.Add(rewards / maxGames);
                }
            });

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"Calculando estatísticas para modelo: {model.Name}");
            Console.WriteLine($"\tPontuação do jogador: {totalPlayerScores.Average()} ± {totalPlayerScores.Sterr()}");
            Console.WriteLine($"\tPontuação do dealer: {totalDealerScores.Average()} ± {totalDealerScores.Sterr()}");
            Console.WriteLine($"\tEmpates: {totalDraws.Average()} ± {totalDraws.Sterr()}");
            Console.WriteLine($"\tRecompensa normalizada: {totalRewards.Average()} ± {totalRewards.Sterr()}");
            Console.Write("--------------------------------------------");
        }

        private static void EstimateRandomWinrate(int maxGames, int samples)
        {
            var mutex = new object();
            var totalPlayerScores = new List<double>();
            var totalDealerScores = new List<double>();
            var totalDraws = new List<double>();
            var totalRewards = new List<double>();

            Parallel.For(0, samples, j =>
            {
                var model = Learning.GetRandomModel();
                var playerScore = 0.0;
                var dealerScore = 0.0;
                var draws = 0.0;
                var rewards = 0.0;

                for (var i = 0; i < maxGames; i++)// Nesse caso, como não atualizamos a tabela Q, podemos jogar os jogos em paralelo
                {
                    Deck deck = new Deck(4);
                    Game game = new Game(ref deck);

                    var reward = Learning.PlayGame(game, ref deck, model);

                    // atualizar pontuação (thread safe)
                    if (reward == 0)
                        draws++;
                    else
                    {
                        if (reward > 0)
                            playerScore += reward;
                        else
                            dealerScore -= reward;
                    }

                    rewards += reward;
                }

                lock (mutex)
                {
                    totalPlayerScores.Add(playerScore);
                    totalDealerScores.Add(dealerScore);
                    totalDraws.Add(draws);
                    totalRewards.Add(rewards / maxGames);
                }
            });

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"Calculando estatísticas para modelo: Aleatório");
            Console.WriteLine($"\tPontuação do jogador: {totalPlayerScores.Average()} ± {totalPlayerScores.Sterr()}");
            Console.WriteLine($"\tPontuação do dealer: {totalDealerScores.Average()} ± {totalDealerScores.Sterr()}");
            Console.WriteLine($"\tEmpates: {totalDraws.Average()} ± {totalDraws.Sterr()}");
            Console.WriteLine($"\tRecompensa normalizada: {totalRewards.Average()} ± {totalRewards.Sterr()}");
            Console.Write("--------------------------------------------");
        }
    }
}