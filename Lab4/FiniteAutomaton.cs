using System;
using System.Collections.Generic;
using System.IO;

public class FiniteAutomaton
{
    private const string ELEM_SEPARATOR = ".";
    private bool isDeterministic;
    private string initialState;
    private List<string> states;
    private List<string> alphabet;
    private List<string> finalStates;
    private readonly Dictionary<Tuple<string, string>, HashSet<string>> transitions;

    private void ReadFromFile(string filePath)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                states = new List<string>(reader.ReadLine().Split(ELEM_SEPARATOR));
                initialState = reader.ReadLine();
                alphabet = new List<string>(reader.ReadLine().Split(ELEM_SEPARATOR));
                finalStates = new List<string>(reader.ReadLine().Split(ELEM_SEPARATOR));

                transitions.Clear();

                while (!reader.EndOfStream)
                {
                    string transitionLine = reader.ReadLine();
                    string[] transitionComponents = transitionLine.Split(' ');

                    if (states.Contains(transitionComponents[0]) &&
                        states.Contains(transitionComponents[2]) &&
                        alphabet.Contains(transitionComponents[1]))
                    {
                        var transitionStates = new Tuple<string, string>(transitionComponents[0], transitionComponents[1]);

                        if (!transitions.ContainsKey(transitionStates))
                        {
                            var transitionStatesSet = new HashSet<string>();
                            transitionStatesSet.Add(transitionComponents[2]);
                            transitions.Add(transitionStates, transitionStatesSet);
                        }
                        else
                        {
                            transitions[transitionStates].Add(transitionComponents[2]);
                        }
                    }
                }
            }
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }

        isDeterministic = CheckIfDeterministic();
    }

    public FiniteAutomaton(string filePath)
    {
        transitions = new Dictionary<Tuple<string, string>, HashSet<string>>();
        ReadFromFile(filePath);
    }

    public bool CheckIfDeterministic()
    {
        return transitions.Values.All(set => set.Count <= 1);
    }

    public List<string> GetStates()
    {
        return states;
    }

    public string GetInitialState()
    {
        return initialState;
    }

    public List<string> GetAlphabet()
    {
        return alphabet;
    }

    public List<string> GetFinalStates()
    {
        return finalStates;
    }

    public Dictionary<Tuple<string, string>, HashSet<string>> GetTransitions()
    {
        return transitions;
    }

    public string WriteTransitions()
    {
        var builder = new System.Text.StringBuilder();
        builder.Append("Transitions: \n");
        foreach (var kvp in transitions)
        {
            builder.Append($"<{kvp.Key.Item1},{kvp.Key.Item2}> -> {string.Join(", ", kvp.Value)}\n");
        }
        return builder.ToString();
    }

    public bool AcceptsSequence(string sequence)
    {
        if (!isDeterministic)
        {
            return false;
        }

        if (sequence.Length == 0)
        {
            return finalStates.Contains(initialState);
        }

        string currentState = initialState;

        foreach (char symbol in sequence)
        {
            var transition = new Tuple<string, string>(currentState, symbol.ToString());

            if (!transitions.ContainsKey(transition))
            {
                return false;
            }

            currentState = transitions[transition].First();
        }

        return finalStates.Contains(currentState);
    }
}
