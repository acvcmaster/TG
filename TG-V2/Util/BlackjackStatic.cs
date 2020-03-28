using Util.Models;

namespace Util
{
    public class BlackjackStatic
    {
        public static bool CheckBlackjack(Card[] hand)
        {
            return hand[0].FaceValue == FaceValue.Ace && hand[1].BlackjackValue == 10 ||
                hand[1].FaceValue == FaceValue.Ace && hand[0].BlackjackValue == 10;
        }

        public static int GetSum(Card[] hand, int length)
        {
            int sum = 0;
            int aces = 0;
            for (int i = 0; i < length; i++)
            {
                Card card = hand[i];
                if (card.FaceValue != FaceValue.Ace)
                    sum += card.BlackjackValue;
                else aces++;
            }

            if (aces > 0)
                if (sum + aces <= 11)
                    aces += 10;

            return sum + aces;
        }
    }
}