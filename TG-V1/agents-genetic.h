#ifndef __AGENTS_GENETIC
#define __AGENTS_GENETIC

#include "game.h"
#include "math.h"
#include "neural-network.h"
#include "util.h"
#include <bits/stdc++.h> 

using namespace std;

// Parameters
#define POPULATION_SIZE 10 // Total population on each iteration
#define BEST_FIT 5 // Select top N of individuals to build next generation
#define MUTATION_RATE 0.07 // Prob
#define MAX_GENERATIONS 100

class GAIndividual {
public:
    vector<float> genome;
    float fitness;
};

static NeuralNetwork neuralNetworkGA;

static void SetupGenericAlgorithmBlackjack()
{
    neuralNetworkGA = NeuralNetwork(2, 1);
    neuralNetworkGA.AddHiddenLayers({2, 2});
    neuralNetworkGA.GetWeights();
}

static bool GenericAlgorithmBlackjack(Game* game, vector<float> data)
{
    // Get the observable state
    float revealedCard = float(game->revealed_card.value > 8 ? 10 : game->revealed_card.value + 1);
    float exposedSum = (float)game->GetSum(game->player_hand); // - (game->player_hand.begin()->value + 1);

    neuralNetworkGA.SetWeights(data);
    return neuralNetworkGA.FeedForward({revealedCard, exposedSum})[0] > 0;
}

static bool compareFitnessGA(GAIndividual a, GAIndividual b)
{
    return a.fitness > b.fitness;
}

static GAIndividual GetChild(GAIndividual parentA, GAIndividual parentB, unsigned genomeSize)
{
    int halfGenomeSize = genomeSize / 2;
    GAIndividual child = GAIndividual();
    child.genome = vector<float>(genomeSize);
    for (int i = 0; i < halfGenomeSize; i++)
        child.genome[i] = parentA.genome[i];
    
    for (int i = halfGenomeSize; i < genomeSize; i++)
        child.genome[i] = parentB.genome[i];
    
    for (int i = 0; i < genomeSize; i++) // Mutation
        if (randf(0, 1) <= MUTATION_RATE)
            child.genome[i] = randf(-100, 100);
    return child;
}

static vector<float> GenericAlgorithmLearnBlackjack()
{
    unsigned genomeSize = neuralNetworkGA.GetWeightCount();
    vector<GAIndividual> population = vector<GAIndividual>(POPULATION_SIZE);

    // Initialize population
    for (int i = 0; i < POPULATION_SIZE; i++)
    {
        population[i] = GAIndividual();
        population[i].genome = vector<float>(genomeSize);
        for (int j = 0; j < genomeSize; j++)
            population[i].genome[j] = randf(-100, 100);
        population[i].fitness = StrategyFitness(GenericAlgorithmBlackjack, population[i].genome);
    }

    float bestFitness = -1;
    vector<float> bestFit = vector<float>();

    for (int generation = 1; generation <= MAX_GENERATIONS; generation++)
    {
        // select most fist (Top BEST_FIT individuals)
        sort(population.begin(), population.end(), compareFitnessGA);

        printf("Best fitness for generation %d : %0.4f\n", generation, population[0].fitness);
        if (population[0].fitness > bestFitness)
        {
            bestFitness = population[0].fitness;
            printf("New best fit individual found at generation %d (Fitness = %0.4f)\n", generation, bestFitness);
            bestFit = population[0].genome;
            // printf("Weights : ");
            // for (int i = 0; i < genomeSize; i++)
            //     printf("%0.2f, ", population[0].genome[i]);
            printf("\n");
            printf("Double-checked fitness : %0.4f\n", StrategyFitness(GenericAlgorithmBlackjack, population[0].genome));
        }

        vector<GAIndividual> new_population = vector<GAIndividual>(POPULATION_SIZE);
        for (int i = 0; i < POPULATION_SIZE; i++)
        {
            int randomA = (int)randf(0, (float)BEST_FIT);
            int randomB = (int)randf(0, (float)BEST_FIT);
            new_population[i] = GetChild(population[randomA], population[randomB], genomeSize);
            new_population[i].fitness = StrategyFitness(GenericAlgorithmBlackjack, new_population[i].genome);
        }
        population = new_population;
    }
    return bestFit;
}

#endif