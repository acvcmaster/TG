#include "neural-network.h"

NeuralNetwork::NeuralNetwork() { }

NeuralNetwork::NeuralNetwork(unsigned input_size, unsigned output_size)
{
    size_input = input_size;
    size_output = output_size;
}

void NeuralNetwork::AddHiddenLayers(vector<unsigned> sizes)
{
    hidden_layer_sizes = sizes;
}

void NeuralNetwork::GetWeights()
{
    /*
    2, 3, 3, 1 -> 2x3, 3x3, 3x1
    */
    unsigned layer_count = hidden_layer_sizes.size() + 2;
    weights = vector<Matrix>(layer_count - 1);
    biases = vector<Matrix>(layer_count - 1);
    
    vector<unsigned> sizes = vector<unsigned>(layer_count);
    sizes[0] = size_input;
    for (int i = 1; i < layer_count - 1; i++)
        sizes[i] = hidden_layer_sizes[i - 1];
    sizes[layer_count - 1] = size_output;

    for (int i = 0; i < layer_count - 1; i++)
        weights[i] = Matrix(sizes[i], sizes[i + 1], 0);
    
    for (int i = 0; i < layer_count - 1; i++)
    {
        if (i == layer_count - 2)
            biases[i] = Matrix(1, size_output, 0);   
        else
            biases[i] = Matrix(1, hidden_layer_sizes[i], 0);   
    }
}

unsigned NeuralNetwork::GetWeightCount()
{
    unsigned result = 0;
    unsigned weights_vector_count = weights.size();
    unsigned biases_vector_count = biases.size();

    for (int i = 0; i < weights_vector_count; i++)
        result += weights[i].getRows() * weights[i].getCols();
    
    for (int i = 0; i < biases_vector_count; i++)
        result += biases[i].getCols();

    return result;
}

void NeuralNetwork::SetWeights(vector<float> new_weights)
{
    int weightIndex = 0;
    for (int i = 0; i < weights.size(); i++)
    {
        unsigned rows = weights[i].getRows();
        unsigned cols = weights[i].getCols();
        for (unsigned j = 0; j < rows; j++)
            for (unsigned k = 0; k < cols; k++)
            {
                weights[i](j, k) = new_weights[weightIndex];
                weightIndex++;
            }
    }

    for (int i = 0; i < biases.size(); i++)
    {
        unsigned cols = biases[i].getCols();
        for (unsigned j = 0; j < cols; j++)
        {
            biases[i](0, j) = new_weights[weightIndex];
            weightIndex++;
        }
    }
}

Matrix NeuralNetwork::Activation(Matrix input) // Hyperbolic tangent
{
    unsigned m_rowSize = input.getRows();
    unsigned m_colSize = input.getCols();

    Matrix result(m_rowSize, m_colSize, 0.0);
    unsigned i,j;
    for (i = 0; i < m_rowSize; i++)
        for (j = 0; j < m_colSize; j++)
            result(i,j) = tanh(input(i, j));

    return result;
}

vector<float> NeuralNetwork::FeedForward(vector<float> inputs)
{
    Matrix input_matrix = Matrix(1, size_input, 0);
    for (int i = 0; i < size_input; i++)
        input_matrix(0, i) = inputs[i];

    int weights_size = weights.size();

    for (int i = 0; i < weights_size; i++)
        input_matrix = Activation(input_matrix * weights[i] + biases[i]);

    return input_matrix.getRow(0);
}