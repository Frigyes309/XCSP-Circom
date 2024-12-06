namespace Converter.Constraints {
    static class AllDifferent {

        static int counter = 0;
        public static string component() {
            return "template AllDifferent(length) { \n" +
                "    signal input values[length];\n" +
                "    signal output different;\n" +
                "\n" +
                "    component comparator[length * (length - 1) / 2];\n" +
                "    component sum[length * (length - 1) / 2 + 1];\n" +
                "    var index = 0;\n" +
                "    sum[0] = Sum();\n" +
                "    sum[0].in[0] <== 0;\n" +
                "    sum[0].in[1] <== 0;\n" +
                "\n" +
                "    for (var i = 0; i < length; i++) {\n" +
                "        for (var j = i + 1; j < length; j++) {\n" +
                "            comparator[index] = IsEqual();\n" +
                "            comparator[index].in[0] <== values[i];\n" +
                "            comparator[index].in[1] <== values[j];\n" +
                "            sum[index + 1] = Sum();\n" +
                "            sum[index + 1].in[0] <== sum[index].sum;\n" +
                "            sum[index + 1].in[1] <== comparator[index].out;\n" +
                "            index += 1;\n" +
                "        }\n" +
                "    }\n" +
                "    component helper = IsEqual();\n" +
                "    helper.in[0] <== sum[index].sum;\n" +
                "    helper.in[1] <== 0;\n" +
                "    different <== helper.out;\n" +
                "}";
            }

        public static string calling(string[] values) {
            string result =  $"    signal output all_different_{++counter}_output;\n" +
                $"    component comparator_all_different_{counter} = AllDifferent({values.Length});\n";
            for (int i = 0; i < values.Length; i++) {
                result += $"    comparator_all_different_{counter}.values[{i}] <== {values[i]};\n";
            }
            result += $"    all_different_{counter}_output <== comparator_all_different_{counter}.different;\n";
            return  result;
        }
    }   
}