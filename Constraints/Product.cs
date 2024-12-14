namespace Converter.Constraints
{
    static class Product
    {
        static int counter = 0;
        public static string component()
        {
            return "template Product2(length) { \n" +
                   "    signal input in[length];\n" +
                   "    signal output product;\n" +
                   "    component subProduct[length - 1]; //We do not use the whole array, but this way the indexing will be fine\n" +
                   "    subProduct[0] = Product();\n" +
                   "    subProduct[0].in[0] <== in[0];\n" +
                   "    subProduct[0].in[1] <== in[1];\n" +
                   "    for (var i = 2; i < length; i++) {\n" +
                   "        subProduct[i - 1] = Product();\n" +
                   "        subProduct[i - 1].in[0] <== in[i];\n" +
                   "        subProduct[i - 1].in[1] <== subProduct[i - 2].product;\n" +
                   "    }\n" +
                   "    product <== subProduct[length - 2].product;\n" +
                   "}";
        }

        public static string calling(string[] values, string equal) {
            string result =  $"    signal output product_{++counter}_output;\n" +
                $"    component comparator_product_{counter} = Product2({values.Length});\n";
            for (int i = 0; i < values.Length; i++) {
                result += $"    comparator_product_{counter}.in[{i}] <== {values[i]};\n";
            }
            result += $"    component comparator_product_equality_{counter} = {Equality(equal)};\n";
            result += $"    comparator_product_equality_{counter}.in[0] <== comparator_product_{counter}.product;\n";
            result += $"    comparator_product_equality_{counter}.in[1] <== {equal};\n";
            result += $"    product_{counter}_output <== comparator_product_equality_{counter}.out;\n";
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