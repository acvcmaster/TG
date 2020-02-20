#ifndef __UTIL
#define __UTIL

#include "stdlib.h"

static float randf(float min, float max)
{
    if (min > max)
        return 0;

    return min + ((float)rand() / (float)RAND_MAX) * (max - min);
}

#endif