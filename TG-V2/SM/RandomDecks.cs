using System;

namespace SM
{
    public static class RandomDecks
    {
        private static Random random = new Random();
        private static Deck[] randomDecks;
        public static int Count { get; set; }
        public static void GenerateRandomDecks(int DeckCount, int count = 100000)
        {
            Count = count;
            randomDecks = new Deck[count];
            for (int i = 0; i < count; i++)
            {
                randomDecks[i] = new Deck(DeckCount);
                randomDecks[i].Shuffle();
            }
        }

        public static Deck PickRandom()
        {
            int index = random.Next(Count);
            return randomDecks[index];
        }
    }
}