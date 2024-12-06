namespace Converter.Constraints {
    static class Element {

        static int counter = 0;
        static public string component() {
            return "template Element(length) {\n" +
                "    signal input index;\n" +
                "    signal input values[length];\n" +
                "    signal input value;\n" +
                "    signal output equal;\n" +
                "    component value_equal[length];\n" +
                "    component index_equal[length];\n" +
                "    component sum[length + 1];\n" +
                "    sum[0] = Sum();\n" +
                "    sum[0].in[0] <== 0;\n" +
                "    sum[0].in[1] <== 0;\n\n" +

                "    for (var i = 0; i < length; i++) {\n" +
                "        value_equal[i] = IsEqual();\n" +
                "        value_equal[i].in[0] <== values[i];\n" +
                "        value_equal[i].in[1] <== value;\n\n" +

                "        index_equal[i] = IsEqual();\n" +
                "        index_equal[i].in[0] <== i;\n" +
                "        index_equal[i].in[1] <== index;\n\n" +

                "        sum[i + 1] = Sum();\n" +
                "        sum[i + 1].in[0] <== sum[i].sum;\n" +
                "        sum[i + 1].in[1] <== value_equal[i].out * index_equal[i].out;\n" +
                "    }\n" +
                "    equal <== sum[length].sum;\n" +
                "}";

        }

        static public string calling(string index, string[] values, string value){
            string result = $"    signal output element_{++counter}_output;\n" +
                $"    component comparator_element_{counter} = Element({values.Length});\n" +
                $"    comparator_element_{counter}.index <== {index};\n";
            for (int i = 0; i < values.Length; i++) {
                result += $"    comparator_element_{counter}.values[{i}] <== {values[i]};\n";
            }
            result += $"    comparator_element_{counter}.value <== {value};\n";
            result += $"    element_{counter}_output <== comparator_element_{counter}.equal;\n";
            return  result;
        }
    }
}