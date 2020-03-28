using System;

namespace Util.Models
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

        public void Shuffle() => Cards.Shuffle();
    }
}