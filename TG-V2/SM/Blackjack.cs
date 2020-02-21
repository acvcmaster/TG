using System;
using Util;

namespace SM
{
    public enum GameState : int
    {
        InitialState = 0,
        FinalState
    }
    public class Blackjack
    {
        private Deck Deck { get; }
        public bool Ended = false;
        public Blackjack(Deck deck)
        {
            Deck = deck;
        }

        public void Play()
        {
            if (Ended)
                return;

            GameState state = GameState.InitialState;
        process_states:
            switch (state)
            {
                case GameState.InitialState:
                    Deck.Shuffle();
                    Transition(ref state, GameState.FinalState);
                    break;
                default:
                    Ended = true;
                    return;
            }
            goto process_states;
        }

        private void Transition(ref GameState state, GameState next) { state = next; }
    }
}