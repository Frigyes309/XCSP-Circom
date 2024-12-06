namespace Converter.Constraints {
    static class AllEqual {

        static int counter = 0;
        public static string component() {
            return "template AllEqual(length) {\n" +
                "    signal input values[length];\n" +
                "    signal output out;\n" +
                "    component comparator[length - 1];\n" +
                "    component identity[length];\n" +
                "    identity[0] = Store();\n" +
                "    identity[0].value <== 1;\n\n" +

                "    for (var i = 1; i < length; i++) {\n" +
                "        comparator[i - 1] = IsEqual();\n" +
                "        comparator[i - 1].in[0] <== values[0];\n" +
                "        comparator[i - 1].in[1] <== values[i];\n\n" +

                "        identity[i] = Store();\n" +
                "        identity[i].value <== comparator[i - 1].out * identity[i - 1].stored_value;\n" +
                "    }\n" +
                "    out <== identity[length - 1].stored_value;\n" +
                "}";
        }

        public static string calling(string[] values) {
            string result = $"    signal output all_equal_{++counter}_output;\n" +
                $"    component comparator_all_equal_{counter} = AllEqual({values.Length});\n";
            for (int i = 0; i < values.Length; i++) {
                result += $"    comparator_all_equal_{counter}.values[{i}] <== {values[i]};\n";
            }
            result += $"    all_equal_{counter}_output <== comparator_all_equal_{counter}.out;\n";
            return result;
        }
    }   
}