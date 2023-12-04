using System;
using System.Collections.Generic;

class MainClass
{
    private static void PrintToFile(string filePath, object obj)
    {
        try
        {
            using (var printStream = new System.IO.StreamWriter(filePath))
            {
                printStream.WriteLine(obj);
            }
        }
        catch (System.IO.FileNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public static void PrintMenu()
    {
        Console.WriteLine("\n\n0. Exit");
        Console.WriteLine("1. Print non-terminals");
        Console.WriteLine("2. Print terminals");
        Console.WriteLine("3. Print starting symbol");
        Console.WriteLine("4. Print all productions");
        Console.WriteLine("5. Print all productions for a non terminal");
        Console.WriteLine("6. Is the grammar a context-free grammar (CFG)?");
    }

    public static void RunGrammar()
    {
        Grammar grammar = new Grammar("D:\\UBB-Projects\\FLCD\\Lab5\\Parser\\G2.in");
        bool notStopped = true;
        while (notStopped)
        {
            PrintMenu();
            Console.WriteLine("Enter your option");
            int option = Convert.ToInt32(Console.ReadLine());
            switch (option)
            {
                case 0:
                    notStopped = false;
                    break;
                case 1:
                    Console.WriteLine($"\n\nNon-terminals ->  + ${grammar.GetNonTerminals()}");
                    break;
                case 2:
                    Console.WriteLine($"\n\nTerminals ->  + ${grammar.GetTerminals()}");
                    break;
                case 3:
                    Console.WriteLine($"\n\nStarting symbol ->  + ${grammar.GetStartingSymbol()}");
                    break;
                case 4:
                    Console.WriteLine("\n\nAll productions: ");

                    foreach (var kvp in grammar.GetProductions())
                    {
                        var lhs = kvp.Key;
                        var rhs = kvp.Value;
                        Console.WriteLine($"{lhs} -> {rhs}");
                    }
                    Console.WriteLine("Invalid command!");
                    break;
                case 5:
                    Console.Write("Enter a non-terminal: ");
                    string nonTerminal = Console.ReadLine();
                    Console.WriteLine($"\n\nProductions for the non-terminal: {nonTerminal}");
                    List<string> key = new List<string> { nonTerminal };
                    try
                    {
                        foreach (var rhs in grammar.GetProductions()[key])
                        {
                            Console.WriteLine($"{string.Join(" ", key)} -> {string.Join(", ", rhs)}");
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        Console.WriteLine("This is not a defined non-terminal");
                    }
                    break;
                case 6:
                    Console.WriteLine($"\n\nIs it a context-free grammar (CFG)? {grammar.IsCFG()}");
                    break;
            }
        }
    }

    public static void LaunchApp()
    {
        Console.WriteLine("0. Exit");
        Console.WriteLine("1. Grammar");
        Console.WriteLine("Your option: ");

        int option = Convert.ToInt32(Console.ReadLine());

        switch (option)
        {
            case 1:
                RunGrammar();
                break;
            case 0:
                break;
            default:
                Console.WriteLine("Invalid command!");
                break;
        }
    }

    public static void Main(string[] args)
    {
        LaunchApp();
    }
}
