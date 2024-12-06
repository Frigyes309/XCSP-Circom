pragma circom 2.1.8;
include "circuits/comparators.circom";

template Sum2(length) { 
    signal input in[length];
    signal input coeffs[length];
    signal output sum;
    component subSum[length - 1]; //We do not use the whole array, but this way the indexing will be fine
    subSum[0] = Sum();
    subSum[0].in[0] <== in[0] * coeffs[0];
    subSum[0].in[1] <== in[1] * coeffs[1];
    for (var i = 2; i < length; i++) {
        subSum[i - 1] = Sum();
        subSum[i - 1].in[0] <== in[i] * coeffs[i];
        subSum[i - 1].in[1] <== subSum[i - 2].sum;
    }
    sum <== subSum[length - 2].sum;
}

template Sum() { 
    signal input in[2];
    signal output sum;
    sum <== in[0] + in[1];
}

template Main() {

    signal input X1;
    signal input X2;
    signal output sum_1_output;
    component comparator_sum_1 = Sum2(2);
    comparator_sum_1.in[0] <== X1;
    comparator_sum_1.coeffs[0] <== 1;
    comparator_sum_1.in[1] <== X2;
    comparator_sum_1.coeffs[1] <== 2;
    component comparator_sum_equality_1 = IsEqual();
    comparator_sum_equality_1.in[0] <== comparator_sum_1.sum;
    comparator_sum_equality_1.in[1] <== 15;
    sum_1_output <== comparator_sum_equality_1.out;


}

component main = Main();
