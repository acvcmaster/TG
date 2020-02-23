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
        public int PlayerHandIndex = 0;
        public int DealerHandIndex = 0;
        public Blackjack(Deck deck, BlackjackStrategy strategy)
        {
            Deck = deck;
            CurrentStrategy = strategy;
        }
        public BlackjackStrategy CurrentStrategy { get; }
        public float Play()
        {
            float profit = 1f;
            GameState state = GameState.InitialState;
        process_states:
            switch (state)
            {
                case GameState.InitialState:
                    GiveCard(PlayerHand, ref PlayerHandIndex);
                    GiveCard(PlayerHand, ref PlayerHandIndex);
                    GiveCard(DealerHand, ref DealerHandIndex, true);
                    Transition(ref state, GameState.PlayerTurn);
                    if (CheckBlackjack(PlayerHand))
                        Transition(ref state, GameState.PlayerBlackjack);
                    break;
                case GameState.PlayerTurn:
                    var move = GetPlayerMove();

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
                    if (GetSum(PlayerHand, PlayerHandIndex) > 21)
                        Transition(ref state, GameState.Stand);
                    else Transition(ref state, GameState.PlayerTurn);
                    break;
                case GameState.Stand:
                    while (GetSum(DealerHand, DealerHandIndex) < 17) // Dealer stands on soft 17
                        GiveCard(DealerHand, ref DealerHandIndex);

                    int playerSum = GetSum(PlayerHand, PlayerHandIndex);
                    int dealerSum = GetSum(DealerHand, DealerHandIndex);

                    if (playerSum > 21 || CheckBlackjack(DealerHand) || (playerSum < dealerSum && dealerSum <= 21)) { profit = -profit; }
                    else if (playerSum == dealerSum) { profit = 0; }
#if INTERACTIVE
                    Console.Write("Player: ");
                    for (int i = 0; i < PlayerHandIndex; i++)
                        Console.Write(PlayerHand[i].Name + ", ");
                    Console.WriteLine();

                    Console.Write("Dealer: ");
                    for (int i = 0; i < DealerHandIndex; i++)
                        Console.Write(DealerHand[i].Name + ", ");
                    Console.WriteLine();
#endif

                    Transition(ref state, GameState.FinalState);
                    break;
                case GameState.DoubleDown:
                    GiveCard(PlayerHand, ref PlayerHandIndex);
                    profit = 2;
                    Transition(ref state, GameState.Stand);
                    break;
                case GameState.PlayerBlackjack:
                    GiveCard(DealerHand, ref DealerHandIndex);
                    profit = CheckBlackjack(DealerHand) ? 0 : 1.5f;
                    Transition(ref state, GameState.FinalState);
                    break;
                case GameState.Split:
                    profit = PlaySplit();
                    Transition(ref state, GameState.FinalState);
                    break;
                default:
#if INTERACTIVE
                    Console.WriteLine($"profit = {profit}");
                    Console.WriteLine("---------------------");
                    Console.WriteLine("\n\n\n");
#endif
                    return profit;
            }
            goto process_states;
        }

        public float PlaySplit()
        {
            return 0;
        }

        private BlackjackMove GetPlayerMove()
        {
            var move = CurrentStrategy(this);
            if (move == BlackjackMove.Split)
            {
                if (PlayerHandIndex == 2 && PlayerHand[0].BlackjackValue == PlayerHand[1].BlackjackValue)
                    return BlackjackMove.Split;
                else return BlackjackMove.Stand;
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
            int aces = 0;
            for (int i = 0; i < length; i++)
            {
                Card card = hand[i];
                if (card.FaceValue != FaceValue.Ace)
                    sum += card.BlackjackValue;
                else aces++;
            }

            if (aces > 0)
                if (sum + aces <= 11)
                    aces += 10;

            return sum + aces;
        }
    }
}