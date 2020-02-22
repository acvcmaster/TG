using System.Collections.Generic;

namespace SM
{
    public delegate BlackjackMove BlackjackStrategy(Blackjack game);
    public class Blackjack
    {
        private Deck Deck { get; }
        public bool Ended = false;
        public Card DealerFaceupCard { get; set; }
        public List<Card> PlayerHand { get; }
        public List<Card> PlayerSecondHand { get; }
        public List<Card> DealerHand { get; }
        private int deckIndex = 0;
        public Blackjack(Deck deck, BlackjackStrategy strategy)
        {
            Deck = deck;
            CurrentStrategy = strategy;
            PlayerHand = new List<Card>();
            DealerHand = new List<Card>();
        }
        public BlackjackStrategy CurrentStrategy { get; }
        public void Play()
        {
            if (Ended)
                return;

            GameState state = GameState.InitialState;
        process_states:
            switch (state)
            {
                case GameState.InitialState:
                    GiveCard(PlayerHand);
                    GiveCard(PlayerHand);
                    GiveCard(DealerHand, true);
                    Transition(ref state, GameState.PlayerTurn);
                    break;
                case GameState.PlayerTurn:
                    var move = CurrentStrategy(this);
                    Transition(ref state, GameState.FinalState);
                    break;
                default:
                    Ended = true;
                    return;
            }
            goto process_states;
        }
        private void Transition(ref GameState state, GameState next) { state = next; }
        public void Reset() { Ended = false; }
        private void GiveCard(List<Card> hand, bool dealerFaceup = false)
        {
            var card = Deck.Cards[deckIndex];
            hand.Add(card);
            if (dealerFaceup)
                this.DealerFaceupCard = card;
            deckIndex++;
        }
    }
}