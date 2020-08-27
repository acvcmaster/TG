using Util.Models;

namespace Util
{
    public static class RandomDecks
    {
        private static Deck[] randomDecks;
        public static int Count { get; set; }
        public static void GenerateRandomDecks(int count = Global.Games, int DeckCount = 4)
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

        public static Deck Pick(int index)
        {
            return randomDecks[index];
        }
    }
}