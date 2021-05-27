using System.Collections.Generic;

namespace TG_V3.Extensions
{
    public static partial class Extensions
    {
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
