using System;
using System.Drawing;
using Util.Models;

namespace Runner
{
    public class Diagrammer
    {
        public static IDiagram Generate(BlackjackChromosome chromosome)
        {
            var values = chromosome.Moves;
            for (int i = 0; i < 160; i++)
            {
                if (i != 0 && i % 10 == 0)
                    Console.WriteLine();

                switch (values[i])
                {
                    case BlackjackMove.Hit:
                        Console.Write("H ");
                        break;
                    case BlackjackMove.Stand:
                        Console.Write("S ");
                        break;
                    case BlackjackMove.DoubleDown:
                        Console.Write("D ");
                        break;
                    case BlackjackMove.Split:
                        Console.Write("P ");
                        break;
                }
            }
            Console.WriteLine();
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