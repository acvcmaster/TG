using System;
using GeneticSharp.Domain.Chromosomes;
using Util.Models;

namespace Runner
{
    public partial class Program
    {
        static void TestChromosomes()
        {
            string guid = Guid.NewGuid().ToString();
            var a = new BlackjackChromosome();
            for (int i = 0; i < 340; i++)
            {
                var gene = BlackjackMove.Hit;
                if (i == 0)
                    gene = BlackjackMove.Stand;
                else if (i == 339)
                    gene = BlackjackMove.Stand;

                a.ReplaceGene(i, new Gene(gene));
            }

            var diagram = Diagrammer.Generate(a, null, null, -1);
            Diagrammer.Save(diagram, 0, guid);
        }
    }
}