#ifndef __GAME
#define __GAME

#include <list> 
#include <iterator>
#include <vector>
using namespace std; 
#include "deck.h"
#define MAX_STATES 100


enum GameState
{
    ST_INITIAL,
    ST_PLAYER_WINS,
    ST_DEALER_WINS,
    ST_BET,
    ST_CHECK_DEALER_BLACKJACK,
    ST_PLAYER_TURN,
    ST_PLAYER_HIT,
    ST_PLAYER_STAND,
    ST_DEALER_WITHDRAW,
    ST_DEALER_TURN,
    ST_FINAL_COMPARISON
};

class Game
{
public:
    Deck game_deck = Deck();
    int game_deck_current = 0;
    GameState state = ST_INITIAL; // Initial state
    Card revealed_card; // Dealer revealed card
    list<Card> dealer_hand, player_hand;
    int player_bet = 0;
    int ProcessStates(bool (*strategy)(Game*, vector<float>), vector<float> strategyData);
    void GivePlayerCard();
    GameState P_ST_Initial();
    GameState P_ST_Bet();
    GameState P_ST_Check_Dealer_Blackjack();
    GameState P_ST_Player_Turn();
    GameState P_ST_Player_Hit();
    GameState P_ST_Player_Stand();
    GameState P_ST_Dealer_Turn();
    GameState P_ST_Final_Comparison();
    int GetSum(list<Card> hand);
    void GiveDealerCard(bool reveal = false);
    bool (*Strategy)(Game*, vector<float>);
    vector<float> StrategyData;
    ~Game() {}
};

#endif