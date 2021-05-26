using System;
using System.Collections.Generic;
using System.Linq;
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
            double learningRate = 0.7;
            double discountFactor = 0.1;
            double explorationFactor = 0.3; // tem que cair durante o aprendizado
            int maxEpisodes = 10000;

            // Moves
            // 0 - Stand
            // 1 - Hit
            // 2 - Doubledown
            // 3 - Split

            double[,,] QHardHands = new double[10, 18, 3]; // dealer card, sum, move
            double[,,] QSoftHands = new double[10, 8, 3]; // dealer card, Ace-N, move
            double[,,] QSplit = new double[10, 10, 4]; // dealer card, pair, move

            var count = 0;


            for (int episode = 0; episode < maxEpisodes; episode++)
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

                        if (tableType != QLearningTable.Split)
                        {
                            int move = 0;
                            if (GlobalRandom.NextDouble() < explorationFactor)
                                move = GlobalRandom.Next(3);
                            else
                            {
                                int[] moves = new int[] { 0, 1, 2 };
                                move = moves.MaxOver(action => table[x, y, action]);
                            }

                            game = MakeMove(game, deck, move);
                            var reward = game.Reward;

                            table[x, y, move] += learningRate * (reward + discountFactor * EstimateMax(game, deck) - table[x, y, move]);
                        }
                        else
                        {
                            // ignorar por enquanto
                            break;
                        }
                    }
                }

                Console.WriteLine(++count);
            }
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
                    return 20 - game.PlayerHand.Sum;
                case QLearningTable.SoftHands:
                    return 19 - game.PlayerHand.Sum;
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

        // Não altera o deck
        public static Game PeekMove(Game game, Deck deck, int move)
        {
            var newDeck = new Deck(deck);
            return MakeMove(game, newDeck, move);
        }

        public static double EstimateMax(Game game, Deck deck)
        {
            List<Game> games = new List<Game>();

            if (!game.Final)
            {
                for (int i = 0; i < 3; i++)
                    games.Add(PeekMove(game, deck, i));

                return games.Select(item => item.Reward).OrderByDescending(item => item).First();
            }

            return 0;
        }

        public static double EstimateMaxSplit()
        {
            return 0;
        }
    }
}
