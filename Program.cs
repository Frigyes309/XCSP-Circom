using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Converter.Constraints;
using System.Text.Json;

namespace Converter
{
    // Main Program
    class Program
    {
        static readonly string[] constraints = {"intension", "extension", "regular", "mdd", "allDifferent", "allEqual", "ordered", "sum", "count", "nValues", "cardinality", "minimum", "maximum", "element", "channel", "stretch", "noOverlap", "cumulative", "instantiation", "slide"};

        static void Main(string[] args)
        {
            // Output files
            const string outputJson = "output/input.json";
            const string circomOutput = "output/Converter.circom";

            // Read the solution file (.json)
            string solutionPath = "input/solution.json";
            Dictionary<string, string> solution = Solution(solutionPath);

            // Read the XCSP file
            string filePath = "input/constraint.xcsp";
            var filteredConstraints = Constraints(filePath);
            
            // Collect unique constraint types
            HashSet<string> constraintTypes = new HashSet<string>();

            // Collect unique variables
            HashSet<string> variables = new HashSet<string>();

            foreach (XmlNode constraint in filteredConstraints)
            {
                constraintTypes.Add(constraint.LocalName);
                switch (constraint.LocalName)
                {
                    case "allDifferent":
                    case "sum":
                        if (!constraintTypes.Contains("sumIdentity"))
                        {
                            constraintTypes.Add("sumIdentity");
                        }
                        break;
                    case "maximum":
                    case "minimum":
                    case "element":
                        if (!constraintTypes.Contains("sumIdentity"))
                        {
                            constraintTypes.Add("sumIdentity");
                        }
                        if (!constraintTypes.Contains("identity"))
                        {
                            constraintTypes.Add("identity");
                        }
                        break;
                    case "channel":
                    case "allEqual":
                        if (!constraintTypes.Contains("identity"))
                        {
                            constraintTypes.Add("identity");
                        }
                        break;
                    default:
                        break;
                }

                // Find the <list> child node and extract variables
                XmlNode listNode = constraint.SelectSingleNode("list");
                if (listNode != null)
                {
                    string variableList = listNode.InnerText.Trim();
                    variableList.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                .ToList()
                                .ForEach(variable => variables.Add(variable));
                }
            }

            // List found constraint types
            Console.WriteLine("Unique constraint types (which are recognized):");
            foreach (var type in constraintTypes)
            {
                Console.WriteLine($"- {type}");
            }
            List<string> output = new List<string>();
            output.Add("pragma circom 2.1.8;");
            output.Add("include \"circuits/comparators.circom\";\n");

            // Include the required components
            output = AddComponents(output, constraintTypes);

            // Create the input file
            string input = "{\n";
            foreach (var p in solution)
            {
                input += $"    \"{p.Key}\": \"{p.Value}\",\n";
            }
            input = input.Remove(input.Length - 2);
            input += "\n}";
            File.WriteAllText(outputJson, input);

            // Add the main component
            output.Add(Instantiation.startMainComponent());
            foreach (var i in solution)
            {
                output.Add($"    signal input {i.Key};");
            }

            // Insert the calling of the components
            foreach (var constraint in filteredConstraints)
            {
                string constraintType = constraint.LocalName;
                switch (constraintType)
                {
                    case "allDifferent":
                        output.Add(AllDifferent.calling(constraint.SelectNodes("list")[0].InnerText.Split(' ').Where(x => x != "").ToArray()));
                        break;
                    case "sum":
                        string[] coeffs;
                        if (constraint.SelectNodes("coeffs").Count != 0)
                        {
                            coeffs = constraint.SelectNodes("coeffs")[0].InnerText.Split(' ').Where(x => x != "").ToArray();
                            
                        }
                        else
                        {
                            coeffs = new string[constraint.SelectNodes("list")[0].InnerText.Split(' ').Where(x => x != "").ToArray().Length];
                            for (int i = 0; i < coeffs.Length; i++)
                            {
                                coeffs[i] = "1";
                            }
                        }
                        string equal = Condition(constraint.SelectNodes("condition")[0].InnerText).Split(' ')[1]; //TODO - Implement the Condition method
                        output.Add(Sum.calling(constraint.SelectNodes("list")[0].InnerText.Split(' ').Where(x => x != "").ToArray(), coeffs, equal));
                        break;
                    case "minimum":
                        output.Add(Minimum.calling(constraint.SelectNodes("list")[0].InnerText.Split(' ').Where(x => x != "").ToArray()));
                        break;
                    case "maximum":
                        output.Add(Maximum.calling(constraint.SelectNodes("list")[0].InnerText.Split(' ').Where(x => x != "").ToArray()));
                        break;
                    case "element":
                        string index = constraint.SelectNodes("index")[0].InnerText.Replace(" ", "");
                        string[] values = constraint.SelectNodes("list")[0].InnerText.Split(' ').Where(x => x != "").ToArray();
                        string value = constraint.SelectNodes("value")[0].InnerText.Replace(" ", "");
                        output.Add(Element.calling(index, values, value));
                        break;
                    case "channel":
                        output.Add(Channel.calling(constraint.SelectNodes("list")[0].InnerText.Split(' ').Where(x => x != "").ToArray(), constraint.SelectNodes("list")[1].InnerText.Split(' ').Where(x => x != "").ToArray()));
                        break;
                    case "identity":
                        output.Add(Identity.component());
                        break;
                    default:
                        break;
                }
            }

            output.Add("\n}\n\ncomponent main = Main();");

            // Close the input file

            // Write the output to a file
            File.WriteAllLines(circomOutput, output);
        }

