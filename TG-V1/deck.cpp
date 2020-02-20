#include "deck.h"
#include "algorithm"

using namespace std;

const char* VALUES[] = {"Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Queen", "Jack", "King"};
const char* SUITS[] = {"diamonds", "spades", "hearts", "clubs"};

void Card::Print()
{
    printf("%s of %s", VALUES[value], SUITS[suit]);
}

void Card::SetValues(int val, int su)
{
    value = val;
    suit = su;
    rng = rand();
}

bool compare_card_rng(Card a, Card b)
{
    return a.rng < b.rng;
    printf("%d is less than %d\n", a.rng, b.rng);
}

void Deck::Shuffle()
{
    sort(deck, deck + 52, compare_card_rng);
}