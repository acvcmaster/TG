using Util;
using Util.Models;
using System.Drawing;

namespace Runner
{
    public class Diagrammer
    {
        public static IDiagram Generate(double[] values)
        {
            StaticNN.SetWeights(values);
            // StaticNN.Compute(new BlackjackInformation())
            return null;
        }
        public static void Save(IDiagram diagram, string file)
        {

        }
    }

    public interface IDiagram
    {

    }
}