        static List<XmlNode> Constraints(string filePath) {
        try
        {
            // XML dokumentum betöltése
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            XmlNodeList allConstraints = xmlDoc.SelectNodes("/instance/constraints/*");

            // Filter the nodes based on their local name
            var filteredConstraints = allConstraints.Cast<XmlNode>()
                .Where(node => constraints.Contains(node.LocalName))
                .ToList();

            
            return filteredConstraints;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba történt a fájl feldolgozása során: {ex.Message}");
            return null;
        }
    }

        static List<string> AddComponents(List<string> output, HashSet<string> filteredConstraints)
        {
            // Add the components to the output
            foreach (var constraint in filteredConstraints)
            {
                switch (constraint)
                {
                    case "identity":
                        output.Add(Identity.component() + "\n");
                        break;
                    case "allDifferent":
                        output.Add(AllDifferent.component() + "\n");
                        break;
                    case "sum":
                        output.Add(Sum.component() + "\n");
                        break;
                    case "sumIdentity":
                        output.Add(SumIdentity.component() + "\n");
                        break;
                    case "count":
                        Console.WriteLine("Count is not supported yet.");
                        break;
                    case "nValues":
                        Console.WriteLine("nValues is not supported yet.");
                        break;
                    case "cardinality":
                        Console.WriteLine("Cardinality is not supported yet.");
                        break;
                    case "minimum":
                        output.Add(Minimum.component() + "\n");
                        break;
                    case "maximum":
                        output.Add(Maximum.component() + "\n");
                        break;
                    case "element":
                        output.Add(Element.component() + "\n");
                        break;
                    case "channel":
                        output.Add(Channel.component() + "\n");
                        break;
                    case "stretch":
                        Console.WriteLine("Stretch is not supported yet.");
                        break;
                    case "noOverlap":
                        Console.WriteLine("NoOverlap is not supported yet.");
                        break;
                    case "cumulative":
                        Console.WriteLine("Cumulative is not supported yet.");
                        break;
                    // This is called IsEqual()
                    /*case "instantiation":
                        string[] names = constraint.SelectNodes("list")[0].InnerText.Split(' ').Where(x => x != "").ToArray();
                        string[] values = constraint.SelectNodes("values")[0].InnerText.Split(' ').Where(x => x != "").ToArray();
                        Instantiation.addComponent(names, values);
                        break;*/
                    case "slide":
                        Console.WriteLine("Slide is not supported yet.");
                        break;
                    case "mdd":
                        Console.WriteLine("MDD is not supported yet.");
                        break;
                    case "regular":
                        Console.WriteLine("Regular is not supported yet.");
                        break;
                    case "allEqual":
                        output.Add(AllEqual.component() + "\n");
                        break;
                    case "ordered":
                        Console.WriteLine("Ordered is not supported yet.");
                        break;
                    default:
                        break;
                }
            }

            return output;
        }
    
        static Dictionary<string, string> Solution(string filePath)
        {
            try
            {
                // Read the solution file
                string solutionContent = File.ReadAllText(filePath);

                var parametersDict = JsonSerializer.Deserialize<Dictionary<string, string>>(solutionContent);

                // Convert dictionary to list of Parameter structs
                /*List<Parameter> parameters = new List<Parameter>();
                foreach (var kvp in parametersDict)
                {
                    parameters.Add(new Parameter(kvp.Key, kvp.Value));
                }*/

                return parametersDict;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the solution file: {ex.Message}");
                return null;
            }
        }
    
    //TODO - Implement the Condition method
        static string Condition(string text) {
            char[] unwantedChars = { '(', ')', ' ' };
            string result = "";

            foreach (char c in text)
            {
                if (Array.IndexOf(unwantedChars, c) == -1)
                {
                    result += c;
                }
            }

            return result.Replace(",", " ");
        }
    }
}