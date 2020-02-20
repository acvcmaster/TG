//
//  matrix.h
//
//  Created by Furkanicus on 12/04/15.
//  Copyright (c) 2015 Furkan. All rights reserved.
// Learnrope.com

#ifndef __EE_242_Project_2__matrix__
#define __EE_242_Project_2__matrix__

#include <stdio.h>
#include <fstream> // for file access
#include <iostream>
#include <stdlib.h>
#include <sstream>
#include <string>
#include <vector>
#include <tuple>
#include <cmath>

using std::vector;
using std::tuple;
class Matrix {
private:
    unsigned m_rowSize;
    unsigned m_colSize;
    vector<vector<float> > m_matrix;
public:
    Matrix(unsigned, unsigned, float);
    Matrix(const char *);
    Matrix(const Matrix &);
    Matrix(vector<float>);
    Matrix();
    ~Matrix();
    
    // Matrix Operations
    Matrix operator+(Matrix &);
    Matrix operator-(Matrix &);
    Matrix operator*(Matrix &);
    Matrix transpose();
    
    // Scalar Operations
    Matrix operator+(float);
    Matrix operator-(float);
    Matrix operator*(float);
    Matrix operator/(float);
    
    // Aesthetic Methods
    float& operator()(const unsigned &, const unsigned &);
    void print() const;
    unsigned getRows() const;
    unsigned getCols() const;
    vector<float> getRow(unsigned);
    
    // Power Iteration
    tuple<Matrix, float, int> powerIter(unsigned, float);
};
#endif /* defined(__EE_242_Project_2__matrix__) */
