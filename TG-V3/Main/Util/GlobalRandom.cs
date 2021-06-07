using System;
using System.Threading;

namespace TG_V3.Util
{
    public static class GlobalRandom
    {
        static int seed = Environment.TickCount;
        static readonly ThreadLocal<Random> LocalRandom = new ThreadLocal<Random>(() =>
            new Random(Interlocked.Increment(ref seed)));

        static Random InnerRandom
        {
            get
            {
                return LocalRandom.Value;
            }
        }

        public static int Next(int maxValue)
        {
            return InnerRandom.Next(maxValue);
        }

        public static double NextDouble()
        {
            return InnerRandom.NextDouble();
        }

        public static double[,,] NextTable(int x, int y, int z)
        {
            var result = new double[x, y, z];

            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                    for (int k = 0; k < z; k++)
                    {
                        result[i, j, k] = NextDouble();
                    }

            return result;
        }
    }
}