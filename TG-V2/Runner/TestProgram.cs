using System;
using System.Linq;
using System.Threading.Tasks;
using Util;
using Util.Models;

namespace Runner
{
    public partial class Program
    {
        static void TestBaselineStrategy()
        {
            Console.WriteLine("Testing optimal stategy.");
            var guid = GuidProvider.NewGuid(false);
            Console.Write("Generating decks.. ");
            RandomDecks.GenerateRandomDecks();
            Console.WriteLine("Done.");

            var HardHands = new ConcactableArray<char>()
            {
                'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S',
                'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S',
                'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S',
                'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S',
                'S', 'S', 'S', 'S', 'S', 'H', 'H', 'H', 'H', 'H',
                'S', 'S', 'S', 'S', 'S', 'H', 'H', 'H', 'H', 'H',
                'S', 'S', 'S', 'S', 'S', 'H', 'H', 'H', 'H', 'H',
                'S', 'S', 'S', 'S', 'S', 'H', 'H', 'H', 'H', 'H',
                'H', 'H', 'S', 'S', 'S', 'H', 'H', 'H', 'H', 'H',
                'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D',
                'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'H', 'H',
                'H', 'D', 'D', 'D', 'D', 'H', 'H', 'H', 'H', 'H',
                'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H',
                'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H',
                'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H',
                'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H'
            };

            var softHands = new ConcactableArray<char>()
            {
                'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S',
                'S', 'S', 'S', 'S', 'D', 'S', 'S', 'S', 'S', 'S',
                'D', 'D', 'D', 'D', 'D', 'S', 'S', 'H', 'H', 'H',
                'H', 'D', 'D', 'D', 'D', 'H', 'H', 'H', 'H', 'H',
                'H', 'H', 'D', 'D', 'D', 'H', 'H', 'H', 'H', 'H',
                'H', 'H', 'D', 'D', 'D', 'H', 'H', 'H', 'H', 'H',
                'H', 'H', 'H', 'D', 'D', 'H', 'H', 'H', 'H', 'H',
                'H', 'H', 'H', 'D', 'D', 'H', 'H', 'H', 'H', 'H'
            };

            var pairs = new ConcactableArray<char>()
            {
                'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P',
                'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S',
                'P', 'P', 'P', 'P', 'P', 'S', 'P', 'P', 'S', 'S',
                'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P',
                'P', 'P', 'P', 'P', 'P', 'P', 'H', 'H', 'H', 'H',
                'P', 'P', 'P', 'P', 'P', 'H', 'H', 'H', 'H', 'H',
                'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'H', 'H',
                'H', 'H', 'H', 'P', 'P', 'H', 'H', 'H', 'H', 'H',
                'P', 'P', 'P', 'P', 'P', 'P', 'H', 'H', 'H', 'H',
                'P', 'P', 'P', 'P', 'P', 'P', 'H', 'H', 'H', 'H'
            };

            var tables = new ConcactableArray<char> { HardHands, softHands, pairs };
            var paragonChromosome = new BlackjackChromosome(tables.Select(item =>
            {
                switch (item)
                {
                    case 'S':
                        return BlackjackMove.Stand;
                    case 'H':
                        return BlackjackMove.Hit;
                    case 'D':
                        return BlackjackMove.DoubleDown;
                    case 'P':
                        return BlackjackMove.Split;
                }
                return BlackjackMove.Stand;
            }).ToArray());

            Console.Write("Evaluating fitness.. ");
            BlackjackFitness fitnessCalculator = new BlackjackFitness();
            double fitness = fitnessCalculator.Evaluate(paragonChromosome);
            Console.WriteLine("Done.");

            Console.Write("Generating diagram.. ");
            var diagram = Diagrammer.Generate(paragonChromosome, null, fitness, 0);
            Diagrammer.Save(diagram, 0, guid);
            Console.WriteLine("Done.");
        }
    }
}