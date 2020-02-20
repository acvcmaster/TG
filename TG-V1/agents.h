#ifndef __AGENTS
#define __AGENTS

#include "game.h"
#include <vector>


#define DEFAULT_BET 1 // 1 chip by default
#define MAX_GAMES 3000 // number of games to estimate score (higher values -> better accuracy but slower)

static int PlayerMakeBet()
{
    return DEFAULT_BET;
}

static bool DealerChallenge(Game* game)
{
    // Dealer always cheats (Challenge only if dealer is going to win)
    int playerSum = game->GetSum(game->player_hand);

    if (playerSum > 21)
        return true;

    int dealerSum = game->GetSum(game->dealer_hand);
    int index = 0;
    Card nextCard;
    while (dealerSum <= 21)
    {
        nextCard = game->game_deck.deck[game->game_deck_current + index];
        dealerSum += (nextCard.value > 8 ? 10 : nextCard.value + 1);
        index++;
    }

    dealerSum -= (nextCard.value > 8 ? 10 : nextCard.value + 1);
    return dealerSum >= playerSum;
}

static bool DealerHit(Game* game)
{
    // Dealer always cheats (Hit until just before busting)
    int dealerSum = game->GetSum(game->dealer_hand);
    Card nextCard = game->game_deck.deck[game->game_deck_current];
    return dealerSum + (nextCard.value > 8 ? 10 : nextCard.value + 1) <= 21;
}

static float StrategyFitness(bool (*strategy)(Game*, vector<float>), vector<float> strategyData)
{
    int player_funds = 0;
    for(int i = 0; i < MAX_GAMES; i++)
	{
		player_funds -= DEFAULT_BET;
		Game current = Game();
		int iwin = current.ProcessStates(strategy, strategyData);
		player_funds += iwin == 1 ? 2 * current.player_bet : ((iwin == 0) ? current.player_bet : 0);
	}
    return (((float)player_funds / (float)(MAX_GAMES * DEFAULT_BET)) + 1) / 2;
}

#endif