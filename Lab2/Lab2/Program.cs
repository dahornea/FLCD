using System;
using System.Collections.Generic;
namespace Lab2bun
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var symbolTable = new SymbolTable<int>(50);

            // Test case 1: AddSymbol and GetByKey
            symbolTable.AddSymbol("x", 42);
            int valueX = symbolTable.GetByKey("x");
            Console.WriteLine("Value of x: " + valueX);

            // Test case 2: AddSymbol and GetPair
            symbolTable.AddSymbol("y", 23);
            var pairY = symbolTable.GetPair("y");
            Console.WriteLine("Key: " + pairY.Key + ", Value: " + pairY.Value);

            symbolTable.AddSymbol(5, 15);
            int value = symbolTable.GetByKey(5);
            Console.WriteLine("Value :" + value);

            int size = symbolTable.getSize();
            Console.WriteLine("SymbolTable size: " + size);
        }

    }
}

