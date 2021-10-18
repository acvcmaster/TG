using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG_V3.Blackjack;
using TG_V3.Extensions;
using TG_V3.Util;

namespace TG_V3
{
    partial class Visualization
    {
        public static void Main()
        {
            // CalculateHouseEdge();
            // ShowBaselineResults();
            // ShowRandomResults();
            // ShowHardHandsBaselineResults();
            // CalculateLearningCurve((ep, maxEp) => 0.005, (ep, maxEp) => 1, 0.9, 20000, 400000);
            ShowQLearningResults(
                learningRate: (ep, maxEp) => 0.005,
                explorationFactor: (ep, maxEp) => 1,
                discountFactor: 0.9,
                maxEpisodes: 100000
            );
            // ShowLoadedModelResutls("Models/Baseline.dat");
        }

        private static void CalculateHouseEdge()
        {
            using (var dados = new StreamWriter("Data/vantagem_casa.csv"))
            {
                var SAMPLES = 50; // número de amostras (e também de threads)
                var STEP = 10000;
                var MAX_STEPS = 20;
                var baseline = Learning.GetBaselineModel();

                for (var max_games = STEP; max_games <= MAX_STEPS * STEP; max_games += STEP)
                {
                    var baselineEstimate = EstimateWinrate(baseline, max_games, SAMPLES);
                    var normalizedRewards = baselineEstimate.NormalizedRewards;
                    var winPercentage = baselineEstimate.WinPercentage;

                    dados.WriteLine($"{max_games}, {winPercentage.Value}, {winPercentage.Uncertainty}, {winPercentage.RelativeUncertainty}, {winPercentage.CoefficientOfVariation}, {normalizedRewards.Value}, {normalizedRewards.Uncertainty}, {normalizedRewards.RelativeUncertainty}, {normalizedRewards.CoefficientOfVariation}");
                    dados.Flush();
                }
            }
        }

        private static void CalculateLearningCurve(Func<int, int, double> learningRate, Func<int, int, double> explorationFactor, double discountFactor, int step, int maxEpisodes) // Curva de aprendizado (ganhos normalizados vs jogos)
        {
            using (var dados = new StreamWriter("Data/curva_aprendizado.csv"))
            {
                int ESTIMATE_MAX_GAMES = 100000;
                int ESTIMATE_SAMPLES = 50;
                double[,,] qHardHands = GlobalRandom.NextTable(10, 16, 3); // dealer card, sum, move
                double[,,] qSoftHands = GlobalRandom.NextTable(10, 8, 3); // dealer card, Ace-N, move
                double[,,] qSplit = GlobalRandom.NextTable(10, 10, 4); // dealer card, pair, move

                for (var i = 0; i <= maxEpisodes; i += step)
                {
                    var estimate = EstimateWinrate(new QLearningModel() { QHardHands = qHardHands, QSoftHands = qSoftHands, QSplit = qSplit }, ESTIMATE_MAX_GAMES, ESTIMATE_SAMPLES);
                    var winPercentage = estimate.WinPercentage;
                    var normalizedRewards = estimate.NormalizedRewards;

                    Console.WriteLine(i); // Remover

                    dados.WriteLine($"{i}, {winPercentage.Value}, {winPercentage.Uncertainty}, {winPercentage.RelativeUncertainty}, {winPercentage.CoefficientOfVariation}, {normalizedRewards.Value}, {normalizedRewards.Uncertainty}, {winPercentage.RelativeUncertainty}, {normalizedRewards.CoefficientOfVariation}");
                    dados.Flush();

                    Learning.QLearning(learningRate, explorationFactor, discountFactor, step, ref qHardHands, ref qSoftHands, ref qSplit);
                }
            }
        }

        private static void ShowRandomResults()
        {
            Console.WriteLine("\n\n\n");

            var estimate = EstimateRandomWinrate(100000, 50);

            Console.WriteLine($"Random strategy winrate: {estimate.WinPercentage}");
            Console.WriteLine($"Random stategy normalized rewards: {estimate.NormalizedRewards}");
            Console.WriteLine("\n\n\n");
        }

        private static void ShowBaselineResults()
        {
            Console.WriteLine("\n\n\n");

            var baseline = Learning.GetBaselineModel();
            var estimate = EstimateWinrate(baseline, 100000, 50);

            var QHardHands = Learning.GetOptimalPolicy(baseline.QHardHands, 10, 16, new int[] { 0, 1, 2 });
            var QSoftHands = Learning.GetOptimalPolicy(baseline.QSoftHands, 10, 8, new int[] { 0, 1, 2 });
            var QSplit = Learning.GetOptimalPolicy(baseline.QSplit, 10, 10, new int[] { 0, 1, 2, 3 });

            Learning.PrintPolicy(QHardHands, 10, 16);
            Learning.PrintPolicy(QSoftHands, 10, 8);
            Learning.PrintPolicy(QSplit, 10, 10);

            Console.WriteLine($"Baseline strategy winrate: {estimate.WinPercentage}");
            Console.WriteLine($"Baseline stategy normalized rewards: {estimate.NormalizedRewards}");
            Console.WriteLine("\n\n\n");
        }

