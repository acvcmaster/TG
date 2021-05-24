using System;

namespace TG_V3.Util
{
    public static class GlobalRandom
    {
        public static Random InnerRandom { get; set; } = new Random();

        public static int Next(int maxValue)
        {
            return InnerRandom.Next(maxValue);
        }
    }
}