using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TG_V3.Blackjack;
using TG_V3.Extensions;
using TG_V3.Util;

namespace TG_V3
{

    partial class Learning
    {
        public enum QLearningTable
        {
            HardHands,
            SoftHands,
            Split
        }

        public static QLearningModel GetQLearningModel(Func<int, int, double> learningRate, Func<int, int, double> explorationFactor, double discountFactor, int maxEpisodes)
        {
            // Moves
            // 0 - Stand
            // 1 - Hit
            // 2 - Doubledown
            // 3 - Split

            double[,,] qHardHands = GlobalRandom.NextTable(10, 16, 3); // dealer card, sum, move
            double[,,] qSoftHands = GlobalRandom.NextTable(10, 8, 3); // dealer card, Ace-N, move
            double[,,] qSplit = GlobalRandom.NextTable(10, 10, 4); // dealer card, pair, move

            QLearning(learningRate, explorationFactor, discountFactor, maxEpisodes, ref qHardHands, ref qSoftHands, ref qSplit);

            return new QLearningModel
            {
                Name = "Aprendido por Q-Learning",
                QHardHands = qHardHands,
                QSoftHands = qSoftHands,
                QSplit = qSplit
            };
        }

        public static QLearningModel GetBaselineModel()
        {
            return new QLearningModel
            {
                Name = "Ótimo",
                QHardHands = GetBaselineTable(QLearningTable.HardHands),
                QSoftHands = GetBaselineTable(QLearningTable.SoftHands),
                QSplit = GetBaselineTable(QLearningTable.Split)
            };
        }

        public static QLearningModel GetRandomModel()
        {
            return new QLearningModel
            {
                Name = "Aleatório",
                QHardHands = GlobalRandom.NextTable(10, 16, 3),
                QSoftHands = GlobalRandom.NextTable(10, 8, 3),
                QSplit = GlobalRandom.NextTable(10, 10, 4)
            };
        }

        public static void QLearning(Func<int, int, double> learningRate, Func<int, int, double> explorationFactor, double discountFactor, int maxEpisodes, ref double[,,] QHardHands, ref double[,,] QSoftHands, ref double[,,] QSplit)
        {
            for (int episode = 0; episode < maxEpisodes; episode++)
            {
                Deck deck = new Deck(4);
                Game game = new Game(ref deck);

                double learning_rate = learningRate(episode, maxEpisodes);
                double exploration_factor = explorationFactor(episode, maxEpisodes);

                if (!game.Final)
                {
                    while (!game.Final)
                    {
                        var tableType = GetQLearningTable(game);
                        double[,,] table = SelectTable(QHardHands, QSoftHands, QSplit, tableType);
                        int x = GetXIndex(game);
                        int y = GetYIndex(game, tableType);

                        int move = 0;
                        if (GlobalRandom.NextDouble() < exploration_factor)
                            move = GlobalRandom.Next(tableType != Learning.QLearningTable.Split ? 3 : 4);
                        else
                        {
                            int[] moves = tableType != Learning.QLearningTable.Split
                                ? new int[] { 0, 1, 2 }
                                : new int[] { 0, 1, 2, 3 };
                            move = moves.MaxOver(action => table[x, y, action]);
                        }

                        if (move != 3)
                        {
                            // No split
                            game = MakeMove(game, ref deck, move);
                            var reward = game.Reward;

                            table[x, y, move] += learning_rate * (reward + discountFactor * EstimateMax(QHardHands, QSoftHands, QSplit, game) - table[x, y, move]);
                        }
                        else
                        {
                            // Calculate split and break
                            var games = Split(game, ref deck);
                            var splitReward = games.Select(item => item.Reward).Sum(); // Soma os lucros imediatos

                            table[x, y, move] += learning_rate * (splitReward + discountFactor * EstimateMaxOnSplit(QHardHands, QSoftHands, QSplit, games) - table[x, y, move]);
                            break;
                        }
                    }
                }
            }
        }

        public static double PlayGame(Game game, ref Deck deck, QLearningModel model)
        {
            if (!game.Final)
            {
                while (!game.Final)
                {
                    var tableType = GetQLearningTable(game);
                    double[,,] table = SelectTable(model.QHardHands, model.QSoftHands, model.QSplit, tableType);
                    int x = GetXIndex(game);
                    int y = GetYIndex(game, tableType);

                    int[] moves = tableType != Learning.QLearningTable.Split
                        ? new int[] { 0, 1, 2 }
                        : new int[] { 0, 1, 2, 3 };
                    var move = moves.MaxOver(action => table[x, y, action]);

                    if (move != 3)
                    {
                        // No split
                        game = MakeMove(game, ref deck, move);
                    }
                    else
                    {
                        // Calculate split and break
                        var splitGames = Split(game, ref deck);
                        var splitReward = 0.0;

                        foreach (var splitGame in splitGames)
                            splitReward += PlayGame(splitGame, ref deck, model);

                        return splitReward;
                    }
                }
            }

            return game.Reward;
        }

        public static double[,,] SelectTable(double[,,] qHardHands, double[,,] qSoftHands, double[,,] qSplit, Learning.QLearningTable tableType)
        {
            switch (tableType)
            {
                case Learning.QLearningTable.HardHands:
                    return qHardHands;
                case Learning.QLearningTable.SoftHands:
                    return qSoftHands;
                case Learning.QLearningTable.Split:
                    return qSplit;
            }

            return null;
        }

        public static Learning.QLearningTable GetQLearningTable(Game game)
        {
            if (game.CanSplit)
                return Learning.QLearningTable.Split;
            else
            {
                if (game.PlayerHand.SoftHand)
                    return Learning.QLearningTable.SoftHands;
                else return Learning.QLearningTable.HardHands;
            }
        }

