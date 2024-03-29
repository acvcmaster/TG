﻿using System;
using GeneticSharp.Domain;
using System.Diagnostics;
using Util;
using System.Collections.Generic;
using System.Linq;
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
        }

        static void GenerateLearningCurves()
        {
            // 52, 0.5f
            // 127 0.4f
            // 177, 0.6f
            // 227, 0.1f

            var curves = new List<UncertainValue>[4]
            {
                new List<UncertainValue>(),
                new List<UncertainValue>(),
                new List<UncertainValue>(),
                new List<UncertainValue>(),
            };

            RandomDecks.GenerateRandomDecks(500000);

            RunGAWithParameters(52, 0.5f, 150000, 0.5f, 100, (_, value) => curves[0].Add(value));
            RunGAWithParameters(127, 0.4f, 150000, 0.5f, 100, (_, value) => curves[1].Add(value));
            RunGAWithParameters(177, 0.6f, 150000, 0.5f, 100, (_, value) => curves[2].Add(value));
            RunGAWithParameters(227, 0.1f, 150000, 0.5f, 100, (_, value) => curves[3].Add(value));

            using (var writer = new StreamWriter("Data/curva_aprendizado.csv"))
            {
                for (var gen = 1; gen <= 100; gen++)
                {
                    writer.WriteLine("{0} {1} {2} {3} {4}",
                        gen,
                        curves[0][gen - 1].Value,
                        curves[1][gen - 1].Value,
                        curves[2][gen - 1].Value,
                        curves[3][gen - 1].Value
                    );
                }
            }
        }

        static void FindParameterModelCorrespondence()
        {
            var models = Directory.GetFiles("Data/Models");
            var orderedModels = models
                .Select(model => new { name = Path.GetFileName(model), created = File.GetCreationTime(model) })
                .OrderBy(item => item.created)
                .ToArray();

            using (var writer = new StreamWriter("Data/parametros_genetico_model.csv"))
            {
                var index = 0;

                for (var populationSize = 27; populationSize <= 300; populationSize += 25)
                    for (var mutationProbability = 0.0f; mutationProbability < 1.1f; mutationProbability += 0.1f)
                    {
                        writer.WriteLine($"{populationSize} {mutationProbability} {orderedModels[index].name}");
                        index++;
                    }

                writer.Flush();
            }
        }

        static void RunGAVariousParameters()
        {
            int episodes = 150000;
            float crossoverProbability = 0.5f;
            int maxGenerations = 100;

            RandomDecks.GenerateRandomDecks(500000);

            using (var writer = new StreamWriter("Data/parametros_genetico.csv"))
            {
                for (var populationSize = 27; populationSize <= 300; populationSize += 25)
                    for (var mutationProbability = 0.0f; mutationProbability < 1.1f; mutationProbability += 0.1f)
                    {
                        var result = RunGAWithParameters(populationSize, mutationProbability, episodes, crossoverProbability, maxGenerations);
                        SaveChromosome(result.Item1);

                        writer.WriteLine($"{populationSize} {mutationProbability} {result.Item2.Value} {result.Item2.Uncertainty}");
                        writer.Flush();
                    }
            }
        }

        static Tuple<BlackjackChromosome, UncertainValue> RunGAWithParameters(int populationSize, float mutationProbability, int episodes, float crossoverProbability, int maxGenerations, Action<int, UncertainValue> onGenerationCompleted = null, int parallelism = 16)
        {
            Console.Write("Setting up.. ");
            var ga = BlackjackGA.SetupGA(populationSize, mutationProbability, episodes, crossoverProbability, maxGenerations, parallelism);
            Stopwatch timer = new Stopwatch();
            Console.WriteLine($"Setup done.");

            var bestFitness = new UncertainValue() { Value = double.NegativeInfinity, Uncertainty = 0 };
            int bestFitnessGeneration = -1;
            var bestFit = new BlackjackChromosome();
            var evaluator = new BlackjackFitness(episodes);

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

                if (uncertainFitness.Value > bestFitness.Value)
                {
                    bestFitness = uncertainFitness;
                    bestFitnessGeneration = algorithm.GenerationsNumber;
                    bestFit = best;
                }

                if (onGenerationCompleted is not null)
                    onGenerationCompleted(algorithm.GenerationsNumber, bestFitness);


                Console.WriteLine("Done.");
                timer.Reset();
                timer.Start();
            };
            Console.WriteLine("Algorithm started.");
            timer.Start();
            ga.Start();


            Console.WriteLine($"Run completed!");

            return new Tuple<BlackjackChromosome, UncertainValue>(bestFit, bestFitness);
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

        static void SaveChromosome(BlackjackChromosome chromosome)
        {
            var values = chromosome?.Moves;
            var name = Guid.NewGuid().ToString();

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
