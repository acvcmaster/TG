using Util;
using Util.Models;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace Runner
{
    public class Diagrammer
    {
        public static IDiagram Generate(double[] values)
        {
            StaticNN.SetWeights(values);
            List<Card[]> Hands = new List<Card[]>();
            for (FaceValue value = FaceValue.Two; value <= FaceValue.Ten; value++)
                Hands.Add(new Card[] { new Card(FaceValue.Three, Suit.Spades), new Card(value, Suit.Spades) });

            for (FaceValue value = FaceValue.Three; value <= FaceValue.Ten; value++)
                Hands.Add(new Card[] { new Card(FaceValue.Ten, Suit.Spades), new Card(value, Suit.Spades) });
            
            Hands.Reverse();
            System.Console.WriteLine();

            foreach (var hand in Hands)
            {
                var faceValues = new ConcactableArray<FaceValue>() { Enumerable.Range(2, 9).Select(item => (FaceValue)item), FaceValue.Ace};
                foreach (var value in faceValues)
                {
                    Card dealerCard = new Card(value, Suit.Spades);
                    var move = StaticNN.Compute(new BlackjackInformation(dealerCard, hand, 2, false));
                    string movestring = null;
                    switch (move)
                    {
                        case BlackjackMove.Hit:
                            movestring = "H";
                            break;
                        case BlackjackMove.Stand:
                            movestring = "S";
                            break;
                        case BlackjackMove.DoubleDown:
                            movestring = "D";
                            break;
                        case BlackjackMove.Split:
                            movestring = "P";
                            break;
                    }
                    var sum = BlackjackStatic.GetSum(hand, 2);
                    var dc = (int)value;
                    System.Console.Write(movestring + " ");
                }
                System.Console.WriteLine();
            }
            return null;
        }
        public static void Save(IDiagram diagram, string file)
        {

        }
    }

    public interface IDiagram
    {
        Image File { get; set; }
    }
}