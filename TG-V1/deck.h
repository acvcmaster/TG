#ifndef __DECK
#define __DECK

#include <stdio.h>
#include <stdlib.h>
#include <time.h>

class Card {
public:

    int value;
    int suit;
    int rng;

    ~Card() {}  

    void SetValues(int val, int su);
    void Print();
};

class Deck {
public:

    Card deck[52];

    Deck() {
        int seed = (int)time(NULL) + (int)clock();
        srand(seed);
        for(int i = 0; i < 52; i++)
            deck[i].SetValues( ((i - i % 4) / 4), i % 4);
    }
    void Shuffle();
    ~Deck() {}
};

#endif
