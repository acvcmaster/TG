using System;
using Util;
using Util.Models;

namespace SM
{
    public class Blackjack
    {
        private Deck Deck { get; }
        public Card DealerFaceupCard { get; set; }
        public Card[] PlayerHand = new Card[21];
        public Card[] PlayerSecondHand = new Card[21];
        public Card[] DealerHand = new Card[21];
        private int DeckIndex = 0;
        public int PlayerHandIndex = 0;
        public int DealerHandIndex = 0;
        public Blackjack(Deck deck, BlackjackStrategy strategy)
        {
            Deck = deck;
            CurrentStrategy = strategy;
        }
        public BlackjackStrategy CurrentStrategy { get; }
        public float Play(Card[] splitHand = null, int splitHandIndex = 0, bool IsSplit = false)
        {
            var PlayerHand = this.PlayerHand;
            var PlayerHandIndex = this.PlayerHandIndex;

            if (splitHand != null && IsSplit)
            {
                PlayerHand = splitHand;
                PlayerHandIndex = splitHandIndex;
            }
            float profit = 1f;
            GameState state = GameState.InitialState;
        process_states:
            switch (state)
            {
                case GameState.InitialState:
                    if (!IsSplit)
                    {
                        GiveCard(PlayerHand, ref PlayerHandIndex);
                        GiveCard(PlayerHand, ref PlayerHandIndex);
                        GiveCard(DealerHand, ref DealerHandIndex, true);
                    }

                    Transition(ref state, GameState.PlayerTurn);
                    if (BlackjackStatic.CheckBlackjack(PlayerHand))
                        Transition(ref state, GameState.PlayerBlackjack);
                    break;
                case GameState.PlayerTurn:
                    var move = GetPlayerMove(PlayerHand, PlayerHandIndex, IsSplit);

                    switch (move)
                    {
                        case BlackjackMove.Hit:
                            Transition(ref state, GameState.Hit);
                            break;
                        case BlackjackMove.Stand:
                            Transition(ref state, GameState.Stand);
                            break;
                        case BlackjackMove.DoubleDown:
                            Transition(ref state, GameState.DoubleDown);
                            break;
                        case BlackjackMove.Split:
                            Transition(ref state, GameState.Split);
                            break;
                    }
                    break;
                case GameState.Hit:
                    GiveCard(PlayerHand, ref PlayerHandIndex);
                    if (BlackjackStatic.GetSum(PlayerHand, PlayerHandIndex) > 21)
                        Transition(ref state, GameState.Bust);
                    else Transition(ref state, GameState.PlayerTurn);
                    break;
                case GameState.Stand:
                    while (BlackjackStatic.GetSum(DealerHand, DealerHandIndex) < 17) // Dealer stands on soft 17
                        GiveCard(DealerHand, ref DealerHandIndex);

                    int playerSum = BlackjackStatic.GetSum(PlayerHand, PlayerHandIndex);
                    int dealerSum = BlackjackStatic.GetSum(DealerHand, DealerHandIndex);

                    if (playerSum > 21 || BlackjackStatic.CheckBlackjack(DealerHand) || (playerSum < dealerSum && dealerSum <= 21)) { profit = -profit; }
                    else if (playerSum == dealerSum) { profit = 0; }

                    Transition(ref state, GameState.FinalState);
                    break;
                case GameState.DoubleDown:
                    GiveCard(PlayerHand, ref PlayerHandIndex);
                    profit = 2;
                    Transition(ref state, GameState.Stand);
                    break;
                case GameState.PlayerBlackjack:
                    GiveCard(DealerHand, ref DealerHandIndex);
                    profit = BlackjackStatic.CheckBlackjack(DealerHand) ? 0 : 1.5f;
                    Transition(ref state, GameState.FinalState);
                    break;
                case GameState.Split:
                    if (PlayerHandIndex == 2 && PlayerHand[0].BlackjackValue == PlayerHand[1].BlackjackValue)
                    {
                        profit = PlaySplit();
                        Transition(ref state, GameState.FinalState);
                    }
                    else throw new InvalidOperationException();
                    break;
                case GameState.Bust:
                    profit = -profit;
                    Transition(ref state, GameState.FinalState);
                    break;
                default:
                    return profit;
            }
            goto process_states;
        }

        public float PlaySplit()
        {
            Card[] PlayerHand1 = new Card[21];
            Card[] PlayerHand2 = new Card[21];

            int PlayerHand1Index = 1;
            int PlayerHand2Index = 1;

            PlayerHand1[0] = PlayerHand[0];
            PlayerHand2[0] = PlayerHand[1];

            GiveCard(PlayerHand1, ref PlayerHand1Index);
            GiveCard(PlayerHand2, ref PlayerHand2Index);

            return Play(PlayerHand1, PlayerHand1Index, true) + Play(PlayerHand2, PlayerHand2Index, true);
        }

        private BlackjackMove GetPlayerMove(Card[] PlayerHand, int PlayerHandIndex, bool IsSplit = false)
        {
            var move = CurrentStrategy(new BlackjackInformation(this, this.DealerFaceupCard, PlayerHand, PlayerHandIndex, IsSplit));
            return move;
        }

        private void Transition(ref GameState state, GameState next) { state = next; }
        private void GiveCard(Card[] hand, ref int index, bool dealerFaceup = false)
        {
            var card = Deck.Cards[DeckIndex];
            hand[index] = card;
            if (dealerFaceup)
                this.DealerFaceupCard = card;
            DeckIndex++;
            index++;
        }

        public float Lookahead(BlackjackMove move)
        {
            switch (move)
            {
                case BlackjackMove.Hit:
                    {
                        var playerHand = PlayerHand.Clone<Card>();
                        playerHand[PlayerHandIndex] = Deck.Cards[DeckIndex];
                        return BlackjackStatic.GetSum(playerHand, PlayerHandIndex + 1) > 21 ? -1 : 0;
                    }
                case BlackjackMove.Stand:
                    return LookaheadStand(1, move);
                case BlackjackMove.DoubleDown:
                    return LookaheadStand(2, move);
                case BlackjackMove.Split:
                    {
                        float profit = -1;

                        var playerHand1 = new Card[21];
                        var playerHand2 = new Card[21];

                        playerHand1[0] = PlayerHand[0];
                        playerHand1[1] = Deck.Cards[DeckIndex];

                        playerHand2[0] = PlayerHand[1];
                        playerHand2[1] = Deck.Cards[DeckIndex + 1];

                        profit += BlackjackStatic.CheckBlackjack(playerHand1) ? 1.5f : 0;
                        profit += BlackjackStatic.CheckBlackjack(playerHand2) ? 1.5f : 0;

                        return profit;
                    }
            }
            return 0;
        }

        public float LookaheadStand(float profit, BlackjackMove move)
        {
            var dealerHand = DealerHand.Clone<Card>();
            var dealerHandIndex = DealerHandIndex;
            var deckIndex = DeckIndex;

            while (BlackjackStatic.GetSum(dealerHand, dealerHandIndex) < 17) // Dealer stands on soft 17
            {
                dealerHand[dealerHandIndex] = Deck.Cards[deckIndex];
                dealerHandIndex++;
                deckIndex++;
            }

            int playerSum = BlackjackStatic.GetSum(PlayerHand, PlayerHandIndex);
            int dealerSum = BlackjackStatic.GetSum(dealerHand, dealerHandIndex);

            if (playerSum > 21 || BlackjackStatic.CheckBlackjack(dealerHand) || (playerSum < dealerSum && dealerSum <= 21)) { return -profit; }
            else if (playerSum == dealerSum) { return 0; }
            else return profit;
        }
    }
}