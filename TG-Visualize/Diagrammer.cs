using System;
using System.IO;
using System.Text;

public static class Diagrammer
{
    private delegate string LabelFunction(int rowIndex);

    public static string Generate(char[] strategy, string name)
    {
        var html = new StringBuilder();
        var basePath = AppDomain.CurrentDomain.BaseDirectory;

        using (StreamReader reader = new StreamReader(Path.Combine(basePath, "Templates/Diagram.html")))
        using (StreamReader styleReader = new StreamReader(Path.Combine(basePath, "Templates/Diagram.css")))
        {
            html.Append(reader.ReadToEnd());
            var hardHands = GenerateHardHands(strategy);
            html.Replace("#hardhands#", hardHands);

            var softhands = GenerateSoftHands(strategy);
            html.Replace("#softhands#", softhands);

            var pairs = GeneratePairs(strategy);
            html.Replace("#pairs#", pairs);

            // Propriedades
            html.Replace("#style#", styleReader.ReadToEnd());
        }

        return html.ToString();
    }

    private static string GenerateHardHands(char[] strategy) => GenerateGeneric(strategy, 0, 160, (i) => $"{20 - i}");

    private static string GenerateSoftHands(char[] strategy) =>
        GenerateGeneric(strategy, 160, 240, (i) => $"A-{9 - i}");

    private static string GeneratePairs(char[] strategy) =>
        GenerateGeneric(strategy, 240, 340, (i) =>
        {
            if (i < 2) { return i == 0 ? "A-A" : "T-T"; }
            else return $"{11 - i}-{11 - i}";
        });

    private static string GenerateGeneric(char[] strategy, int startIndex, int endIndex, LabelFunction function)
    {
        StringBuilder hardhands = new StringBuilder();
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

            switch (strategy[i])
            {
                case 'H':
                    hardhands.Append($"<th value=\"H\"></th>\n");
                    break;
                case 'S':
                    hardhands.Append($"<th value=\"S\"></th>\n");
                    break;
                case 'D':
                    hardhands.Append($"<th value=\"D\"></th>\n");
                    break;
                case 'P':
                    hardhands.Append($"<th value=\"P\"></th>\n");
                    break;
            }
        }
        hardhands.Append("</tr>\n");
        return hardhands.ToString();
    }
}