#include <stdio.h>
#include "game.h"
#include "agents.h"
#include "agents-genetic.h"
#include "matrix.h"

static bool Cheating(Game* game, vector<float> data)
{
    // Dealer always cheats (Hit until just before busting)
    int playerSum = game->GetSum(game->player_hand);
    Card nextCard = game->game_deck.deck[game->game_deck_current];
    return playerSum + (nextCard.value > 8 ? 10 : nextCard.value + 1) <= 21;
}

int main() {
	SetupGenericAlgorithmBlackjack();
    vector<float> weights = GenericAlgorithmLearnBlackjack();
    neuralNetworkGA.SetWeights(weights);
    for (float exposedSum = 5; exposedSum <= 20; exposedSum++) {
        for (float revealedCard = 1; revealedCard <= 10; revealedCard++) {
            bool result = neuralNetworkGA.FeedForward({revealedCard, exposedSum})[0] > 0;
            printf("sum = %0.1f, dc = %0.1f, hit = %s\n", exposedSum, revealedCard, result ? "hit" : "stand");
        }
    }

    // printf("Cheating fitness : %0.4f\n", StrategyFitness(Cheating, {}));
    // for (int i = 0; i < 10; i++) {
    //     Game a = Game();
    //     a.ProcessStates(GenericAlgorithmBlackjack, {68.04, -35.50, 36.96, 59.69, -63.58, -82.30, 72.04, -37.70, 75.95});
    // }
	return 0;
}