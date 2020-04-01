using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using GeneticSharp.Domain;
using Util;
using Util.Models;

namespace Runner
{
    public class Diagrammer
    {
        public delegate string LabelFunction(int rowIndex);
        public static string Generate(BlackjackChromosome chromosome, GeneticAlgorithm algorithm, double? bestFitness, int bestFitnessGeneration)
        {
            bestFitness = bestFitness.HasValue ? bestFitness.Value : double.NaN;
            StringBuilder html = null;
            using (StreamReader reader = new StreamReader(Path.Combine("Templates", "Diagram.html")))
            {
                html = new StringBuilder(reader.ReadToEnd());
                var hardHands = GenerateHardHands(chromosome);
                html.Replace("#hardhands#", hardHands);

                var softhands = GenerateSoftHands(chromosome);
                html.Replace("#softhands#", softhands);

                var pairs = GeneratePairs(chromosome);
                html.Replace("#pairs#", pairs);

                // Propriedades
                html.Replace("#population#", Global.Population.ToString("N0", Global.Culture));
                html.Replace("#games#", Global.Games.ToString("N0", Global.Culture));
                html.Replace("#mutation_rate#", Global.Mutation.ToString("P", Global.Culture));

                html.Replace("#current_generation#", algorithm.GenerationsNumber.ToString("N0", Global.Culture));
                html.Replace("#current_fitness#", ((int)algorithm.BestChromosome.Fitness).ToString("N0", Global.Culture));
                html.Replace("#best_fitness#", bestFitness.Value.ToString("N0", Global.Culture));
                html.Replace("#best_fitness_generation#", bestFitnessGeneration.ToString("N0", Global.Culture));
            }

            return html.ToString();
        }

        private static string GenerateHardHands(BlackjackChromosome chromosome) => GenerateGeneric(chromosome, 0, StrategyMapper.Tables[0], (i) => $"{20 - i}");

        private static string GenerateSoftHands(BlackjackChromosome chromosome) =>
            GenerateGeneric(chromosome, StrategyMapper.Tables[0], StrategyMapper.Tables[1], (i) => $"A-{9 - i}");

        private static string GeneratePairs(BlackjackChromosome chromosome) =>
            GenerateGeneric(chromosome, StrategyMapper.Tables[1], StrategyMapper.Tables[2], (i) => {
                if (i < 2) { return i == 0 ? "A-A" : "T-T"; }
                else return $"{11 - i}-{11 - i}";
            });

        private static string GenerateGeneric(BlackjackChromosome chromosome, int startIndex, int endIndex, LabelFunction function)
        {
            StringBuilder hardhands = new StringBuilder();
            var values = chromosome.Moves;
            for (int i = startIndex; i < endIndex; i++)
            {
                var relativeIndex = i - startIndex;
                if (relativeIndex % 10 == 0)
                {
                    if (relativeIndex != 0)
                        hardhands.Append("</tr>\n");
                    hardhands.Append("<tr>\n");
                    hardhands.Append($"<th>{function(relativeIndex / 10)}</th>\n");
                }

                switch (values[i])
                {
                    case BlackjackMove.Hit:
                        hardhands.Append($"<th value=\"H\"></th>\n");
                        break;
                    case BlackjackMove.Stand:
                        hardhands.Append($"<th value=\"S\"></th>\n");
                        break;
                    case BlackjackMove.DoubleDown:
                        hardhands.Append($"<th value=\"D\"></th>\n");
                        break;
                    case BlackjackMove.Split:
                        hardhands.Append($"<th value=\"P\"></th>\n");
                        break;
                }
            }
            hardhands.Append("</tr>\n");
            return hardhands.ToString();
        }

        public static void Save(string diagram, int generation, string guid)
        {
            if (!Directory.Exists("Diagrams"))
                Directory.CreateDirectory("Diagrams");

            if (!Directory.Exists(Path.Combine("Diagrams", guid)))
                Directory.CreateDirectory(Path.Combine("Diagrams", guid));

            if (!Directory.Exists(Path.Combine("Diagrams", guid, "HTML")))
                Directory.CreateDirectory(Path.Combine("Diagrams", guid, "HTML"));

            if (!Directory.Exists(Path.Combine("Diagrams", guid, "Images")))
                Directory.CreateDirectory(Path.Combine("Diagrams", guid, "Images"));

            if (!Directory.Exists(Path.Combine("Diagrams", guid, "GIF")))
                Directory.CreateDirectory(Path.Combine("Diagrams", guid, "GIF"));



            var html = Path.Combine("Diagrams", guid, "HTML", $"{generation}.html");
            var image = Path.Combine("Diagrams", guid, "Images", $"{generation}.png");
            var images = Path.Combine("Diagrams", guid, "Images", "%01d.png");
            var gif = Path.Combine("Diagrams", guid, "GIF", $"{guid}.gif");

            using (StreamWriter writer = new StreamWriter(html))
                writer.Write(diagram);

            if (!File.Exists(Path.Combine("Diagrams", guid, "HTML", "Diagram.css")))
                File.Copy(Path.Combine("Templates", "Diagram.css"), Path.Combine("Diagrams", guid, "HTML", "Diagram.css"));

            html = Path.Combine(Environment.CurrentDirectory, html);
            image = Path.Combine(Environment.CurrentDirectory, image);
            images = Path.Combine(Environment.CurrentDirectory, images);
            gif = Path.Combine(Environment.CurrentDirectory, gif);

            Console.Write("image.. ");
            if (File.Exists(html)) // usar chrome para tirar um print (precisa do chrome no PATH)
            {
                var chrome = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "chrome",
                        Arguments = $"--headless --disable-gpu --screenshot=\"{image}\" --window-size=1600,900 \"{html}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                        CreateNoWindow = true
                    }
                };
                chrome.Start();
                chrome.WaitForExit();
            }

            Console.Write("gif.. ");
            var ffmpeg = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-y -r 3 -i \"{images}\" -f gif \"{gif}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = true
                }
            };
            ffmpeg.Start();
            ffmpeg.WaitForExit();
        }
    }
}