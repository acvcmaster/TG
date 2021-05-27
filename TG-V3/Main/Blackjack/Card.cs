namespace TG_V3.Blackjack
{
    public enum Suit : int
    {
        Diamonds = 1,
        Spades,
        Hearts,
        Clubs
    }

    public enum FaceValue : int
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

    public struct Card
    {
        public FaceValue FaceValue { get; set; }
        public Suit Suit { get; set; }

        public Card(FaceValue faceValue, Suit suit)
        {
            this.FaceValue = faceValue;
            this.Suit = suit;
        }

        public Card(int faceValue, int suit)
        {
            this.FaceValue = (FaceValue)faceValue;
            this.Suit = (Suit)suit;
        }

        public string Name
        {
            get { return $"{this.FaceValue.ToString()} of {this.Suit.ToString()}"; }
        }

        public int BlackjackValue
        {
            get
            {
                int facevalue = (int)this.FaceValue;
                return facevalue < 10 ? facevalue : 10;
            }
        }
    }
}
