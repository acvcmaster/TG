namespace TG_V3.Extensions
{
    public static partial class Extensions
    {
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
    }
}
