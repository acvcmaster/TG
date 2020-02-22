using System;

namespace SM
{
    public delegate BlackjackMove BlackjackStrategy(Blackjack game);
    public class Blackjack
    {
        private Deck Deck { get; }
        public bool Ended = false;
        public Blackjack(Deck deck, BlackjackStrategy strategy)
        {
            Deck = deck;
            CurrentStrategy = strategy;
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
    }
}