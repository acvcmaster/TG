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
            get { return (int)this.FaceValue < 10 ? (int)this.FaceValue: 10; }
        }
    }
}
