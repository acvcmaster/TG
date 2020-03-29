using System.Drawing;

namespace Runner
{
    public class Diagrammer
    {
        public static IDiagram Generate(BlackjackChromosome chromosome)
        {
            return null;
        }
        public static void Save(IDiagram diagram, string file)
        {

        }
    }

    public interface IDiagram
    {
        Image File { get; set; }
    }
}