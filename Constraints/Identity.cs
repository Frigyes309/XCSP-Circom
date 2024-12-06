namespace Converter.Constraints
{
    static class Identity
    {
        public static string component() {
            return "template Store() { \n" +
                   "    signal input value;\n" +
                    "    signal output stored_value;\n" +
                    "    stored_value <== value;\n" +
                    "}\n";
        }
    }
}