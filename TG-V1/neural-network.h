#ifndef __NEURAL_NETWORK
#define __NEURAL_NETWORK

#include "math.h"
#include <vector>
#include "matrix.h"

class NeuralNetwork {
    unsigned size_input;
    unsigned size_output;
    vector<unsigned> hidden_layer_sizes;
    vector<Matrix> weights;
    vector<Matrix> biases;
public:
    NeuralNetwork();
    NeuralNetwork(unsigned input_size, unsigned output_size);
    void AddHiddenLayers(vector<unsigned> sizes);
    void GetWeights();
    unsigned GetWeightCount();
    void SetWeights(vector<float> new_weights);
    Matrix Activation(Matrix input);
    vector<float> FeedForward(vector<float> inputs);
};

#endif
