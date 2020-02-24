using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using System.Collections.Generic;

namespace Runner
{
    public class BlackjackPopulation : Population
    {
        public BlackjackPopulation(int minSize, int maxSize, IChromosome adamChromosome) : base(minSize, maxSize, adamChromosome)
        {
        }


        public override void CreateInitialGeneration()
        {

        }

        public override void CreateNewGeneration(IList<IChromosome> chromosomes)
        {
        }

        public override void EndCurrentGeneration()
        {
        }
    }
}
