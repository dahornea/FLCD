using System;
using System.IO;
using System.Runtime.InteropServices;
using Lab2bun;

public class Program
{
    private static void PrintToFile(string filePath, object obj)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(obj);
            }
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void Run(string filePath)
    {
        MyScanner scanner = new MyScanner(filePath);
        scanner.Scan();
        PrintToFile(filePath.Replace(".txt", "ST.txt"), scanner.GetSymbolTable());
        PrintToFile(filePath.Replace(".txt", "PIF.txt"), scanner.GetPif());
    }

    private static void RunScanner()
    {
        Run("D:\\UBB-Projects\\FLCD\\Lab3\\input_files\\p1.txt");
        Run("D:\\UBB-Projects\\FLCD\\Lab3\\input_files\\p2.txt");
        Run("D:\\UBB-Projects\\FLCD\\Lab3\\input_files\\p3.txt");
        Run("D:\\UBB-Projects\\FLCD\\Lab3\\input_files\\p1err.txt");
    }

public class DFAOptions
{
    private static void PrintMenu()
    {
        Console.WriteLine("1. Print states.");
        Console.WriteLine("2. Print alphabet.");
        Console.WriteLine("3. Print final states.");
        Console.WriteLine("4. Print transitions.");
        Console.WriteLine("5. Print initial state.");
        Console.WriteLine("6. Print is deterministic.");
        Console.WriteLine("7. Check if sequence is accepted by DFA.");
    }

    public static void OptionsForDFA()
    {
        FiniteAutomaton finiteAutomaton = new FiniteAutomaton("input_files/FA.txt");

        Console.WriteLine("FA read from file.");
        PrintMenu();
        Console.WriteLine("Your option: ");

        int option = int.Parse(Console.ReadLine());

        while (option != 0)
        {
            switch (option)
            {
                case 1:
                    Console.WriteLine("Final states: ");
                    Console.WriteLine(string.Join(", ", finiteAutomaton.GetStates()));
                    Console.WriteLine();
                    break;

                case 2:
                    Console.WriteLine("Alphabet: ");
                    Console.WriteLine(string.Join(", ", finiteAutomaton.GetAlphabet()));
                    Console.WriteLine();
                    break;

                case 3:
                    Console.WriteLine("Final states: ");
                    Console.WriteLine(string.Join(", ", finiteAutomaton.GetFinalStates()));
                    Console.WriteLine();
                    break;

                case 4:
                    Console.WriteLine(finiteAutomaton.WriteTransitions());
                    break;

                case 5:
                    Console.WriteLine("Initial state: ");
                    Console.WriteLine(finiteAutomaton.GetInitialState());
                    Console.WriteLine();
                    break;

                case 6:
                    Console.WriteLine("Is deterministic?");
                    Console.WriteLine(finiteAutomaton.CheckIfDeterministic());
                    break;

                case 7:
                    {
                        Console.WriteLine("Your sequence: ");
                        string sequence = Console.ReadLine();

                        if (finiteAutomaton.AcceptsSequence(sequence))
                            Console.WriteLine("Sequence is valid");
                        else
                            Console.WriteLine("Invalid sequence");
                    }
                    break;

                default:
                    Console.WriteLine("Invalid command!");
                    break;
            }

            Console.WriteLine();
            PrintMenu();
            Console.WriteLine("Your option: ");
            option = int.Parse(Console.ReadLine());
        }
    }
}

    public static void Main(string[] args)
    {
        Console.WriteLine("1. Run scanner.");
        Console.WriteLine("2. Run DFA.");
        Console.WriteLine("Your option: ");

        int option = int.Parse(Console.ReadLine());

        switch (option)
        {
            case 1:
                RunScanner();
                break;

            case 2:
                DFAOptions.OptionsForDFA();
                break;

            default:
                Console.WriteLine("Invalid command!");
                break;
        }
    }
}


