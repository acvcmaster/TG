using System.Linq;
using TG_V3.Extensions;

namespace TG_V3.Blackjack
{
    public struct Deck
    {
        public Card[] Cards { get; set; } // 4 decks of 52 cards

        public Deck(Deck previous)
        {
            Cards = previous.Cards?.Clone<Card>();
        }

        /// <summary>
        /// Contructs a shuffled deck
        /// </summary>
        /// <param name="numDecks"></param>
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

            Shuffle();
        }

        public void Shuffle() => Cards.Shuffle();

        public Card Pop()
        {
            var result = Cards.First();
            Cards = Cards.Skip(1).ToArray();

            return result;
        }
    }
}