using System;
using System.Linq;

namespace SM
{
    public class Deck
    {
        private Random rng = new Random();
        public Card[] Cards { get; set; } // 4 decks of 52 cards
        public Deck(int numDecks = 4)
        {
            Cards = new Card[numDecks * 52];
            int index = 0;
            for (int card = 1; card <= 13; card++)
                for (int suit = 1; suit <= 4; suit++)
                    for (int i = 0; i < numDecks; i++)
                    {
                        Cards[index] = new Card(card, suit);
                        index++;
                    }
        }

        /// <summary>
        /// Fisher-Yates shuffle. [Complexity: O(n)]
        /// </summary>
        public void Shuffle()
        {
            for (int i = 0; i < Cards.Length; i++)
            {
                var j = rng.Next(0, i + 1);
                var temp = Cards[i];
                Cards[i] = Cards[j];
                Cards[j] = temp;
            }
        }
    }
}