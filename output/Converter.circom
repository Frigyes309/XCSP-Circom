pragma circom 2.1.8;
include "circuits/comparators.circom";

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

template Sum() { 
    signal input in[2];
    signal output sum;
    sum <== in[0] + in[1];
}

template Store() { 
    signal input value;
    signal output stored_value;
    stored_value <== value;
}


template Main() {

    signal input X1;
    signal input X2;
    signal output min_1_output;
    component comparator_min_1 = Minimum(2);
    comparator_min_1.values[0] <== X1;
    comparator_min_1.values[1] <== X2;
    min_1_output <== comparator_min_1.min;


}

component main = Main();
