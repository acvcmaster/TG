using System.Collections.Generic;
using System.Linq;

namespace TG_V3.Blackjack
{
    public class Game
    {
        // Starts a new game
        public Game(ref Deck deck)
        {
            // Distribute cards
            PlayerHand = new Hand();
            DealerHand = new Hand();

            PlayerHand.Add(deck.Pop());
            PlayerHand.Add(deck.Pop());

            // pre-determining the dealer's hand
            // dealer buys until soft 17
            while (DealerHand.Sum < 17)
                DealerHand.Add(deck.Pop());
            
            CheckNatural();
        }

        // Clones the previous game and returns a new object
        public Game(Game previous)
        {
            PlayerHand = new Hand(previous.PlayerHand);
            DealerHand = new Hand(previous.DealerHand);
            Final = previous.Final;
            Reward = previous.Reward;
        }

        public Hand PlayerHand { get; set; }
        public Hand DealerHand { get; set; }
        public bool Final { get; set; }
        public double Reward { get; set; }
        public Card? DealerFaceUpCard => DealerHand?.Cards?.FirstOrDefault();

        public bool DealerBlackjack
        {
            get
            {
                if (DealerHand?.Count == 2)
                {
                    var first = DealerHand?.CardAt(0);
                    var second = DealerHand?.CardAt(1);

                    return (first?.BlackjackValue == 10 && second?.FaceValue == FaceValue.Ace)
                     || (second?.BlackjackValue == 10 && first?.FaceValue == FaceValue.Ace);
                }

                return false;
            }
        }

        public bool PlayerBlackjack
        {
            get
            {
                if (PlayerHand?.Count == 2)
                {
                    var first = PlayerHand?.CardAt(0);
                    var second = PlayerHand?.CardAt(1);

                    return (first?.BlackjackValue == 10 && second?.FaceValue == FaceValue.Ace)
                     || (second?.BlackjackValue == 10 && first?.FaceValue == FaceValue.Ace);
                }

                return false;
            }
        }

        public bool CanSplit => !Final &&
            PlayerHand?.Count == 2 &&
            PlayerHand.CardAt(0).Value.FaceValue ==
            PlayerHand.CardAt(1).Value.FaceValue;

        public Game Stand(ref Deck deck, bool doubledown = false)
        {
            var game = new Game(this);

            if (!game.Final)
            {
                var player = game.PlayerHand.Sum;
                var dealer = game.DealerHand.Sum;

                if (player > 21)
                    game.Reward = -1;
                else
                {
                    if (dealer > 21)
                        game.Reward = 1;
                    else
                    {
                        if (player < dealer)
                            game.Reward = -1;
                        else if (player > dealer)
                            game.Reward = 1;
                        else game.Reward = 0;
                    }
                }

                game.Reward *= doubledown ? 2 : 1;
                game.Final = true;
            }

            return game;
        }

        public Game Hit(ref Deck deck, bool doubledown = false)
        {
            var game = new Game(this);

            if (!game.Final)
            {
                game.PlayerHand.Add(deck.Pop());

                if (game.PlayerHand.Sum == 21)
                {
                    return game.Stand(ref deck, doubledown);
                }
                else if (game.PlayerHand.Sum > 21)
                {
                    game.Final = true;
                    game.Reward = !doubledown ? -1 : -2;
                }
            }

            return game;
        }

        public Game DoubleDown(ref Deck deck)
        {
            var game = new Game(this);

            if (!game.Final)
            {
                game = game.Hit(ref deck, true);
                game = game.Stand(ref deck, true);
            }

            return game;
        }

        public IEnumerable<Game> Split(ref Deck deck)
        {
            var results = new List<Game>();

            if (!Final && CanSplit)
            {
                var left = new Game(this);
                var right = new Game(this);

                left?.PlayerHand?.Replace(1, deck.Pop());
                right?.PlayerHand?.Replace(0, deck.Pop());

                left?.CheckNatural();
                right?.CheckNatural();

                results.Add(left);
                results.Add(right);
            }

            return results;
        }

        public void CheckNatural()
        {
            if (PlayerBlackjack)
            {
                Final = true;
                Reward = DealerBlackjack ? 0 : 1.5;
            }
        }
    }
}