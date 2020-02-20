using System;
using SM;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            Card a = new dynamic[] {FaceValue.Ace, Suit.Spades};
            Console.WriteLine(a.Name);
        }
    }
}
