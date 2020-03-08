using System;
using System.Threading;

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
    }
}