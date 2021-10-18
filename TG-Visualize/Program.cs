using System;
using System.IO;

namespace TG_Visualize
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = args?.Length > 0 ? args[0] : null;
            var name = !string.IsNullOrWhiteSpace(file) ? Path.GetFileName(file) : null;
            var nameWithoutExtension = !string.IsNullOrWhiteSpace(file) ? Path.GetFileNameWithoutExtension(file) : null;

            if (!string.IsNullOrWhiteSpace(file))
            {
                if (File.Exists(file))
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(file))
                        {
                            var data = reader.ReadToEnd();
                            var possibleMoves = data?.Split(',');
                            if (possibleMoves?.Length == 340)
                            {
                                var strategy = new char[340];
                                for (var i = 0; i < 340; i++)
                                {
                                    var move = possibleMoves[i];
                                    if (move.Length == 1)
                                    {
                                        strategy[i] = move[0];
                                        using (var writer = new StreamWriter($"{nameWithoutExtension}.html"))
                                        {
                                            var html = Diagrammer.Generate(strategy, name);
                                            writer.Write(html);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Invalid move detected on file '{name}'. Offending value: '{move}'.");
                                        break;
                                    }
                                }
                            }
                            else
                                Console.WriteLine($"Invalid stategy detected on file '{name}'. Invalid length ({possibleMoves?.Length ?? 0} but expected 340).");
                        }
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine($"The conversion problem threw an error: {error?.Message}");
                    }
                }
                else
                    Console.WriteLine($"No such file '{file}'.");
            }
            else
                Console.WriteLine("No file provided!");
        }
    }
}
