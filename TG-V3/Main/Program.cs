﻿using System;
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

    class Program
    {
        static void Main(string[] args)
        {
            double learningRate = 0.6;
            double discountFactor = 0.1;
            double explorationFactor = 0.25; // tem que cair durante o aprendizado
            int batchSize = 10000;
            int maxBatches = 10000;

            // Moves
            // 0 - Stand
            // 1 - Hit
            // 2 - Doubledown
            // 3 - Split

            double[,,] QHardHands = GlobalRandom.NextTable(10, 16, 3); // dealer card, sum, move
            double[,,] QSoftHands = GlobalRandom.NextTable(10, 8, 3); // dealer card, Ace-N, move
            double[,,] QSplit = GlobalRandom.NextTable(10, 10, 4); // dealer card, pair, move
            var mutex = new Object();

            Parallel.For(0, maxBatches, (batch =>
            {
                for (int episode = 0; episode < batchSize; episode++)
                {
                    Deck deck = new Deck(4);
                    Game game = new Game(deck);

                    if (!game.Final)
                    {
                        while (!game.Final)
                        {
                            var tableType = GetQLearningTable(game);
                            double[,,] table = SelectTable(QHardHands, QSoftHands, QSplit, tableType);
                            int x = GetXIndex(game);
                            int y = GetYIndex(game, tableType);

                            int move = 0;
                            if (GlobalRandom.NextDouble() < explorationFactor)
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
                                game = MakeMove(game, deck, move);
                                var reward = game.Reward;

                                lock (mutex)
                                    table[x, y, move] += learningRate * (reward + discountFactor * EstimateMax(QHardHands, QSoftHands, QSplit, game, deck) - table[x, y, move]);
                            }
                            else
                            {
                                // Calculate split and break
                                var games = Split(game, deck);
                                var splitReward = games.Select(item => item.Reward).Average(); // Média ou soma?

                                lock (mutex)
                                    table[x, y, move] += learningRate * (splitReward + discountFactor * EstimateMaxOnSplit(QHardHands, QSoftHands, QSplit, games, deck) - table[x, y, move]);
                                break;
                            }
                        }
                    }
                }
            }));

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


            Console.WriteLine("Done!");
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
        public static Game MakeMove(Game game, Deck deck, int move)
        {
            switch (move)
            {
                case 0:
                    return game.Stand(deck);
                case 1:
                    return game.Hit(deck);
                case 2:
                    return game.DoubleDown(deck);
            }

            return game;
        }

        public static double EstimateMax(double[,,] qHardHands, double[,,] qSoftHands, double[,,] qSplit, Game game, Deck deck)
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

        public static double EstimateMaxOnSplit(double[,,] qHardHands, double[,,] qSoftHands, double[,,] qSplit, IEnumerable<Game> games, Deck deck)
        {
            return games.Select(game => EstimateMax(qHardHands, qSoftHands, qSplit, game, deck)).Average();
        }

        // Altera o deck
        public static IEnumerable<Game> Split(Game game, Deck deck)
        {
            return game.Split(deck);
        }

        public static char[,] GetOptimalPolicy(double[,,] table, int x, int y, int[] moves)
        {
            char[,] result = new char[x, y];
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                {
                    switch (moves.MaxOver(item => table[i, j, item]))
                    {
                        case 0:
                            result[i, j] = 'S';
                            break;
                        case 1:
                            result[i, j] = 'H';
                            break;
                        case 2:
                            result[i, j] = 'D';
                            break;
                        case 3:
                            result[i, j] = 'P';
                            break;
                    }
                }

            return result;
        }

        public static void PrintPolicy(char[,] policy, int x, int y)
        {
            var foreground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Black;
            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    var move = policy[i, j];
                    var color = Console.BackgroundColor;

                    switch (move)
                    {
                        case 'S':
                            Console.BackgroundColor = ConsoleColor.Red;
                            break;
                        case 'H':
                            Console.BackgroundColor = ConsoleColor.Green;
                            break;
                        case 'D':
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            break;
                        case 'P':
                            Console.BackgroundColor = ConsoleColor.Magenta;
                            break;
                    }
                    Console.Write($" {move} ");
                    Console.BackgroundColor = color;
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = foreground;
        }
    }
}