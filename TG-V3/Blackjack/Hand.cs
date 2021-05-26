using System.Collections.Generic;
using System.Linq;

namespace TG_V3.Blackjack
{
    public class Hand
    {
        public Hand()
        {
            Cards = new List<Card>();
        }

        public Hand(Hand previous)
        {
            Cards = new List<Card>(previous?.Cards);
        }

        public List<Card> Cards { get; set; }

        public int Sum
        {
            get
            {
                if (SoftHand)
                {
                    var first = CardAt(0).Value;
                    var second = CardAt(1).Value;

                    return 10 + (first.FaceValue == FaceValue.Ace
                        ? second.BlackjackValue
                        : first.BlackjackValue);
                }
                else return Cards.Sum(item => item.BlackjackValue);
            }
        }

        public bool SoftHand
        {
            get
            {
                if (Count == 2)
                {
                    return CardAt(0)?.FaceValue == FaceValue.Ace ^
                        CardAt(1)?.FaceValue == FaceValue.Ace;
                }

                return false;
            }
        }

        public int Count => Cards?.Count() ?? 0;

        public Card? CardAt(int index)
        {
            return Cards?.ElementAt(index);
        }

        public void Add(Card card)
        {
            Cards.Add(card);
        }
    }
}