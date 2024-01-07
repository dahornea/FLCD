using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab7
{
    class Program
    {
        static void Main()
        {
            Grammar g = new Grammar();
            g.ReadFromFile("D:\\UBB-Projects\\FLCD\\Lab7\\G1.in");
            Console.WriteLine(g);

            g.MakeEnhancedGrammar();

            Parser parser = new Parser(g);
            parser.CreateCanonicalCollection();

            foreach (var state in parser.canonicalCollection)
            {
                Console.WriteLine(state);
            }

            parser.CreateParsingTable();

            Console.WriteLine(parser.parsingTable);

            Console.WriteLine("Enter a sequence: ");
            string sequence = Console.ReadLine();
            List<string> sequenceList = sequence.Split(' ').ToList();

            List<object> outputBand = parser.ParseSequence(sequenceList);

            Console.WriteLine(string.Join(" ", outputBand));

            List<int> outputBand = List<>
            parserOutput.ComputeParsingTree();

            foreach (var item in parserOutput._parsingTree)
            {
                Console.WriteLine(item);
            }

            parserOutput.PrintToFile("out1.txt");
        }
    }
}