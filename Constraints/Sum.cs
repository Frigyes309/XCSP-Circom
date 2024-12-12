namespace Converter.Constraints
{
    static class Sum
    {
        static int counter = 0;
        public static string component()
        {
            return "template Sum2(length) { \n" +
                   "    signal input in[length];\n" +
                   "    signal input coeffs[length];\n" +
                   "    signal output sum;\n" +
                   "    component subSum[length - 1]; //We do not use the whole array, but this way the indexing will be fine\n" +
                   "    subSum[0] = Sum();\n" +
                   "    subSum[0].in[0] <== in[0] * coeffs[0];\n" +
                   "    subSum[0].in[1] <== in[1] * coeffs[1];\n" +
                   "    for (var i = 2; i < length; i++) {\n" +
                   "        subSum[i - 1] = Sum();\n" +
                   "        subSum[i - 1].in[0] <== in[i] * coeffs[i];\n" +
                   "        subSum[i - 1].in[1] <== subSum[i - 2].sum;\n" +
                   "    }\n" +
                   "    sum <== subSum[length - 2].sum;\n" +
                   "}";
        }

        public static string calling(string[] values, string[] coeffs, string equal) {
            string result =  $"    signal output sum_{++counter}_output;\n" +
                $"    component comparator_sum_{counter} = Sum2({values.Length});\n";
            for (int i = 0; i < values.Length; i++) {
                result += $"    comparator_sum_{counter}.in[{i}] <== {values[i]};\n";
                result += $"    comparator_sum_{counter}.coeffs[{i}] <== {coeffs[i]};\n";
            }
            result += $"    component comparator_sum_equality_{counter} = {Equality(equal)};\n";
            result += $"    comparator_sum_equality_{counter}.in[0] <== comparator_sum_{counter}.sum;\n";
            result += $"    comparator_sum_equality_{counter}.in[1] <== {equal};\n";
            result += $"    sum_{counter}_output <== comparator_sum_equality_{counter}.out;\n";
            return  result;
        }

        private static string Equality(string equal) {
            switch (equal) {
                case "eq":
                    return "IsEqual()";
                case "gt":
                    return "GreaterThan(252)";
                case "lt":
                    return "LessThan(252)";
                case "ge":
                    return "GreaterEqThan(252)";
                case "le":
                    return "LessEqThan(252)";
                default:
                    return "IsEqual()";
            }
        }
    }
}