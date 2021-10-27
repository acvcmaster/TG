using System;
using GeneticSharp.Domain;
using System.Diagnostics;
using Util;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using SM;
using System.Threading.Tasks;
using System.IO;
using Util.Models;

namespace Runner
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Setting up.. ");
            var ga = BlackjackGA.SetupGA(populationSize: 120, mutationProbability: 0.2f, episodes: 150000, crossoverProbability: 0.5f, maxGenerations: 100);
            RandomDecks.GenerateRandomDecks(150000);
            Stopwatch timer = new Stopwatch();
            Console.WriteLine($"Setup done.");

            double? bestFitness = double.NegativeInfinity;
            int bestFitnessGeneration = -1;
            var bestFit = new BlackjackChromosome();
            var evaluator = new BlackjackFitness(150000);

            ga.GenerationRan += (gen, ev) =>
            {
                timer.Stop();
                var millis = timer.ElapsedMilliseconds;
                var algorithm = gen as GeneticAlgorithm;
                var best = algorithm.BestChromosome as BlackjackChromosome;
                Console.WriteLine($"Generation {algorithm.GenerationsNumber} ended (took {millis} ms)!");


                var fitness = new List<double>();

                Parallel.For(0, 50, i =>
                {
                    var sample = evaluator.Evaluate(best);

                    lock (fitness)
                        fitness.Add(sample);
                });

                var uncertainFitness = new UncertainValue() { Value = fitness.Average(), Uncertainty = fitness.Sterr() };

                Console.WriteLine($"Fitness for current generation is: {uncertainFitness}");

                if (uncertainFitness.Value > bestFitness)
                {
                    bestFitness = uncertainFitness.Value;
                    bestFitnessGeneration = algorithm.GenerationsNumber;
                    bestFit = best;
                }

                Console.WriteLine("Done.");
                timer.Reset();
                timer.Start();
            };
            Console.WriteLine("Algorithm started.");
            timer.Start();
            ga.Start();


            Console.WriteLine($"Run completed!");
            SaveChromosome(bestFit);
        }

        static void VerificarModeloAleatorio()
        {
            var chromo = new BlackjackChromosome();
            var evaluator = new BlackjackFitness(150000);
            var values = new List<double>();

            SaveChromosome(chromo);

            for (var i = 0; i < 50; i++)
                values.Add(evaluator.Evaluate(chromo));

            var fitness = new UncertainValue()
            {
                Value = values.Average(),
                Uncertainty = values.Sterr(),
            };

            Console.WriteLine($"Random model normalized rewards: {fitness}");
        }

        static void SaveChromosome(BlackjackChromosome chromosome, string name = null)
        {
            var values = chromosome?.Moves;
            name = name ?? Guid.NewGuid().ToString();

            using (var writer = new StreamWriter($"Data/Models/{name}.dat"))
            {
                var result = string.Join(',', values
                    ?.Select(item =>
                    {
                        switch (item)
                        {
                            case BlackjackMove.Hit:
                                return 'H';
                            case BlackjackMove.Stand:
                                return 'S';
                            case BlackjackMove.DoubleDown:
                                return 'D';
                            case BlackjackMove.Split:
                                return 'P';
                            default:
                                throw new Exception("Invalid move.");
                        }
                    }));

                writer.Write(result);
                writer.Flush();
            }

            Console.WriteLine($"Results saved to Data/Models/{name}.dat");
        }

        static void ObterCoeffVar()
        {
            using (var writer = new StreamWriter("Data/coeff_var.csv"))
            {
                for (var games = 1000; games <= 500000; games += 1000)
                {
                    Console.WriteLine($"{games} -> {FitnessCoeffVar(games)}");
                    writer.WriteLine($"{games} {FitnessCoeffVar(games)}");
                    writer.Flush();
                }
            }
        }

        static double FitnessCoeffVar(int games)
        {
            var chromo = new BlackjackChromosome();
            var fitnesses = new List<double>();
            var moves = (chromo as BlackjackChromosome).Moves;

            Parallel.For(0, 50, j =>
            {
                var fitness = 0.0;
                for (int i = 0; i < games; i++)
                {
                    Blackjack game = new Blackjack(RandomDecks.PickRandom(), (info) => StrategyMapper.GetMove(info, moves));
                    fitness += game.Play();
                }

                fitnesses.Add(fitness / games);
            });

            return fitnesses.CoeffVar();
        }
    }
}
