using System;

class Program
{
    static void Main()
    {
        Grammar g = new Grammar();
        g.ReadFromFile("D:\\UBB-Projects\\FLCD\\Lab5\\Parser\\G2.in");
        Console.WriteLine(g);
        g.MakeEnhancedGrammar();
        Parser parser = new Parser(g);
        parser.CreateCanonicalCollection();
        foreach (var state in parser.canonicalCollection)
        {
            Console.WriteLine(state.ToString());
        }
    }
}
