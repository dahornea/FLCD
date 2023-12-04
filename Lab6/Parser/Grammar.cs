using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Grammar
{
    public List<string> N { get; set; }
    public List<string> E { get; set; }
    public Dictionary<string, List<(string, int)>> P { get; set; }
    public string S { get; set; }

    public Grammar(List<string> N, List<string> E, Dictionary<string, List<(string, int)>> P, string S)
    {
        this.N = N;
        this.E = E;
        this.P = P;
        this.S = S;
    }

    public static bool Validate(List<string> N, List<string> E, Dictionary<string, List<(string, int)>> P, string S)
    {
        if (!N.Contains(S))
            return false;

        foreach (var key in P.Keys)
        {
            if (!N.Contains(key))
                return false;

            foreach (var move in P[key])
            {
                foreach (var ch in move.Item1)
                {
                    if (!N.Contains(ch.ToString()) && !E.Contains(ch.ToString()) && ch != 'E')
                        return false;

                }
            }
        }

        return true;
    }

    public static List<string> ParseLine(string line)
    {
        return line.Trim().Split('=')[1].Trim().Substring(1, line.Length - 2).Trim().Split(',').Select(value => value.Trim()).ToList();
    }

    public static Grammar FromFile(string fileName)
    {
        using (StreamReader file = new StreamReader(fileName))
        {
            List<string> N = ParseLine(file.ReadLine());
            List<string> E = ParseLine(file.ReadLine());
            string S = file.ReadLine().Split('=')[1].Trim();
            Dictionary<string, List<(string, int)>> P = ParseRules(ParseLine(string.Concat(file.ReadToEnd())));

            if (!Validate(N, E, P, S))
            {
                throw new Exception("Wrong input file.");
            }

            return new Grammar(N, E, P, S);
        }
    }

    public static Dictionary<string, List<(string, int)>> ParseRules(List<string> rules)
    {
        Dictionary<string, List<(string, int)>> result = new Dictionary<string, List<(string, int)>>();
        int index = 1;

        foreach (var rule in rules)
        {
            var parts = rule.Split(new[] { "->" }, StringSplitOptions.None);
            var lhs = parts[0].Trim();
            var rhs = parts[1].Split('|').Select(value => value.Trim()).ToList();

            foreach (var value in rhs)
            {
                if (result.ContainsKey(lhs))
                {
                    result[lhs].Add((value, index));
                }
                else
                {
                    result[lhs] = new List<(string, int)> { (value, index) };
                }
                index++;
            }
        }

        return result;
    }

    public string[] SplitRhs(string prod)
    {
        return prod.Split(' ');
    }

    public bool IsNonTerminal(string value)
    {
        return N.Contains(value);
    }

    public bool IsTerminal(string value)
    {
        return E.Contains(value);
    }

    public List<(string, int)> GetProductionsFor(string nonTerminal)
    {
        if (!IsNonTerminal(nonTerminal))
        {
            throw new Exception("Can only show productions for non-terminals");
        }
        return P.ContainsKey(nonTerminal) ? P[nonTerminal] : null;
    }

    public (string, string) GetProductionForIndex(int index)
    {

        foreach (var kvp in P)
        {
            var key = kvp.Key;
            var value = kvp.Value;

            foreach (var v in value)
            {
                if (v.Item2 == index)
                {
                    return (key, v.Item1);
                }
            }
        }
        return (null, null);

    }

    public override string ToString()
    {
        return "N = { " + string.Join(", ", N) + " }\n"
             + "E = { " + string.Join(", ", E) + " }\n"
             + "P = { " + string.Join(", ", P.Select(prod => $"{prod.Key} -> {string.Join(" | ", prod.Value.Select(v => v.Item1))}")) + " }\n"
             + "S = " + S + "\n";
    }
}
