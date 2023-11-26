using System;

class Program
{
    static void Main(string[] args)
    {
        Grammar g = new Grammar("D:\\UBB-Projects\\FLCD\\Lab5\\Parser\\G2.in");
        Console.WriteLine(g.ToString());

        if (g.IsCFG())
        {
            Console.WriteLine("The grammar is a CFG");
        }
        else
        {
            Console.WriteLine("The grammar is not a CFG");
        }
    }
}
