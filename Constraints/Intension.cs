namespace Converter.Constraints
{
    static class Intension
    {
        static int counter = 0;
        public static string component(string[] inputs, string[] components, string connection, string value, string id)
        {
            string intension = $"template Intension_{id}";
            for (int i = 0; i < components.Length; i++) {
                intension += $"_{components[i]}";
            }
            intension += $"_{connection}() {{\n" +
                $"    signal input inputs[3];\n" +
                $"    signal input rating;\n" +
                $"    signal output out;\n";
            for (int i = 0; i < components.Length; i++) {
                intension += $"\n    component comparator_{i} = {Equality(components[i])};\n" +
                    $"    comparator_{i}.in[0] <== inputs[{i}];\n" +
                    $"    comparator_{i}.in[1] <== {inputs[i]};";
            }
            
            if (connection == "and") {
                intension += $"\n    component product = Product2({components.Length + 1});\n";
                for (int i = 0; i < components.Length; i++) {
                    intension += $"    product.in[{i}] <== comparator_{i}.out;\n";
                }
                intension += $"    component equal = IsEqual();\n" +
                    $"    equal.in[0] <== rating;\n" +
                    $"    equal.in[1] <== {value};\n" +
                    $"    product.in[{components.Length}] <== equal.out;\n" +
                    $"    out <== product.product";
            } else {
                intension += $"\n    component product = Product2({components.Length + 1});\n";
                for (int i = 0; i < components.Length; i++) {
                    intension += $"    product.in[{i}] <== comparator_{i}.out;\n";
                }
                intension += $"    component equal = IsEqual();\n" +
                    $"    equal.in[0] <== rating;\n" +
                    $"    equal.in[1] <== {value};\n" +
                    $"    product.in[{components.Length}] <== equal.out;\n";
                intension += $"\n    component or = LessEqThan(252);" +
                    $"\n    or.in[0] <== 1;" +
                    $"\n    or.in[1] <== product.product;" +
                    $"\n    out <== or.out";
            }
            intension += ";\n}\n";
            return intension;
        }

        public static string calling(string id, string[] components, string[] name_gen) {
            string name = $"{id}";
            for (int i = 0; i < name_gen.Length; i++) {
                name += $"_{name_gen[i]}";
            }
            string call = $"    signal output intension_{name}_{++counter}_output;\n" +
                $"    component comparator_intension_{counter} = Intension_{name}();\n" +
                $"    comparator_intension_{counter}.rating <== rating;\n";
            for (int i = 0; i < components.Length - 1; i++) {
                call += $"    comparator_intension_{counter}.inputs[{i}] <== {components[i]};\n";
            }
            call += $"    intension_{name}_{counter}_output <== comparator_intension_{counter}.out;\n";
            return call;
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