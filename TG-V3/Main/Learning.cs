using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TG_V3.Blackjack;
using TG_V3.Extensions;
using TG_V3.Util;

namespace TG_V3
{
    enum QLearningTable
    {
        HardHands,
        SoftHands,
        Split
    }

    partial class Learning
    {
        public IDictionary<int, double[,,]> LearningData = new Dictionary<int, double[,,]>();

        static void Main(string[] args)
        {
            // Moves
            // 0 - Stand
            // 1 - Hit
            // 2 - Doubledown
            // 3 - Split

            double[,,] QHardHands = GlobalRandom.NextTable(10, 16, 3); // dealer card, sum, move
            double[,,] QSoftHands = GlobalRandom.NextTable(10, 8, 3); // dealer card, Ace-N, move
            double[,,] QSplit = GlobalRandom.NextTable(10, 10, 4); // dealer card, pair, move


            TrainModel((ep, max) => 0.6, (ep, max) => 0.3, 0.1, 100000, ref QHardHands, ref QSoftHands, ref QSplit);


            // var QHardHands = GetBaselineTable(QLearningTable.HardHands);
            // var QSoftHands = GetBaselineTable(QLearningTable.SoftHands);
            // var QSplit = GetBaselineTable(QLearningTable.Split);


            var policyHardHands = GetOptimalPolicy(QHardHands, 10, 16, new int[] { 0, 1, 2 });
            var policySoftHands = GetOptimalPolicy(QSoftHands, 10, 8, new int[] { 0, 1, 2 });
            var policySplit = GetOptimalPolicy(QSplit, 10, 10, new int[] { 0, 1, 2, 3 });

            PrintPolicy(policyHardHands, 10, 16);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            PrintPolicy(policySoftHands, 10, 8);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            PrintPolicy(policySplit, 10, 10);


            EstimateWinrate(QHardHands, QSoftHands, QSplit, 100000);

            Console.WriteLine("Done!");
        }

        private static void TrainModel(Func<int, int, double> learningRate, Func<int, int, double> explorationFactor, double discountFactor, int maxEpisodes, ref double[,,] QHardHands, ref double[,,] QSoftHands, ref double[,,] QSplit)
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
                            move = GlobalRandom.Next(tableType != QLearningTable.Split ? 3 : 4);
                        else
                        {
                            int[] moves = tableType != QLearningTable.Split
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

        public static double PlayGame(Game game, ref Deck deck, double[,,] QHardHands, double[,,] QSoftHands, double[,,] QSplit)
        {
            if (!game.Final)
            {
                while (!game.Final)
                {
                    var tableType = GetQLearningTable(game);
                    double[,,] table = SelectTable(QHardHands, QSoftHands, QSplit, tableType);
                    int x = GetXIndex(game);
                    int y = GetYIndex(game, tableType);

                    int[] moves = tableType != QLearningTable.Split
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
                            splitReward += PlayGame(splitGame, ref deck, QHardHands, QSoftHands, QSplit);

                        return splitReward;
                    }
                }
            }

            return game.Reward;
        }

        public static double[,,] SelectTable(double[,,] qHardHands, double[,,] qSoftHands, double[,,] qSplit, QLearningTable tableType)
        {
            switch (tableType)
            {
                case QLearningTable.HardHands:
                    return qHardHands;
                case QLearningTable.SoftHands:
                    return qSoftHands;
                case QLearningTable.Split:
                    return qSplit;
            }

            return null;
        }

        public static QLearningTable GetQLearningTable(Game game)
        {
            if (game.CanSplit)
                return QLearningTable.Split;
            else
            {
                if (game.PlayerHand.SoftHand)
                    return QLearningTable.SoftHands;
                else return QLearningTable.HardHands;
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

        public static int GetYIndex(Game game, QLearningTable tableType)
        {
            switch (tableType)
            {
                case QLearningTable.HardHands:
                case QLearningTable.SoftHands:
                    return 20 - game.PlayerHand.Sum;
                case QLearningTable.Split:
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

                var moves = tableType != QLearningTable.Split
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
    }
}
