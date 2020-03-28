using System;
using System.Linq;

namespace Util
{
    public static class Global
    {
        public static readonly int[] NNHiddenLayers = new int[] { 2, 2 };
        public static readonly bool[] NNBiases = new bool[] { false, false };
        public const double GeneticRangeLow = 0;
        public const double GeneticRangeHigh = 21;
        public const int FractionDigits = 4;
        private const double Log10 = 3.3219280948873624; // na base 2
        public static int GenomeBits
        {
            get => (int)Math.Ceiling(Math.Log(GeneticRangeHigh - GeneticRangeLow + 1, 2) + Log10 * Math.Log(FractionDigits, 2));
        }
        // sem contar biases (adicionar depois)
        public static int WeightCount
        {
            get
            {
                var total = 0;
                var weights = new ConcactableArray<int>() {3, NNHiddenLayers, 1};
                for (int i = 0; i < weights.Length - 1; i++)
                    total += weights[i] * weights[i + 1];
                return total;
            }
        }
    }
}