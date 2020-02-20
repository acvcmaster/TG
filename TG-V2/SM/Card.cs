using System;

namespace SM
{
    public enum Suit: int
    {
        Diamonds = 1,
        Spades,
        Hearts,
        Clubs
    }

    public enum FaceValue: int
    {
        Ace = 1,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Queen,
        Jack,
        King
    }

    public class Card
    {
        public FaceValue FaceValue { get; set; } = FaceValue.Ace;
        public Suit Suit { get; set; } = Suit.Diamonds;

        public string Name
        {
            get { return $"{this.FaceValue.ToString()} of {this.Suit.ToString()}"; }
        }

        public int BlackjackValue 
        {
            get
            {
                return 0;
            }
        }

        public static implicit operator Card(dynamic[] values)
        {
            return new Card();
        }
    }
}
