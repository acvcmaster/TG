using System;
using System.Threading;
using Util.Models;

namespace Util
{
    public static class StaticRandom
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static int Next(int max)
        {
            return random.Value.Next(max);
        }

        public static double NextDouble(double min, double max)
        {
            return min + (max - min) * random.Value.NextDouble();
        }

        public static BlackjackMove NextMove(bool includeSplit)
        {
            return (BlackjackMove)Next(includeSplit ? 4 : 3);
        }

        public static T RandomElement<T>(this T[] @array)
        {
            var index = Next(@array.Length);
            return @array[index];
        }
    }
}