        private static void ShowHardHandsBaselineResults()
        {
            Console.WriteLine("\n\n\n");

            var estimate = EstimateRandomWinrate(100000, 50, true);

            Console.WriteLine($"Baseline strategy with random soft/split winrate: {estimate.WinPercentage}");
            Console.WriteLine($"Baseline strategy with random soft/split normalized rewards: {estimate.NormalizedRewards}");
            Console.WriteLine("\n\n\n");
        }

        private static void ShowQLearningResults(Func<int, int, double> learningRate, Func<int, int, double> explorationFactor, double discountFactor, int maxEpisodes)
        {
            Console.WriteLine("\n\n\n");

            var learned = Learning.GetQLearningModel(learningRate, explorationFactor, discountFactor, maxEpisodes);
            var estimate = EstimateWinrate(learned, 100000, 50);

            var QHardHands = Learning.GetOptimalPolicy(learned.QHardHands, 10, 16, new int[] { 0, 1, 2 });
            var QSoftHands = Learning.GetOptimalPolicy(learned.QSoftHands, 10, 8, new int[] { 0, 1, 2 });
            var QSplit = Learning.GetOptimalPolicy(learned.QSplit, 10, 10, new int[] { 0, 1, 2, 3 });

            Learning.PrintPolicy(QHardHands, 10, 16);
            Learning.PrintPolicy(QSoftHands, 10, 8);
            Learning.PrintPolicy(QSplit, 10, 10);

            var parameterString = GetParameterString(learningRate, explorationFactor, discountFactor, maxEpisodes);
            Learning.SaveModel(learned, $"{parameterString}{estimate}");

            Console.WriteLine($"Learned strategy winrate: {estimate.WinPercentage}");
            Console.WriteLine($"Learned stategy normalized rewards: {estimate.NormalizedRewards}");
            Console.WriteLine("\n\n\n");
        }

        private static string GetParameterString(Func<int, int, double> learningRate, Func<int, int, double> explorationFactor, double discountFactor, int maxEpisodes)
        {
            var lr = learningRate(0, 0);
            var ef = explorationFactor(0, 0);
            var df = discountFactor;
            var me = maxEpisodes;

            return @$"
Parâmetros utilizados:
    Taxa de aprendizado: {lr.ToString("0.#######")}
    Fator de exploração: {ef.ToString("0.#######")}
    Fator de desconto: {df.ToString("0.#######")}
    Número de episódios: {me.ToString("0.#######")}
";
        }

        private static void ShowLoadedModelResutls(string path)
        {
            Console.WriteLine("\n\n\n");

            var learned = Learning.LoadModel(path);
            var estimate = EstimateWinrate(learned, 100000, 50);

            var QHardHands = Learning.GetOptimalPolicy(learned.QHardHands, 10, 16, new int[] { 0, 1, 2 });
            var QSoftHands = Learning.GetOptimalPolicy(learned.QSoftHands, 10, 8, new int[] { 0, 1, 2 });
            var QSplit = Learning.GetOptimalPolicy(learned.QSplit, 10, 10, new int[] { 0, 1, 2, 3 });

            Learning.PrintPolicy(QHardHands, 10, 16);
            Learning.PrintPolicy(QSoftHands, 10, 8);
            Learning.PrintPolicy(QSplit, 10, 10);

            Console.WriteLine($"Learned strategy winrate: {estimate.WinPercentage}");
            Console.WriteLine($"Learned stategy normalized rewards: {estimate.NormalizedRewards}");
            Console.WriteLine("\n\n\n");
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

        private static WinrateEstimate EstimateRandomWinrate(int maxGames, int samples, bool baselineHardHands = false)
        {
            var mutex = new object();
            var totalPlayerScores = new List<double>();
            var totalDealerScores = new List<double>();
            var totalDraws = new List<double>();
            var totalRewards = new List<double>();
            var totalWinrate = new List<double>();
            var baseline = Learning.GetBaselineModel();

            Parallel.For(0, samples, j =>
            {
                var model = Learning.GetRandomModel();

                if (baselineHardHands)
                {
                    model.QHardHands = baseline.QHardHands;
                    model.Name = "Hardhands ótimas";
                }

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

            builder.AppendLine("Resultados:");
            builder.AppendLine($"\tPontuação do jogador: {PlayerScore}");
            builder.AppendLine($"\tPontuação do dealer: {DealerScore}");
            builder.AppendLine($"\tEmpates: {Draws}");
            builder.AppendLine($"\tTaxa de vitória: {WinPercentage}");
            builder.AppendLine($"\tRecompensa normalizada: {NormalizedRewards}");

            return $"{builder}";
        }
    }

    public struct UncertainValue
    {
        public double Value { get; set; }
        public double Uncertainty { get; set; }
        public double RelativeUncertainty => 100 * Math.Abs(Uncertainty) / Math.Abs(Value);
        public double CoefficientOfVariation { get; set; }

        public override string ToString()
        {
            return $"{Value.ToString("0.#######")} ± {Uncertainty.ToString("0.#######")} ({(RelativeUncertainty).ToString("0.##")} %)";
        }
    }
}