using System;
using System.Collections.Generic;

namespace TG_V3.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Returns the element of a generic sequence having the maximum value when taken over a given function.
        /// </summary>
        /// <param name="enumerable">A generic sequence</param>
        /// <param name="fitness">A fitness function of sorts</param>
        /// <typeparam name="T">The sequence type</typeparam>
        /// <returns>The value that maximizes the fitness function.</returns>
        public static T MaxOver<T>(this IEnumerable<T> @enumerable, Func<T, double> fitness)
        {
            double maxValue = double.NegativeInfinity;
            T max = default(T);

            foreach (var item in enumerable)
            {
                var value = fitness(item);
                if (value > maxValue)
                {
                    max = item;
                    maxValue = value;
                }
            }

            return max;
        }
    }
}
