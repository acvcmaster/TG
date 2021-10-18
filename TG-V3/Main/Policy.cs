using System;
using TG_V3.Extensions;

namespace TG_V3
{
    partial class Learning
    {
        public static char[,] GetOptimalPolicy(double[,,] table, int x, int y, int[] moves)
        {
            char[,] result = new char[x, y];
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                {
                    switch (moves.MaxOver(item => table[i, j, item]))
                    {
                        case 0:
                            result[i, j] = 'S';
                            break;
                        case 1:
                            result[i, j] = 'H';
                            break;
                        case 2:
                            result[i, j] = 'D';
                            break;
                        case 3:
                            result[i, j] = 'P';
                            break;
                    }
                }

            return result;
        }

        public static void PrintPolicy(char[,] policy, int x, int y)
        {
            var foreground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Black;
            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    var move = policy[i, j];
                    var color = Console.BackgroundColor;

                    switch (move)
                    {
                        case 'S':
                            Console.BackgroundColor = ConsoleColor.Red;
                            break;
                        case 'H':
                            Console.BackgroundColor = ConsoleColor.Green;
                            break;
                        case 'D':
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            break;
                        case 'P':
                            Console.BackgroundColor = ConsoleColor.Magenta;
                            break;
                    }
                    Console.Write($" {move} ");
                    Console.BackgroundColor = color;
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = foreground;
            Console.WriteLine("\n\n");
        }

        #region Baseline
        public static double[,,] GetBaselineTable(QLearningTable table)
        {
            switch (table)
            {
                case QLearningTable.HardHands:
                    return GetTableFromPolicy(GetHardHandsBaselinePolicy(), 10, 16, 3);
                case QLearningTable.SoftHands:
                    return GetTableFromPolicy(GetSoftHandsBaselinePolicy(), 10, 8, 3);
                case QLearningTable.Split:
                    return GetTableFromPolicy(GetSplitBaselinePolicy(), 10, 10, 4);
                default:
                    return null;
            }
        }

        public static char[,] GetHardHandsBaselinePolicy()
        {
            return Transpose(new char[16, 10]
            {
                {'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S'},
                {'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S'},
                {'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S'},
                {'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S'},
                {'S', 'S', 'S', 'S', 'S', 'H', 'H', 'H', 'H', 'H'},
                {'S', 'S', 'S', 'S', 'S', 'H', 'H', 'H', 'H', 'H'},
                {'S', 'S', 'S', 'S', 'S', 'H', 'H', 'H', 'H', 'H'},
                {'S', 'S', 'S', 'S', 'S', 'H', 'H', 'H', 'H', 'H'},
                {'H', 'H', 'S', 'S', 'S', 'H', 'H', 'H', 'H', 'H'},
                {'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D'},
                {'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'H', 'H'},
                {'H', 'D', 'D', 'D', 'D', 'H', 'H', 'H', 'H', 'H'},
                {'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H'},
                {'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H'},
                {'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H'},
                {'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H', 'H'},
            }, 10, 16);
        }

        public static char[,] GetSoftHandsBaselinePolicy()
        {
            return Transpose(new char[8, 10]
            {
                {'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S'},
                {'S', 'S', 'S', 'S', 'D', 'S', 'S', 'S', 'S', 'S'},
                {'D', 'D', 'D', 'D', 'D', 'S', 'S', 'H', 'H', 'H'},
                {'H', 'D', 'D', 'D', 'D', 'H', 'H', 'H', 'H', 'H'},
                {'H', 'H', 'D', 'D', 'D', 'H', 'H', 'H', 'H', 'H'},
                {'H', 'H', 'D', 'D', 'D', 'H', 'H', 'H', 'H', 'H'},
                {'H', 'H', 'H', 'D', 'D', 'H', 'H', 'H', 'H', 'H'},
                {'H', 'H', 'H', 'D', 'D', 'H', 'H', 'H', 'H', 'H'},
            }, 10, 8);
        }

        public static char[,] GetSplitBaselinePolicy()
        {
            return Transpose(new char[10, 10]
            {
                {'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P'},
                {'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', 'S'},
                {'P', 'P', 'P', 'P', 'P', 'S', 'P', 'P', 'S', 'S'},
                {'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P'},
                {'P', 'P', 'P', 'P', 'P', 'P', 'H', 'H', 'H', 'H'},
                {'P', 'P', 'P', 'P', 'P', 'H', 'H', 'H', 'H', 'H'},
                {'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'H', 'H'},
                {'H', 'H', 'H', 'P', 'P', 'H', 'H', 'H', 'H', 'H'},
                {'P', 'P', 'P', 'P', 'P', 'P', 'H', 'H', 'H', 'H'},
                {'P', 'P', 'P', 'P', 'P', 'P', 'H', 'H', 'H', 'H'},
            }, 10, 10);
        }

        public static double[,,] GetTableFromPolicy(char[,] policy, int x, int y, int z)
        {
            var result = new double[x, y, z];

            for (var i = 0; i < x; i++)
                for (var j = 0; j < y; j++)
                    for (var k = 0; k < z; k++)
                        result[i, j, k] = k == GetNumberFromChar(policy[i, j]) ? 1 : 0;

            return result;
        }
        #endregion

        public static double GetNumberFromChar(char move)
        {
            // Moves
            // 0 - Stand
            // 1 - Hit
            // 2 - Doubledown
            // 3 - Split

            switch (move)
            {
                case 'S':
                    return 0;
                case 'H':
                    return 1;
                case 'D':
                    return 2;
                case 'P':
                    return 3;
                default:
                    return 0;
            }
        }
        public static T[,] Transpose<T>(T[,] array, int x, int y)
        {
            var result = new T[x, y];

            for (var i = 0; i < x; i++)
                for (var j = 0; j < y; j++)
                {
                    result[i, j] = array[j, i];
                }

            return result;
        }


        public static char[] GetValidatedPolices(string data)
        {
            var STRATEGY_LENGTH = 340;
            if (!string.IsNullOrWhiteSpace(data))
            {
                var cells = data.Split(',');
                var result = new char[STRATEGY_LENGTH];

                if (cells.Length == STRATEGY_LENGTH)
                {
                    for (int i = 0; i < cells.Length; i++)
                    {
                        if (cells[i].Length == 1)
                            result[i] = cells[i][0];
                        else
                            throw new Exception($"Invalid policy data: wrong move length (at cell {i}).");
                    }

                    return result;
                }
                else
                    throw new Exception($"Invalid policy data: wrong cell number (found {cells.Length}).");
            }
            else
                throw new Exception("Invalid policy data: no data found.");
        }
    }
}