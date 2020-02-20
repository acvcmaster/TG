#include "game.h"
#include "agents.h"
#include "stdio.h"

int Game::ProcessStates(bool (*strategy)(Game*, vector<float>), vector<float> strategyData)
{
    Strategy = strategy;
    StrategyData = strategyData;
    GameState next_state = state;
    for(int st = 0; st < MAX_STATES; st++) // MAX_STATES = 100
    {
        switch(state)
        {
            case ST_INITIAL:
                next_state = P_ST_Initial();
                break;
            case ST_BET:
                next_state = P_ST_Bet();
                break;
            case ST_CHECK_DEALER_BLACKJACK:
                next_state = P_ST_Check_Dealer_Blackjack();
                break;
            case ST_PLAYER_TURN:
                next_state = P_ST_Player_Turn();
                break;
            case ST_PLAYER_HIT:
                next_state = P_ST_Player_Hit();
                break;
            case ST_PLAYER_STAND:
                next_state = P_ST_Player_Stand();
                break;
            case ST_DEALER_TURN:
                next_state = P_ST_Dealer_Turn();
                break;
            case ST_FINAL_COMPARISON:
                next_state = P_ST_Final_Comparison();
                break;
            case ST_DEALER_WINS:
                return -1;
            case ST_PLAYER_WINS:
                return 1;
            case ST_DEALER_WITHDRAW:
                return 0;
            default:
                return 0;
        }
        state = next_state;
    }
    return 0; // Tie
}

GameState Game::P_ST_Initial()
{
    game_deck.Shuffle();
    GivePlayerCard();
    GiveDealerCard();
    return ST_BET;
}

GameState Game::P_ST_Bet()
{
    player_bet = PlayerMakeBet();
    GiveDealerCard(true);
    return ST_CHECK_DEALER_BLACKJACK;
}

GameState Game::P_ST_Check_Dealer_Blackjack()
{
    Card second = dealer_hand.back();
    dealer_hand.pop_back();
    Card first = dealer_hand.back();
    dealer_hand.push_back(second);
    if((second.value == 1 && first.value > 8) || (first.value == 1 && second.value > 8))
        return ST_DEALER_WINS;
    else return ST_PLAYER_TURN;
}

GameState Game::P_ST_Player_Turn()
{
    bool hit = Strategy(this, StrategyData);
    // int exposed_sum = GetSum(player_hand);
    // Card dealer_exposed = dealer_hand.back();
    // printf("When sum = %d and dealer card is ", exposed_sum);
    // dealer_exposed.Print();
    // printf(" player : %s\n", hit ? "hit" : "stand");
    return hit ? ST_PLAYER_HIT : ST_PLAYER_STAND;
}

GameState Game::P_ST_Player_Hit()
{
    GivePlayerCard();
    int exposed_sum = GetSum(player_hand);
    exposed_sum -= (player_hand.begin())->value + 1;
    return exposed_sum > 21 ? ST_DEALER_WINS : ST_PLAYER_TURN; // Bust
}

GameState Game::P_ST_Player_Stand()
{
    if(DealerChallenge(this))
    {
        // open dealer card (no implementation needed)
        return ST_DEALER_TURN;
    }
    else return ST_DEALER_WITHDRAW; // player keeps bet
}

GameState Game::P_ST_Dealer_Turn()
{
    if(DealerHit(this))
    {
        GiveDealerCard(false);
        if(GetSum(dealer_hand) == 21)
            return ST_DEALER_WINS;
        else return ST_DEALER_TURN;
    }
    else return ST_FINAL_COMPARISON;
}

GameState Game::P_ST_Final_Comparison()
{
    int player_sum = GetSum(player_hand);
    if(player_sum > 21)
        return ST_DEALER_WINS;
    int dealer_sum = GetSum(dealer_hand);
    return dealer_sum >= player_sum ? ST_DEALER_WINS : ST_PLAYER_WINS;
}

int Game::GetSum(list<Card> hand)
{
    int sum = 0;
    list <Card> :: iterator it; 
    for(it = hand.begin(); it != hand.end(); it++)
        sum += it->value > 8 ? 10 : it->value + 1;
    return sum; 
}

void Game::GivePlayerCard()
{
    player_hand.push_back(game_deck.deck[game_deck_current]); // Card to player
    game_deck_current++;
}

void Game::GiveDealerCard(bool reveal)
{
    Card current_card = game_deck.deck[game_deck_current];
    dealer_hand.push_back(current_card); // Card to dealer
    game_deck_current++;
    if(reveal)
        revealed_card = current_card;
}