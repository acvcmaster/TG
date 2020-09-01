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

                    int table, dealerCard, sum;
                    BlackjackMove result = BlackjackMove.Hit;

                    GetIndexes(info, out table, out dealerCard, out sum);
                    BlackjackMove[] possibleMoves = PossibleMoves(table);

                    if (StaticRandom.NextDouble(0, 1) < explorationFactor)
                        result = possibleMoves.RandomElement();
                    else
                        result = possibleMoves.MaxOver(element => Q[table, dealerCard, sum, (int)element]);


                    var reward = (info.Game as Blackjack).Lookahead(result);
                    // Obter próximo estado (blackjack info, se é final ou não e a reward)
                    // Se o estado for final maxQ = 0
                    Q[table, dealerCard, sum, (int)result] += learningRate * (reward + discountFactor * maxQ - Q[table, dealerCard, sum, (int)result]);
                    return result;
                });

                game.Play();
            }

            Console.WriteLine("Q-Learning: Done.");

            var guid = GuidProvider.NewGuid(false);
            BlackjackChromosome qChromosome = new BlackjackChromosome(GetPolicy());
            Console.Write("Evaluating fitness.. ");
            BlackjackFitness fitnessCalculator = new BlackjackFitness();
            double fitness = fitnessCalculator.Evaluate(qChromosome);
            Console.WriteLine("Done.");

            Console.Write("Generating diagram.. ");
            var diagram = Diagrammer.Generate(qChromosome, null, fitness, 0);
            Diagrammer.Save(diagram, 0, guid);
            Console.WriteLine("Done.");
        }

        private static void GetIndexes(BlackjackInformation info, out int table, out int dealerCard, out int sum)
        {
            table = GetTable(info);
            dealerCard = info.DealerFaceupCard.BlackjackValue != 1 ? info.DealerFaceupCard.BlackjackValue - 2 : 9;
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
                default:
                    sum = 0;
                    break;
            }
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

        static BlackjackMove[] GetPolicy()
        {
            var moves = new ConcactableArray<BlackjackMove>();

            for (int table = 0; table < 3; table++)
                for (int sum = 0; sum < 16; sum++)
                    for (int dealerCard = 0; dealerCard < 10; dealerCard++)
                    {
                        var bestMove = PossibleMoves(table).MaxOver(element => Q[table, dealerCard, sum, (int)element]);
                        moves.Add(bestMove); // default(BlackjackMove) como fallback
                    }
            return moves.ToArray();
        }
    }
}
