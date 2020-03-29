using System;
using Util;

namespace Runner
{
    public partial class Program
    {
        static void TestRandomWeights()
        {
            int weights = Global.WeightCount;
            for (int i = 1; ; i++)
            {
                Console.WriteLine($"Diagram {i}");
                double[] random = new double[weights];
                for (int j = 0; j < weights; j++)
                    random[j] = StaticRandom.NextDouble(Global.GeneticRangeLow, Global.GeneticRangeHigh);

                foreach (var val in random)
                    Console.Write($"{val} ");
                Console.WriteLine();
                Diagrammer.Generate(random);
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}