namespace Converter.Constraints {
    static class Minimum {

        static int counter = 0;
        public static string component() {
            return "template Minimum(length) { \n" +
                "    signal input values[length];\n" +
                    "    signal output min;\n" +
                    "\n" +
                    "    component local_min[length];\n" +
                    "    component comparator[length];\n" +
                    "    component sum[length];\n" +
                    "    local_min[0] = Store();\n" +
                    "    local_min[0].value <== values[0];\n" +
                    "\n" +
                    "    for (var i = 1; i < length; i++) {\n" +
                    "        comparator[i] = LessThan(252);\n" +
                    "        comparator[i].in[0] <== values[i];\n" +
                    "        comparator[i].in[1] <== local_min[i - 1].stored_value;\n" +
                    "\n" +
                    "        local_min[i] = Store();\n" +
                    "        sum[i] = Sum();\n" +
                    "        sum[i].in[0] <== values[i] * comparator[i].out;\n" +
                    "        sum[i].in[1] <== local_min[i - 1].stored_value * (1 - comparator[i].out);\n" +
                    "        local_min[i].value <== sum[i].sum;\n" +
                    "    }\n" +
                    "    min <== local_min[length - 1].stored_value;\n" +
                    "}";
        }

        public static string calling(string[] values) {
            string result =  $"    signal output min_{++counter}_output;\n" +
                $"    component comparator_min_{counter} = Minimum({values.Length});\n";
            for (int i = 0; i < values.Length; i++) {
                result += $"    comparator_min_{counter}.values[{i}] <== {values[i]};\n";
            }
            result += $"    min_{counter}_output <== comparator_min_{counter}.min;\n";
            return  result;
        }
    }
}