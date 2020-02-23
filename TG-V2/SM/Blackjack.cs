using System;
using System.Linq;
using System.Collections.Generic;

namespace SM
{
    public class Blackjack
    {
        private Deck Deck { get; }
        public Card DealerFaceupCard { get; set; }
        public Card[] PlayerHand = new Card[21];
        public Card[] PlayerSecondHand = new Card[21];
        public Card[] DealerHand = new Card[21];
        private int deckIndex = 0;
        private int playerHandIndex = 0;
        // private int playerSecondHandIndex = 0;
        private float profit = 1;
        private float profit2 = 0;
        private bool split = false;
        private bool secondHandTurn = false;
        private int dealerHandIndex = 0;
        public Blackjack(Deck deck, BlackjackStrategy strategy)
        {
            Deck = deck;
            CurrentStrategy = strategy;
        }
        public BlackjackStrategy CurrentStrategy { get; }
        public float Play()
        {
            GameState state = GameState.InitialState;
        process_states:
            switch (state)
            {
                case GameState.InitialState:
                    GiveCard(PlayerHand, ref playerHandIndex);
                    GiveCard(PlayerHand, ref playerHandIndex);
                    GiveCard(DealerHand, ref dealerHandIndex, true);
                    Transition(ref state, GameState.PlayerTurn);
                    if (CheckBlackjack(PlayerHand))
                        Transition(ref state, GameState.PlayerBlackjack);
                    break;
                case GameState.PlayerTurn:
                    var move = GetPlayerMove();

                    switch (move)
                    {
                        case BlackjackMove.Stand:
                            Transition(ref state, GameState.Stand);
                            break;
                        case BlackjackMove.DoubleDown:
                            Transition(ref state, GameState.DoubleDown);
                            break;
                    }

                    break;
                case GameState.Hit:
                    break;
                case GameState.Stand:
                    Transition(ref state, GameState.FinalState);
                    break;
                case GameState.DoubleDown:
                    GiveCard(PlayerHand, ref playerHandIndex);
                    profit = 2;
                    Transition(ref state, GameState.Stand);
                    break;
                case GameState.PlayerBlackjack:
                    GiveCard(DealerHand, ref dealerHandIndex);
                    profit = CheckBlackjack(DealerHand) ? 0 : 1.5f;
                    Transition(ref state, GameState.FinalState);
                    break;
                default:
                    return profit;
            }
            goto process_states;
        }

        private BlackjackMove GetPlayerMove()
        {
            var move = CurrentStrategy(this);
            if (move == BlackjackMove.Split)
            {
                // if (playerHandIndex == 2 && PlayerHand[0].BlackjackValue == PlayerHand[1].BlackjackValue)
                //     return BlackjackMove.Split;
                // else
                return BlackjackMove.Stand;
            }

            return move;
        }

        private void Transition(ref GameState state, GameState next) { state = next; }
        private void GiveCard(Card[] hand, ref int index, bool dealerFaceup = false)
        {
            var card = Deck.Cards[deckIndex];
            hand[index] = card;
            if (dealerFaceup)
                this.DealerFaceupCard = card;
            deckIndex++;
            index++;
        }
        private static bool CheckBlackjack(Card[] hand)
        {
            return hand[0].FaceValue == FaceValue.Ace && hand[1].BlackjackValue == 10 ||
                hand[1].FaceValue == FaceValue.Ace && hand[0].BlackjackValue == 10;
        }

        public static int GetSum(Card[] hand, int length)
        {
            int sum = 0;
            bool ace = false;
            for (int i = 0; i < length; i++)
            {
                Card card = hand[i];
                sum += card.BlackjackValue;
                if (card.FaceValue == FaceValue.Ace && !ace)
                {
                    sum += 10;
                    ace = true;
                }
            }
            return sum;
        }
    }
}