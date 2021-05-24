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
                int sum = 0;
                int aces = 0;

                foreach (var card in Cards)
                {
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