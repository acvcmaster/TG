namespace Util.Models
{
    public class BlackjackInformation
    {
        public Card DealerFaceupCard { get; }
        public Card[] PlayerHand { get; }
        public int PlayerHandIndex { get; }
        public bool IsSplit { get; }
        public int PlayerSum
        {
            get { return BlackjackStatic.GetSum(PlayerHand, PlayerHandIndex); }
        }
        public object Game { get; }

        public BlackjackInformation(object Game, Card DealerFaceupCard, Card[] PlayerHand, int PlayerHandIndex, bool IsSplit)
        {
            this.Game = Game;
            this.DealerFaceupCard = DealerFaceupCard;
            this.PlayerHand = PlayerHand;
            this.PlayerHandIndex = PlayerHandIndex;
            this.IsSplit = IsSplit;
        }
    }
    public delegate BlackjackMove BlackjackStrategy(BlackjackInformation game);
}