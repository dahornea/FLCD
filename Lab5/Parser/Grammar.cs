using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Grammar
{
    private readonly string ELEMENT_SEPARATOR = "&";
    private readonly string SEPARATOR_OR_TRANSITION = "\\|";
    private readonly string TRANSITION_CONCATENATION = " ";
    private readonly string EPSILON = "EPS";
    private readonly string SEPARATOR_LEFT_RIGHT_HAND_SIDE = "->";

    // LR(0)
    private HashSet<string> nonTerminals;
    private HashSet<string> terminals;
    private Dictionary<List<string>, HashSet<List<string>>> productions;
    private string startingSymbol;
    private bool isCFG;

    private void ProcessProduction(string production)
    {
        string[] leftAndRightHandSide = production.Split(new[] { SEPARATOR_LEFT_RIGHT_HAND_SIDE }, StringSplitOptions.None);
        List<string> splitLHS = leftAndRightHandSide[0].Split(new[] { TRANSITION_CONCATENATION }, StringSplitOptions.None).ToList();
        string[] splitRHS = leftAndRightHandSide[1].Split(new[] { SEPARATOR_OR_TRANSITION }, StringSplitOptions.None);


        if (!productions.ContainsKey(splitLHS))
        {
            productions[splitLHS] = new HashSet<List<string>>();
        }

        for (int i = 0; i < splitRHS.Length; i++)
        {
            var rhsList = splitRHS[i].Split(new[] { TRANSITION_CONCATENATION }, StringSplitOptions.None).ToList();

            productions[splitLHS].Add(rhsList);
        }
    }

    private void LoadFromFile(string filePath)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                nonTerminals = new HashSet<string>(reader.ReadLine().Split(new[] { ELEMENT_SEPARATOR }, StringSplitOptions.None));
                terminals = new HashSet<string>(reader.ReadLine().Split(new[] { ELEMENT_SEPARATOR }, StringSplitOptions.None));
                startingSymbol = reader.ReadLine();

                productions = new Dictionary<List<string>, HashSet<List<string>>>();

                while (!reader.EndOfStream)
                {
                    ProcessProduction(reader.ReadLine());
                }

                isCFG = CheckIfCFG();
            }
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private bool CheckIfCFG()
    {
        if (!nonTerminals.Contains(startingSymbol))
        {
            return false;
        }

        foreach (var leftHandSide in productions.Keys)
        {
            if (leftHandSide.Count != 1 || !nonTerminals.Contains(leftHandSide[0]))
            {
                return false;
            }

            foreach (var possibleNextMoves in productions[leftHandSide])
            {
                foreach (var possibleNextMove in possibleNextMoves)
                {
                    if (!nonTerminals.Contains(possibleNextMove) && !terminals.Contains(possibleNextMove) && possibleNextMove != EPSILON)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public Grammar(string filePath)
    {
        LoadFromFile(filePath);
    }

    public HashSet<string> GetNonTerminals()
    {
        return nonTerminals;
    }

    public HashSet<string> GetTerminals()
    {
        return terminals;
    }

    public Dictionary<List<string>, HashSet<List<string>>> GetProductions()
    {
        return productions;
    }

    public string GetStartingSymbol()
    {
        return startingSymbol;
    }

    public bool IsCFG()
    {
        return isCFG;
    }
}
