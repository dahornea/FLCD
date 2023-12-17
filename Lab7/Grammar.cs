using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lab7
{
    public class Grammar
    {
        public const string EPSILON = "epsilon";
        public string STARTING_SYMBOL = "";

        public List<string> N { get; private set; }
        public List<string> E { get; private set; }
        public string S { get; private set; }
        public Dictionary<string, List<List<string>>> P { get; private set; }
        public bool IsEnhanced { get; private set; }

        public Grammar(bool isEnhanced = false)
        {
            N = new List<string>();
            E = new List<string>();
            S = "";
            P = new Dictionary<string, List<List<string>>>();
            IsEnhanced = isEnhanced;
        }

        public bool CheckIfGrammarIsEnhanced()
        {
            // Check that the starting symbol has only one production
            if (P[S].Count != 1)
            {
                return false;
            }

            // Check that the starting symbol does not appear on the right hand side of any production
            foreach (var production in P.Values)
            {
                foreach (var rhs in production)
                {
                    if (rhs.Contains(S))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void MakeEnhancedGrammar()
        {
            if (!IsEnhanced)
            {
                // Add a new non-terminal symbol S'
                N.Add(STARTING_SYMBOL);
                // Add a new production S' -> S
                P[STARTING_SYMBOL] = new List<List<string>> { new List<string> { S } };
                // Change the starting symbol to Z
                S = STARTING_SYMBOL;
                IsEnhanced = true;
            }
        }

        private static List<string> ProcessLine(string line)
        {
            // Get what comes after the '='
            return new List<string>(line.Trim().Split(' ').Skip(2));
        }

        public void ReadFromFile(string fileName)
        {
            using (StreamReader file = new StreamReader(fileName))
            {
                N = ProcessLine(file.ReadLine());
                E = ProcessLine(file.ReadLine());
                S = ProcessLine(file.ReadLine())[0];

                file.ReadLine();  // P =

                // Get all transitions
                P = new Dictionary<string, List<List<string>>>();
                while (!file.EndOfStream)
                {
                    string[] split = file.ReadLine().Split(new[] { "->" }, StringSplitOptions.None);
                    string source = split[0].Trim();
                    string sequence = split[1].TrimStart();

                    List<string> sequenceList = new List<string>(sequence.Split(' '));

                    if (P.ContainsKey(source))
                    {
                        P[source].Add(sequenceList);
                    }
                    else
                    {
                        P[source] = new List<List<string>> { sequenceList };
                    }
                }
            }
        }

        public Tuple<string, string> GetProductionById(int? prodId)
        {
            foreach (var prod in P.Keys)
            {
                foreach (var prodValue in P[prod])
                {
                    if (int.TryParse(prodValue[1], out int intValue) && intValue == prodId)
                    {

                        return Tuple.Create(prod, prodValue[0]);
                    }
                }
            }
            return null;
        }

        public bool CheckCfg()
        {
            bool hasStartingSymbol = false;

            foreach (string key in P.Keys)
            {
                if (key == S)
                {
                    hasStartingSymbol = true;
                }

                if (!N.Contains(key))
                {
                    return false;
                }
            }

            if (!hasStartingSymbol)
            {
                return false;
            }

            foreach (var production in P.Values)
            {
                foreach (var rhs in production)
                {
                    foreach (var value in rhs)
                    {
                        if (!N.Contains(value) && !E.Contains(value) && value != EPSILON)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public override string ToString()
        {
            string result = "N = " + string.Join(", ", N) + "\n";
            result += "E = " + string.Join(", ", E) + "\n";
            result += "S = " + S + "\n";
            result += "P = { ";
            foreach (var entry in P)
            {
                result += entry.Key.ToString() + " -> " + string.Join(" | ", entry.Value.ToString()) + ", ";
            }
            result = result.TrimEnd(',', ' ') + " }\n";
            return result;
        }
    }
}