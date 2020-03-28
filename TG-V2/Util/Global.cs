using System;
using System.Linq;

namespace Util
{
    public static class Global
    {
        public const int Population = 250;
        public const int Games = 100000;
        public const float Crossover = 0.5f;
        public const int MaxGenerations = 100;
        public const float Mutation = 0.4f;
        public const int Parallelism = 16;
        public static readonly int[] NNHiddenLayers = new int[] { 2, 2 };
        public static readonly bool[] NNBiases = new bool[] { false, false }; // TODO: Ainda não implementado
        public const double GeneticRangeLow = 0;
        public const double GeneticRangeHigh = 21;
        public const int FractionDigits = 4;
        public static int GenomeBits
        {
            get => (int)Math.Ceiling(Math.Log(GeneticRangeHigh * Math.Pow(10, FractionDigits), 2)) + 1;
        }
        // sem contar biases (adicionar depois)
        public static int WeightCount
        {
            get
            {
                var total = 0;
                var weights = new ConcactableArray<int>() { 3, NNHiddenLayers, 1 };
                for (int i = 0; i < weights.Length - 1; i++)
                    total += weights[i] * weights[i + 1];
                return total;
            }
        }
    }
}