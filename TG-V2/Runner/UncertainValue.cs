using System;
using System.Collections.Generic;
using System.Linq;

namespace Runner
{
    public static class StatExtensions
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
            return Stdev(list) / Math.Sqrt(count);
        }

        /// <summary>
        /// Expressed as a percentage.
        /// </summary>
        public static double CoeffVar(this IEnumerable<double> list)
        {
            double average = list.Average();
            return 100 * list.Stdev() / average;
        }
    }

    public class UncertainValue
    {
        public double Value { get; set; }
        public double Uncertainty { get; set; }
        public double RelativeUncertainty => 100 * Math.Abs(Uncertainty) / Math.Abs(Value);


        public override string ToString()
        {
            return $"{Value.ToString("0.#######")} ± {Uncertainty.ToString("0.#######")} ({(RelativeUncertainty).ToString("0.##")} %)";
        }
    }
}