        public static int GetXIndex(Game game)
        {
            var dealerCard = game?.DealerFaceUpCard?.BlackjackValue;
            if (dealerCard.HasValue)
            {
                var value = dealerCard.Value;
                return value == 1 ? 9 : value - 2;
            }

            return -1;
        }

        public static int GetYIndex(Game game, Learning.QLearningTable tableType)
        {
            switch (tableType)
            {
                case Learning.QLearningTable.HardHands:
                case Learning.QLearningTable.SoftHands:
                    return 20 - game.PlayerHand.Sum;
                case Learning.QLearningTable.Split:
                    return (game.PlayerHand.Sum / 2) == 1 ? 0 : (11 - game.PlayerHand.Sum / 2);
            }

            return -1;
        }

        // Altera o deck
        public static Game MakeMove(Game game, ref Deck deck, int move)
        {
            switch (move)
            {
                case 0:
                    return game.Stand(ref deck);
                case 1:
                    return game.Hit(ref deck);
                case 2:
                    return game.DoubleDown(ref deck);
            }

            return game;
        }

        public static double EstimateMax(double[,,] qHardHands, double[,,] qSoftHands, double[,,] qSplit, Game game)
        {
            if (!game.Final)
            {
                var tableType = GetQLearningTable(game);
                double[,,] table = SelectTable(qHardHands, qSoftHands, qSplit, tableType);

                int x = GetXIndex(game);
                int y = GetYIndex(game, tableType);

                var moves = tableType != Learning.QLearningTable.Split
                    ? new int[] { 0, 1, 2 }
                    : new int[] { 0, 1, 2, 3 };
                var bestMove = moves.MaxOver(action => table[x, y, action]);

                return table[x, y, bestMove];
            }

            return 0;
        }

        public static double EstimateMaxOnSplit(double[,,] qHardHands, double[,,] qSoftHands, double[,,] qSplit, IEnumerable<Game> games)
        {
            return games.Select(game => EstimateMax(qHardHands, qSoftHands, qSplit, game)).Sum();
        }

        // Altera o deck
        public static IEnumerable<Game> Split(Game game, ref Deck deck)
        {
            return game.Split(ref deck);
        }


        public static QLearningModel LoadModel(string path)
        {
            using (var reader = new StreamReader(path))
            {
                var polices = LoadPolicies(reader.BaseStream);

                if (polices != null)
                {
                    return new QLearningModel()
                    {
                        Name = Path.GetFileNameWithoutExtension(path),
                        QHardHands = GetTableFromPolicy(polices.Item1, 10, 16, 3),
                        QSoftHands = GetTableFromPolicy(polices.Item2, 10, 8, 3),
                        QSplit = GetTableFromPolicy(polices.Item3, 10, 10, 4),
                    };
                }
                else
                    throw new Exception($"Failed to load model from file '{path}'.");
            }
        }

        public static Tuple<char[,], char[,], char[,]> LoadPolicies(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var data = reader.ReadToEnd();
                var moves = GetValidatedPolices(data);
                var hardHands = new char[16, 10];
                var softHands = new char[8, 10];
                var splits = new char[10, 10];
                var index = 0;

                for (int j = 0; j < 16; j++)
                    for (int i = 0; i < 10; i++)
                    {
                        hardHands[j, i] = moves[index];
                        index++;
                    }

                for (int j = 0; j < 8; j++)
                    for (int i = 0; i < 10; i++)
                    {
                        softHands[j, i] = moves[index];
                        index++;
                    }

                for (int j = 0; j < 10; j++)
                    for (int i = 0; i < 10; i++)
                    {
                        splits[j, i] = moves[index];
                        index++;
                    }

                return new Tuple<char[,], char[,], char[,]>
                (
                    Transpose(hardHands, 10, 16),
                    Transpose(softHands, 10, 8),
                    Transpose(splits, 10, 10)
                );
            }
        }

        public static void SaveModel(QLearningModel model, string report = null)
        {
            var QHardHands = Transpose(Learning.GetOptimalPolicy(model.QHardHands, 10, 16, new int[] { 0, 1, 2 }), 16, 10);
            var QSoftHands = Transpose(Learning.GetOptimalPolicy(model.QSoftHands, 10, 8, new int[] { 0, 1, 2 }), 8, 10);
            var QSplit = Transpose(Learning.GetOptimalPolicy(model.QSplit, 10, 10, new int[] { 0, 1, 2, 3 }), 10, 10);
            var name = Guid.NewGuid().ToString();
            char[] result = new char[340];
            var index = 0;

            for (int j = 0; j < 16; j++)
                for (int i = 0; i < 10; i++)
                {
                    result[index] = QHardHands[j, i];
                    index++;
                }

            for (int j = 0; j < 8; j++)
                for (int i = 0; i < 10; i++)
                {
                    result[index] = QSoftHands[j, i];
                    index++;
                }

            for (int j = 0; j < 10; j++)
                for (int i = 0; i < 10; i++)
                {
                    result[index] = QSplit[j, i];
                    index++;
                }

            using (var writer = new StreamWriter($"Data/Models/{name}.dat"))
                writer.Write(string.Join(',', result));

            if (!string.IsNullOrWhiteSpace(report))
            {
                using (var writer = new StreamWriter($"Data/Models/{name}.log"))
                    writer.Write(report);
            }
        }
    }

    public class QLearningModel
    {
        public string Name { get; set; }
        public double[,,] QHardHands { get; set; }
        public double[,,] QSoftHands { get; set; }
        public double[,,] QSplit { get; set; }
    }
}
