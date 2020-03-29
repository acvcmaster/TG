using System;
using Util.Models;

namespace Runner
{
    public static class StrategyMapper
    {
        public static readonly int[] Tables = new int[] { 160, 240, 340 };
        public static BlackjackMove GetMove(BlackjackInformation information, BlackjackMove[] moves)
        {
            if (information.PlayerSum == 21)
                return BlackjackMove.Stand;

            var dealer = information.DealerFaceupCard.BlackjackValue;
            if (information.PlayerHandIndex == 2)
            {
                var face1 = information.PlayerHand[0].FaceValue;
                var face2 = information.PlayerHand[1].FaceValue;
                if ((face1 == FaceValue.Ace || face2 == FaceValue.Ace) && !(face1 == FaceValue.Ace && face2 == FaceValue.Ace)) // Soft hand
                {
                    var other = face1 == FaceValue.Ace ?
                        information.PlayerHand[1].BlackjackValue : information.PlayerHand[0].BlackjackValue;
                    return moves[GetIndex(Table.SoftHands, dealer, 9 - other)];
                }
                else if (face1 == face2) // Pairs
                    return moves[GetIndex(Table.Pairs, dealer, face1 == FaceValue.Ace ?
                        0 : 11 - information.PlayerHand[0].BlackjackValue)];
                else return moves[GetIndex(Table.HardHands, dealer, 20 - information.PlayerSum)]; // hard hand
            }
            else return moves[GetIndex(Table.HardHands, dealer, 20 - information.PlayerSum)]; // hard hand
        }
        private static int GetIndex(Table table, int dealerCard, int verticalIndex)
        {
            int itable = (int)table;
            int startIndex = (itable - 1) < 0 ? 0 : Tables[itable - 1];
            int endIndex = Tables[itable];
            int dealerCardIndex = dealerCard == 1 ? 9 : dealerCard - 2;

            int result = startIndex + (10 * verticalIndex + dealerCardIndex);

            if (result >= endIndex)
                throw new InvalidOperationException();

            return result;
        }
    }

    public enum Table : int
    {
        HardHands = 0,
        SoftHands,
        Pairs
    }
}