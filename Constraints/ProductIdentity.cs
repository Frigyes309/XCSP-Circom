namespace Converter.Constraints
{
    static class ProductIdentity
    {
        public static string component()
        {
            return "template Product() { \n" +
                   "    signal input in[2];\n" +
                   "    signal output product;\n" +
                   "    product <== in[0] * in[1];\n" +
                   "}";
        }
    }
}