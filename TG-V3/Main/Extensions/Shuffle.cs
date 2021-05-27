using TG_V3.Util;

namespace TG_V3.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Fisher-Yates shuffle. [Complexity: O(n)]
        /// </summary>
        public static void Shuffle<T>(this T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                var j = GlobalRandom.Next(i + 1);
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }
    }
}
