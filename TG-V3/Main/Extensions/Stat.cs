using System;
using System.Collections.Generic;
using System.Linq;

namespace TG_V3.Extensions
{
    public static partial class Extensions
    {
        public static double Stdev(this IEnumerable<double> list)
        {
            if (list.Any())
            {
                double average = list.Average();
                double sum = list.Sum(d => Math.Pow(d - average, 2));
                return Math.Sqrt((sum) / (list.Count() - 1));
            }

            return 0;
        }

        public static double Sterr(this IEnumerable<double> list)
        {
            double count = list?.Count() ?? 1;
            return Stdev(list) / count;
        }
    }
}
