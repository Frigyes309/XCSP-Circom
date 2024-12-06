using System.IO;

namespace Converter.Constraints
{
    static public class Instantiation
    {
        private static List<string> initiations = new List<string>();
        private static List<string> input = new List<string>();

        public static string startMainComponent()
        {
            string result = "template Main() {\n";
            foreach (string i in initiations)
            {
                result += i;
            }
            return result;
        }
    
        public static void createInput()
        {
            string result = "";
            foreach (string i in input)
            {
                result += i;
            }
            File.AppendAllText("input.json", result);
        }
    }
}