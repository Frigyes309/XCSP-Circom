namespace Converter.Constraints {
    static class Maximum {

        static int counter = 0;
        public static string component() {
            return "template Maximum(length) { \n" +
                   "    signal input values[length];\n" +
                    "    signal output max;\n" +
                    "\n" +
                    "    component local_max[length];\n" +
                    "    component comparator[length];\n" +
                    "    component sum[length];\n" +
                    "    local_max[0] = Store();\n" +
                    "    local_max[0].value <== values[0];\n" +
                    "\n" +
                    "    for (var i = 1; i < length; i++) {\n" +
                    "        comparator[i] = GreaterThan(252);\n" +
                    "        comparator[i].in[0] <== values[i];\n" +
                    "        comparator[i].in[1] <== local_max[i - 1].stored_value;\n" +
                    "\n" +
                    "        local_max[i] = Store();\n" +
                    "        sum[i] = Sum();\n" +
                    "        sum[i].in[0] <== values[i] * comparator[i].out;\n" +
                    "        sum[i].in[1] <== local_max[i - 1].stored_value * (1 - comparator[i].out);\n" +
                    "        local_max[i].value <== sum[i].sum;\n" +
                    "    }\n" +
                    "    max <== local_max[length - 1].stored_value;\n" +
                    "}";
        }

        public static string calling(string[] values) {
            string result =  $"    signal output max_{++counter}_output;\n" +
                $"    component comparator_max_{counter} = Maximum({values.Length});\n";
            for (int i = 0; i < values.Length; i++) {
                result += $"    comparator_max_{counter}.values[{i}] <== {values[i]};\n";
            }
            result += $"    max_{counter}_output <== comparator_max_{counter}.max;\n";
            return  result;
        }
    }
}