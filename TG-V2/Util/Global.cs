using System.Globalization;

namespace Util
{
    public static class Global
    {
        public const int Population = 250;
        public const int Games = 100000;
        public const float Crossover = 0.5f; // cada filho contém aproximadamente metade do pai e da mãe
        public const int MaxGenerations = 1000;
        public const float Mutation = 0;
        public const int Parallelism = 16;

        public static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
    }
}