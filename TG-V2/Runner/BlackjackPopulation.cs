using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using SM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runner
{
    public class BlackjackPopulation : Population
    {
        public IDictionary<int, double> GenerationFitness { get; set; }
        public int Games { get; }

        public BlackjackPopulation(int maxSize, int games, IChromosome adamChromosome) : base(maxSize, maxSize, adamChromosome)
        {
            Games = games;
        }

        public override void CreateNewGeneration(IList<IChromosome> chromosomes)
        {
            base.CreateNewGeneration(chromosomes);
            GetFitnessForAll();
        }

        public void GetFitnessForAll()
        {
            if (GenerationFitness == null)
                GenerationFitness = new Dictionary<int, double>();
            GenerationFitness.Clear();

            Parallel.For(0, Games, j =>
            {
                Deck deck = new Deck(4);
                deck.Shuffle();
                Dictionary<int, double> fitnessIncrement = new Dictionary<int, double>();
                foreach (var chromosome in CurrentGeneration.Chromosomes)
                {
                    var hash = GetChromosomeHash(chromosome);
                    Blackjack game = new Blackjack(deck, (info) =>
                    {
                        // TODO: Get stategy from genome
                        return BlackjackMove.Stand;
                    });
                    fitnessIncrement.Add(hash, game.Play());
                }

                lock (GenerationFitness) // thread-safe
                {
                    foreach (var increment in fitnessIncrement)
                    {
                        if (!GenerationFitness.ContainsKey(increment.Key))
                            GenerationFitness.Add(increment.Key, 0);

                        GenerationFitness[increment.Key] += fitnessIncrement[increment.Key];
                    }
                }
            });
            System.Console.WriteLine("OK!");
        }

        public int GetChromosomeHash(IChromosome chromosome)
        {
            var genes = chromosome.GetGenes();
            Console.WriteLine(genes[0].Value);
            return 0;
        }
    }
}
