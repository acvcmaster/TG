using System;
using TG_V3.Blackjack;
using TG_V3.Extensions;

namespace TG_V3
{
    class Program
    {
        static void Main(string[] args)
        {
            Deck deck = new Deck(4);
            Game game = new Game(deck);


            game = game.DoubleDown(deck);
        }
    }
}
