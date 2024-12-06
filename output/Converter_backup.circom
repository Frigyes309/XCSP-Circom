pragma circom 2.1.8;
include "circuits/comparators.circom";

template Store() { 
    signal input value;
    signal output stored_value;
    stored_value <== value;
}

template AllDifferent(length) { 
    signal input values[length];
    signal output different;

    component comparator[length * (length - 1) / 2];
    component sum[length * (length - 1) / 2 + 1];
    var index = 0;
    sum[0] = Sum();
    sum[0].in[0] <== 0;
    sum[0].in[1] <== 0;

    for (var i = 0; i < length; i++) {
        for (var j = i + 1; j < length; j++) {
            comparator[index] = IsEqual();
            comparator[index].in[0] <== values[i];
            comparator[index].in[1] <== values[j];
            sum[index + 1] = Sum();
            sum[index + 1].in[0] <== sum[index].sum;
            sum[index + 1].in[1] <== comparator[index].out;
            index += 1;
        }
    }
    component helper = IsEqual();
    helper.in[0] <== sum[index].sum;
    helper.in[1] <== 0;
    different <== helper.out;
}

template Sum() { 
    signal input in[2];
    signal output sum;
    sum <== in[0] + in[1];
}

template Sum2(length) { 
    signal input in[length];
    signal input coeffs[length];
    signal output sum;
    component subSum[length - 1];
    for (var i = 0; i < length - 1; i++) {
        subSum[i] = Sum();
        subSum[i].in[0] <== in[i] * coeffs[i];
        subSum[i].in[1] <== in[i + 1] * coeffs[i + 1];
    }
    sum <== subSum[length - 2].sum;
}

template Minimum(length) { 
    signal input values[length];
    signal output min;

    component local_min[length];
    component comparator[length];
    component sum[length];
    local_min[0] = Store();
    local_min[0].value <== values[0];

    for (var i = 1; i < length; i++) {
        comparator[i] = LessThan(252);
        comparator[i].in[0] <== values[i];
        comparator[i].in[1] <== local_min[i - 1].stored_value;

        local_min[i] = Store();
        sum[i] = Sum();
        sum[i].in[0] <== values[i] * comparator[i].out;
        sum[i].in[1] <== local_min[i - 1].stored_value * (1 - comparator[i].out);
        local_min[i].value <== sum[i].sum;
    }
    min <== local_min[length - 1].stored_value;
}

template Maximum(length) { 
    signal input values[length];
    signal output max;

    component local_max[length];
    component comparator[length];
    component sum[length];
    local_max[0] = Store();
    local_max[0].value <== values[0];

    for (var i = 1; i < length; i++) {
        comparator[i] = GreaterThan(252);
        comparator[i].in[0] <== values[i];
        comparator[i].in[1] <== local_max[i - 1].stored_value;

        local_max[i] = Store();
        sum[i] = Sum();
        sum[i].in[0] <== values[i] * comparator[i].out;
        sum[i].in[1] <== local_max[i - 1].stored_value * (1 - comparator[i].out);
        local_max[i].value <== sum[i].sum;
    }
    max <== local_max[length - 1].stored_value;
}

template Element(length) {
    signal input index;
    signal input values[length];
    signal input value;
    signal output equal;
    component value_equal[length];
    component index_equal[length];
    component sum[length + 1];
    sum[0] = Sum();
    sum[0].in[0] <== 0;
    sum[0].in[1] <== 0;

    for (var i = 0; i < length; i++) {
        value_equal[i] = IsEqual();
        value_equal[i].in[0] <== values[i];
        value_equal[i].in[1] <== value;

        index_equal[i] = IsEqual();
        index_equal[i].in[0] <== i;
        index_equal[i].in[1] <== index;

        sum[i + 1] = Sum();
        sum[i + 1].in[0] <== sum[i].sum;
        sum[i + 1].in[1] <== value_equal[i].out * index_equal[i].out;
    }
    equal <== sum[length].sum;
}

template Channel(length) {
    signal input array1[length];
    signal input array2[length];
    signal output value;
    component equal[length * length * 3];
    component identity[length * length + 1];
    identity[0] = Store();
    identity[0].value <== 1;
    for (var i = 0; i < length; i++) {
        for (var j = 0; j < length; j++) {
        equal[(i * length + j) * 3] = IsEqual();
        equal[(i * length + j) * 3].in[0] <== array1[i];
        equal[(i * length + j) * 3].in[1] <== array2[j];
        equal[(i * length + j) * 3 + 1] = IsEqual();
        equal[(i * length + j) * 3 + 1].in[0] <== array2[j];
        equal[(i * length + j) * 3 + 1].in[1] <== array1[i];
        equal[(i * length + j) * 3 + 2] = IsEqual();
        equal[(i * length + j) * 3 + 2].in[0] <== equal[(i * length + j) * 3].out;
        equal[(i * length + j) * 3 + 2].in[1] <== equal[(i * length + j) * 3 + 1].out;
        identity[(i * length + j) + 1] = Store();
        identity[(i * length + j) + 1].value <== identity[(i * length + j)].stored_value * equal[(i * length + j) * 3 + 2].out;
        }
    }
    value <== identity[length * length].stored_value;
}

template Main() {

    signal input X1;
    signal input X2;
    signal input X3;
    signal input X4;
    signal input Y1;
    signal input Y2;
    signal output all_different_1_output;
    component comparator_all_different_1 = AllDifferent(3);
    comparator_all_different_1.values[0] <== X1;
    comparator_all_different_1.values[1] <== X2;
    comparator_all_different_1.values[2] <== X3;
    all_different_1_output <== comparator_all_different_1.different;

    signal output sum_1_output;
    component comparator_sum_1 = Sum2(3);
    comparator_sum_1.in[0] <== X1;
    comparator_sum_1.coeffs[0] <== 1;
    comparator_sum_1.in[1] <== X2;
    comparator_sum_1.coeffs[1] <== 1;
    comparator_sum_1.in[2] <== X3;
    comparator_sum_1.coeffs[2] <== 1;
    component comparator_sum_equality_1 = Sum();
    comparator_sum_equality_1.in[0] <== comparator_sum_1.sum;
    comparator_sum_equality_1.in[1] <== 15;
    sum_1_output <== comparator_sum_equality_1.sum;

    signal output min_1_output;
    component comparator_min_1 = Minimum(3);
    comparator_min_1.values[0] <== X1;
    comparator_min_1.values[1] <== X2;
    comparator_min_1.values[2] <== X3;
    min_1_output <== comparator_min_1.min;

    signal output max_1_output;
    component comparator_max_1 = Maximum(3);
    comparator_max_1.values[0] <== X1;
    comparator_max_1.values[1] <== X2;
    comparator_max_1.values[2] <== X3;
    max_1_output <== comparator_max_1.max;

    signal output element_1_output;
    component comparator_element_1 = Element(4);
    comparator_element_1.index <== 0;
    comparator_element_1.values[0] <== X1;
    comparator_element_1.values[1] <== X2;
    comparator_element_1.values[2] <== X3;
    comparator_element_1.values[3] <== X4;
    comparator_element_1.value <== X1;
    element_1_output <== comparator_element_1.equal;

    signal output channel_1_output;
    component comparator_channel_1 = Channel(3);
    comparator_channel_1.array1[0] <== X1;
    comparator_channel_1.array2[0] <== X3;
    comparator_channel_1.array1[1] <== X2;
    comparator_channel_1.array2[1] <== X2;
    comparator_channel_1.array1[2] <== X3;
    comparator_channel_1.array2[2] <== X1;
    channel_1_output <== comparator_channel_1.value;


}

component main = Main();
