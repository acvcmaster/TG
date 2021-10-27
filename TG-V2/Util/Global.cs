using System.Globalization;

namespace Util
{
    public static class Global
    {
        public const int Population = 200;
        public const int Games = 150000;
        public const float Crossover = 0.5f; // cada filho contém aproximadamente metade do pai e da mãe
        public const int MaxGenerations = 300;
        public const float Mutation = 0;
        public const int Parallelism = 16;
        // public const int TournamentSize = 2;
        public static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
    }
}