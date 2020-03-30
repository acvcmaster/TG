using System.Collections.Generic;
using System.Linq;

namespace Util
{
    public static class Extensions
    {
        /// <summary>
        /// Fisher-Yates shuffle. [Complexity: O(n)]
        /// </summary>
        public static void Shuffle<T>(this T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                var j = StaticRandom.Next(i + 1);
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        /// <summary>
        /// Print all elements of the array to a string.
        /// </summary>
        /// <param name="array">The array</param>
        /// <typeparam name="T">The array type</typeparam>
        /// <returns>A string containing all elements</returns>
        public static string Print<T>(this T[] array)
        {
            string result = string.Empty;
            for (int i = 0; i < array.Length; i++)
            {
                result += $"{array[i].ToString()}";
                if (i != array.Length - 1)
                    result += " ";
            }
            return result;
        }

        /// <summary>
        /// Creates a deep copy of the array.
        /// </summary>
        /// <param name="array">The array</param>
        /// <typeparam name="T">The array type</typeparam>
        /// <returns>A new array that is a carbon copy of the previous.</returns>
        public static T[] Clone<T>(this T[] array)
        {
            return new List<T>(array).ToArray();
        }
    }
}
