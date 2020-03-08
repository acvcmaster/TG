using Util;

namespace SM
{
    public static class RandomDecks
    {
        private static Deck[] randomDecks;
        public static int Count { get; set; }
        public static void GenerateRandomDecks(int count = 100000, int DeckCount = 4)
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
            int index = StaticRandom.Next(Count);
            return randomDecks[index];
        }
    }
}