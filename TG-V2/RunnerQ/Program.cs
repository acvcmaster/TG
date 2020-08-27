using System;
using System.Linq;
using SM;
using Util;
using Util.Models;

namespace RunnerQ
{
    class Program
    {
        static double[,,,] Q = new double[3, 10, 17, 4];

        static void Main(string[] args)
        {
            double learningRate = 0.5;
            double discountFactor = 0.5;
            double explorationFactor = 1; // tem que cair durante o aprendizado
            int maxEpisodes = 50000;

            RandomDecks.GenerateRandomDecks(maxEpisodes);

            for (int episode = 0; episode < maxEpisodes; episode++)
            {
                var deck = RandomDecks.Pick(episode);
                Blackjack game = new Blackjack(deck, (info) =>
                {
                    var table = GetTable(info);
                    var possibleMoves = PossibleMoves(table);
                    var dealerCard = info.DealerFaceupCard.BlackjackValue != 1 ? info.DealerFaceupCard.BlackjackValue - 2 : 9;
                    var sum = 0;

                    var result = BlackjackMove.Hit;
                    var maxQ = double.NegativeInfinity;

                    switch (table)
                    {
                        case 0:
                            sum = 21 - info.PlayerSum;
                            break;
                        case 1:
                            sum = 9 - (info.PlayerHand[0].FaceValue == FaceValue.Ace ? info.PlayerHand[1].BlackjackValue : info.PlayerHand[0].BlackjackValue);
                            break;
                        case 2:
                            sum = info.PlayerHand[0].FaceValue != FaceValue.Ace ? 11 - info.PlayerHand[0].BlackjackValue : 0;
                            break;
                    }

                    foreach (var move in possibleMoves)
                    {
                        var q = Q[table, dealerCard, sum, (int)move];
                        if (q > maxQ)
                        {
                            maxQ = q;
                            result = move;
                        }
                    }

                    if (StaticRandom.NextDouble(0, 1) < explorationFactor)
                        result = possibleMoves.RandomElement();

                    // lookahead para ver reward (0 se for Hit)
                    var reward = result == BlackjackMove.Hit ? 0 : (info.Game as Blackjack).Lookahead(result);
                    Q[table, dealerCard, sum, (int)result] += learningRate * (reward + discountFactor * maxQ - Q[table, dealerCard, sum, (int)result]);
                    return result;
                });

                game.Play();
            }
            Console.WriteLine("Done!");
        }

        static BlackjackMove[] PossibleMoves(int table)
        {
            ConcactableArray<BlackjackMove> resultado = new ConcactableArray<BlackjackMove>() { BlackjackMove.Hit, BlackjackMove.Stand, BlackjackMove.DoubleDown };
            if (table == 2)
            {
                return new ConcactableArray<BlackjackMove>() { resultado, BlackjackMove.Split }.ToArray();
            }
            return resultado.ToArray();
        }

        static int GetTable(BlackjackInformation info)
        {
            if (info.PlayerHandIndex == 2)
            {
                if (info.PlayerHand[0].BlackjackValue == info.PlayerHand[1].BlackjackValue)
                    return 2;
                else if (info.PlayerHand[0].FaceValue == FaceValue.Ace || info.PlayerHand[1].FaceValue == FaceValue.Ace)
                    return 1;
            }
            return 0;
        }
    }
}
