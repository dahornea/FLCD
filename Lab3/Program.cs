using System;
using System.IO;
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

    public static void Main(string[] args)
    {
        Run("C:\\Users\\dhornea\\Desktop\\compilatoare\\Lab2bun\\input_files\\p1.txt");
        Run("C:\\Users\\dhornea\\Desktop\\compilatoare\\Lab2bun\\input_files\\p2.txt");
        Run("C:\\Users\\dhornea\\Desktop\\compilatoare\\Lab2bun\\input_files\\p3.txt");
        Run("C:\\Users\\dhornea\\Desktop\\compilatoare\\Lab2bun\\input_files\\p1err.txt");
    }
}


