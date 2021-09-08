#define HOUSE_EDGE

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG_V3.Blackjack;
using TG_V3.Extensions;

namespace TG_V3
{
    partial class Visualization
    {
        public static void Main()
        {

#if HOUSE_EDGE // Vantagem da casa

            using (var dados = new StreamWriter("Data/vantagem_casa.csv"))
            {
                var SAMPLES = 20; // número de amostras (e também de threads)
                var STEP = 10000;
                var MAX_STEPS = 10;
                var baseline = Learning.GetBaselineModel();

                for (var max_games = STEP; max_games <= MAX_STEPS * STEP; max_games += STEP)
                {
                    var baselineEstimate = EstimateWinrate(baseline, max_games, SAMPLES);
                    var normalizedRewards = baselineEstimate.NormalizedRewards;
                    var winPercentage = baselineEstimate.WinPercentage;

                    dados.WriteLine($"{max_games}, {winPercentage.Value}, {winPercentage.Uncertainty}, {winPercentage.CoefficientOfVariation}, {normalizedRewards.Value}, {normalizedRewards.Uncertainty}, {100 * normalizedRewards.CoefficientOfVariation}");
                }
            }
#endif

            // EstimateRandomWinrate(MAX_GAMES, SAMPLES);





            // QHardHands = GetOptimalPolicy(QHardHands, 10, 16, new int[] { 0, 1, 2 }),
            // QSoftHands = GetOptimalPolicy(QSoftHands, 10, 8, new int[] { 0, 1, 2 }),
            // QSplit = GetOptimalPolicy(QSplit, 10, 10, new int[] { 0, 1, 2, 3 })
        }

        private static WinrateEstimate EstimateWinrate(QLearningModel model, int maxGames, int samples)
        {
            var mutex = new object();
            var totalPlayerScores = new List<double>();
            var totalDealerScores = new List<double>();
            var totalDraws = new List<double>();
            var totalRewards = new List<double>();
            var totalWinrate = new List<double>();

            Parallel.For(0, samples, j =>
            {
                var playerScore = 0.0;
                var dealerScore = 0.0;
                var draws = 0.0;
                var rewards = 0.0;
                var wins = 0.0;

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
                        {
                            playerScore += reward;
                            wins++;
                        }
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
                    totalWinrate.Add(100 * wins / maxGames);
                }
            });

            return new WinrateEstimate
            {
                PlayerScore = new UncertainValue() { Value = totalPlayerScores.Average(), Uncertainty = totalPlayerScores.Sterr(), CoefficientOfVariation = totalPlayerScores.CoeffVar() },
                DealerScore = new UncertainValue() { Value = totalDealerScores.Average(), Uncertainty = totalDealerScores.Sterr(), CoefficientOfVariation = totalPlayerScores.CoeffVar() },
                Draws = new UncertainValue() { Value = totalDraws.Average(), Uncertainty = totalDraws.Sterr(), CoefficientOfVariation = totalPlayerScores.CoeffVar() },
                NormalizedRewards = new UncertainValue() { Value = totalRewards.Average(), Uncertainty = totalRewards.Sterr(), CoefficientOfVariation = totalPlayerScores.CoeffVar() },
                WinPercentage = new UncertainValue() { Value = totalWinrate.Average(), Uncertainty = totalWinrate.Sterr(), CoefficientOfVariation = totalWinrate.CoeffVar() }
            };
        }

        private static WinrateEstimate EstimateRandomWinrate(int maxGames, int samples)
        {
            var mutex = new object();
            var totalPlayerScores = new List<double>();
            var totalDealerScores = new List<double>();
            var totalDraws = new List<double>();
            var totalRewards = new List<double>();
            var totalWinrate = new List<double>();

            Parallel.For(0, samples, j =>
            {
                var model = Learning.GetRandomModel();
                var playerScore = 0.0;
                var dealerScore = 0.0;
                var draws = 0.0;
                var rewards = 0.0;
                var wins = 0.0;

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
                        {
                            playerScore += reward;
                            wins++;
                        }
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
                    totalWinrate.Add(100 * wins / maxGames);
                }
            });

            return new WinrateEstimate
            {
                PlayerScore = new UncertainValue() { Value = totalPlayerScores.Average(), Uncertainty = totalPlayerScores.Sterr(), CoefficientOfVariation = totalPlayerScores.CoeffVar() },
                DealerScore = new UncertainValue() { Value = totalDealerScores.Average(), Uncertainty = totalDealerScores.Sterr(), CoefficientOfVariation = totalPlayerScores.CoeffVar() },
                Draws = new UncertainValue() { Value = totalDraws.Average(), Uncertainty = totalDraws.Sterr(), CoefficientOfVariation = totalPlayerScores.CoeffVar() },
                NormalizedRewards = new UncertainValue() { Value = totalRewards.Average(), Uncertainty = totalRewards.Sterr(), CoefficientOfVariation = totalPlayerScores.CoeffVar() },
                WinPercentage = new UncertainValue() { Value = totalWinrate.Average(), Uncertainty = totalWinrate.Sterr(), CoefficientOfVariation = totalWinrate.CoeffVar() }
            };
        }
    }

    public class WinrateEstimate
    {
        public UncertainValue PlayerScore { get; set; }
        public UncertainValue DealerScore { get; set; }
        public UncertainValue Draws { get; set; }
        public UncertainValue NormalizedRewards { get; set; }
        public UncertainValue WinPercentage { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("--------------------------------------------");
            builder.AppendLine($"\tPontuação do jogador: {PlayerScore}");
            builder.AppendLine($"\tPontuação do dealer: {DealerScore}");
            builder.AppendLine($"\tEmpates: {Draws}");
            builder.AppendLine($"\tRecompensa normalizada: {NormalizedRewards}");
            builder.AppendLine("--------------------------------------------");

            return $"{builder}";
        }
    }

    public struct UncertainValue
    {
        public double Value { get; set; }
        public double Uncertainty { get; set; }
        public double RelativeUncertainty => Math.Abs(Uncertainty) / Value;
        public double CoefficientOfVariation { get; set; }

        public override string ToString()
        {
            return $"{Value.ToString("0.#######")} ± {Uncertainty.ToString("0.#######")} ({(100 * RelativeUncertainty).ToString("0.##")} %)";
        }
    }
}