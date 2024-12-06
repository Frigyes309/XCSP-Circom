namespace Converter.Constraints
{
    public static class Channel {

        static int counter = 0;
        public static string component()
        {
            return "template Channel(length) {\n" +
                "    signal input array1[length];\n" +
                "    signal input array2[length];\n" +
                "    signal output value;\n" +
                "    component equal[length * length * 3];\n" +
                "    component identity[length * length + 1];\n" +
                "    identity[0] = Store();\n" +
                "    identity[0].value <== 1;\n\n" +

                "    for (var i = 0; i < length; i++) {\n" +
                "        for (var j = 0; j < length; j++) {\n" +
                "            equal[(i * length + j) * 3] = IsEqual();\n" +
                "            equal[(i * length + j) * 3].in[0] <== array1[i];\n" +
                "            equal[(i * length + j) * 3].in[1] <== array2[j];\n\n" +

                "            equal[(i * length + j) * 3 + 1] = IsEqual();\n" +
                "            equal[(i * length + j) * 3 + 1].in[0] <== array2[j];\n" +
                "            equal[(i * length + j) * 3 + 1].in[1] <== array1[i];\n\n" +

                "            equal[(i * length + j) * 3 + 2] = IsEqual();\n" +
                "            equal[(i * length + j) * 3 + 2].in[0] <== equal[(i * length + j) * 3].out;\n" +
                "            equal[(i * length + j) * 3 + 2].in[1] <== equal[(i * length + j) * 3 + 1].out;\n\n" +

                "            identity[(i * length + j) + 1] = Store();\n" +
                "            identity[(i * length + j) + 1].value <== identity[(i * length + j)].stored_value * equal[(i * length + j) * 3 + 2].out;\n" +
                "        }\n" +
                "    }\n" +
                "    value <== identity[length * length].stored_value;\n" +
                "}";

        }

        public static string calling(string[] array1, string[] array2)
        {
            string result = $"    signal output channel_{++counter}_output;\n" +
                $"    component comparator_channel_{counter} = Channel({array1.Length});\n";
            for (int i = 0; i < array1.Length; i++)
            {
                result += $"    comparator_channel_{counter}.array1[{i}] <== {array1[i]};\n";
                result += $"    comparator_channel_{counter}.array2[{i}] <== {array2[i]};\n";
            }
            result += $"    channel_{counter}_output <== comparator_channel_{counter}.value;\n";
            return result;
        }
    }
}