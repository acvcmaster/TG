using System;
using System.Linq;
using SM;
using Util;
using Util.Models;
using Runner;

namespace RunnerQ
{
    class Program
    {
        static double[,,,] Q = new double[3, 10, 16, 4];

        static void Main(string[] args)
        {
            Console.WriteLine("Q-Learning: Started.");
            double learningRate = 0.7;
            double discountFactor = 0.1;
            double explorationFactor = 0.3; // tem que cair durante o aprendizado
            int maxEpisodes = 200000;

            RandomDecks.GenerateRandomDecks(maxEpisodes);

            for (int episode = 0; episode < maxEpisodes; episode++)
            {
                var deck = RandomDecks.Pick(episode);
                Blackjack game = new Blackjack(deck, (info) =>
                {
                    if (info.PlayerSum == 21)
                        return BlackjackMove.Stand;

                    var table = GetTable(info);
                    var possibleMoves = PossibleMoves(table);
                    var dealerCard = info.DealerFaceupCard.BlackjackValue != 1 ? info.DealerFaceupCard.BlackjackValue - 2 : 9;
                    var sum = 0;

                    var result = BlackjackMove.Hit;
                    var maxQ = double.NegativeInfinity;

                    switch (table)
                    {
                        case 0:
                            sum = 20 - info.PlayerSum;
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

            Console.WriteLine("Q-Learning: Done.");

            var moves = GetMoves();

            var guid = GuidProvider.NewGuid();
            BlackjackChromosome qChromosome = new BlackjackChromosome(moves);
            Console.Write("Evaluating fitness.. ");
            BlackjackFitness fitnessCalculator = new BlackjackFitness();
            double fitness = fitnessCalculator.Evaluate(qChromosome);
            Console.WriteLine("Done.");

            Console.Write("Generating diagram.. ");
            var diagram = Diagrammer.Generate(qChromosome, null, fitness, 0);
            Diagrammer.Save(diagram, 0, guid);
            Console.WriteLine("Done.");
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

        static BlackjackMove[] GetMoves()
        {
            ConcactableArray<BlackjackMove> moves = new ConcactableArray<BlackjackMove>();
            for (int table = 0; table < 3; table++)
                for (int sum = 0; sum < 16; sum++)
                    for (int dealerCard = 0; dealerCard < 10; dealerCard++)
                    {
                        int maxMove = -1;
                        double maxValue = double.NegativeInfinity;

                        for (int move = 0; move < 4; move++)
                        {
                            var value = Q[table, dealerCard, sum, move];
                            if (value > maxValue)
                            {
                                maxMove = move;
                                maxValue = value;
                            }
                        }

                        moves.Add((BlackjackMove)maxMove);
                    }
            return moves.ToArray();
        }
    }
}
