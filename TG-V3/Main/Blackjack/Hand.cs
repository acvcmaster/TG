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

        public int HardSum => Cards.Sum(item => item.BlackjackValue);

        public int Sum
        {
            get
            {
                var hardSum = HardSum;

                if (Cards.Where(item => item.FaceValue == FaceValue.Ace).Any())
                {
                    var softSum = 10 + hardSum;
                    return softSum > 21 ? hardSum : softSum;
                }
                else return hardSum;
            }
        }

        public bool SoftHand => HardSum != Sum;

        public int Count => Cards?.Count() ?? 0;

        public Card? CardAt(int index)
        {
            return Cards?.ElementAt(index);
        }

        public void Add(Card card)
        {
            Cards.Add(card);
        }

        public void Replace(int index, Card? card)
        {
            if (card.HasValue)
                Cards[index] = card.Value;
        }
    }
}