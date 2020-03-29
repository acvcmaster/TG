using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Util.Models;

namespace Runner
{
    public class Diagrammer
    {
        public delegate string LabelFunction(int rowIndex);
        public static string Generate(BlackjackChromosome chromosome)
        {
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


            var html = Path.Combine("Diagrams", guid, "HTML", $"{generation}.html");
            var image = Path.Combine("Diagrams", guid, "Images", $"{generation}.png");

            using (StreamWriter writer = new StreamWriter(html))
                writer.Write(diagram);

            if (!File.Exists(Path.Combine("Diagrams", guid, "HTML", "Diagram.css")))
                File.Copy(Path.Combine("Templates", "Diagram.css"), Path.Combine("Diagrams", guid, "HTML", "Diagram.css"));

            html = Path.Combine(Environment.CurrentDirectory, html);
            image = Path.Combine(Environment.CurrentDirectory, image);

            if (File.Exists(html)) // usar chrome para tirar um print (precisa do chrome no PATH)
            {
                var process = new Process
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
                process.Start();
                process.WaitForExit();
            }
        }
    }
}