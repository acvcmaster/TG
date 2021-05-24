using System.Collections.Generic;
using System.Linq;

namespace TG_V3.Blackjack
{
    public class Game
    {
        // Starts a new game
        public Game(Deck deck)
        {
            // Distribute cards
            PlayerHand = new Hand();
            DealerHand = new Hand();

            PlayerHand.Add(deck.Pop());
            PlayerHand.Add(deck.Pop());
            DealerHand.Add(deck.Pop());

            if (PlayerBlackjack)
            {
                Final = true;
                Reward = 1.5;
            }
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

                    return (first?.BlackjackValue == 10 && first?.FaceValue != FaceValue.Ten && second?.FaceValue == FaceValue.Ace)
                     || (second?.BlackjackValue == 10 && second?.FaceValue != FaceValue.Ten && first?.FaceValue == FaceValue.Ace);
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

                    return (first?.BlackjackValue == 10 && first?.FaceValue != FaceValue.Ten && second?.FaceValue == FaceValue.Ace)
                     || (second?.BlackjackValue == 10 && second?.FaceValue != FaceValue.Ten && first?.FaceValue == FaceValue.Ace);
                }

                return false;
            }
        }

        public Game Stand(Deck deck)
        {
            var game = new Game(this);

            if (!game.Final)
            {
                // dealer buys until soft 17
                while (game.DealerHand.Sum < 17)
                    game.DealerHand.Add(deck.Pop());

                game.Reward = 1;

                if (game.DealerBlackjack || (game.PlayerHand.Sum < game.DealerHand.Sum && game.DealerHand.Sum <= 21))
                    game.Reward = -1;
                else if (game.PlayerHand.Sum == game.DealerHand.Sum)
                    game.Reward = 0;

                game.Final = true;
            }

            return game;
        }

        public Game Hit(Deck deck)
        {
            var game = new Game(this);

            if (!game.Final)
            {
                game.PlayerHand.Add(deck.Pop());
                if (game.PlayerHand.Sum > 21)
                {
                    game.Final = true;
                    game.Reward = -1;
                }
            }

            return game;
        }

        public Game DoubleDown(Deck deck)
        {
            var game = new Game(this);

            if (!game.Final)
            {
                game.PlayerHand.Add(deck.Pop());
                if (game.PlayerHand.Sum > 21)
                {
                    game.Final = true;
                    game.Reward = -2;
                }
                else
                {
                    game = game.Stand(deck);
                    game.Reward *= 2;
                }
            }

            return game;
        }

        public IEnumerable<Game> Split(Deck deck)
        {
            var game = new Game(this);

            if (!game.Final)
            {

            }

            return null;
        }
    }
}