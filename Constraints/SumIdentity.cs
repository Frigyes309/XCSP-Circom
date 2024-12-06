namespace Converter.Constraints
{
    static class SumIdentity
    {
        public static string component()
        {
            return "template Sum() { \n" +
                   "    signal input in[2];\n" +
                   "    signal output sum;\n" +
                   "    sum <== in[0] + in[1];\n" +
                   "}";
        }
    }
}