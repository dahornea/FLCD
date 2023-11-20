using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Lab2bun
{
    public class MyScanner
    {
        private readonly List<string> operators = new List<string>
    {
        "+", "-", "*", "/", "%", "le", "ge", "eq", "ne", "lt", "gt", "=", "&&"
    };

        private readonly List<string> separators = new List<string>
    {
        "{" , "}" , "(" , ")" , "[" , "]" , ":" , ";" , " " , "," , "\t" , "\n" , "'" , "\"", ".", "\\n"
    };

        private readonly List<string> reservedWords = new List<string>
    {
        "read", "write", "if", "endif", "else", "for", "while",
        "export", "start", "struc", "end", "start", "endwhile", "data"
    };

        private readonly string filePath;
        private SymbolTable symbolTable;
        private ProgramInternalForm pif;

        public MyScanner(string filePath)
        {
            this.filePath = filePath;
            this.symbolTable = new SymbolTable(50);
            this.pif = new ProgramInternalForm();
        }

        private string ReadFile()
        {
            StringBuilder fileContent = new StringBuilder();
            using (StreamReader reader = new StreamReader(this.filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    fileContent.Append(line).Append("\n");
                }
            }
            return fileContent.ToString().Replace("\t", "");
        }

        private List<Tuple<string, Tuple<int, int>>> CreateListOfProgramElements()
        {
            try
            {
                string content = ReadFile();
                string separatorsString = string.Join("|", separators.Select(Regex.Escape));
                string pattern = $"(?<={separatorsString})|(?={separatorsString})";
                string[] tokens = Regex.Split(content, pattern);
                List<string> nonEmptyTokens = new List<string>();
                foreach (string token in tokens)
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        nonEmptyTokens.Add(token);
                    }
                }

                return Tokenize(nonEmptyTokens);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        private List<Tuple<string, Tuple<int, int>>> Tokenize(List<string> tokensToBe)
        {
            var resultedTokens = new List<Tuple<string, Tuple<int, int>>>();
            bool isStringConstant = false;
            bool isCharConstant = false;
            var createdString = new StringBuilder();
            int lineNumber = 1;
            int columnNumber = 1;

            foreach (string token in tokensToBe)
            {
                switch (token)
                {
                    case "\"":
                        if (isStringConstant)
                        {
                            createdString.Append(token);
                            resultedTokens.Add(new Tuple<string, Tuple<int, int>>(createdString.ToString(), new Tuple<int, int>(lineNumber, columnNumber)));
                            createdString = new StringBuilder();
                        }
                        else
                        {
                            createdString.Append(token);
                        }
                        isStringConstant = !isStringConstant;
                        break;
                    case "'":
                        if (isCharConstant)
                        {
                            createdString.Append(token);
                            resultedTokens.Add(new Tuple<string, Tuple<int, int>>(createdString.ToString(), new Tuple<int, int>(lineNumber, columnNumber)));
                            createdString = new StringBuilder();
                        }
                        else
                        {
                            createdString.Append(token);
                        }
                        isCharConstant = !isCharConstant;
                        break;
                    case "\n":
                        lineNumber++;
                        columnNumber = 1;
                        break;
                    default:
                        if (isStringConstant)
                        {
                            createdString.Append(token);
                        }
                        else if (isCharConstant)
                        {
                            createdString.Append(token);
                        }
                        else if (token != " ")
                        {
                            resultedTokens.Add(new Tuple<string, Tuple<int, int>>(token, new Tuple<int, int>(lineNumber, columnNumber)));
                            columnNumber++;
                        }
                        break;
                }
            }

            return resultedTokens;
        }

        public void Scan()
        {
            List<Tuple<string, Tuple<int, int>>> tokens = CreateListOfProgramElements();
            bool lexicalErrorExists = false;

            if (tokens == null)
            {
                return;
            }

            tokens.ForEach(t =>
            {
                string token = t.Item1;
                if (this.reservedWords.Contains(token.ToLower()))
                {
                    this.pif.Add(new Tuple<string, Tuple<int, int>>(token, new Tuple<int, int>(-1, -1)), 2);
                }
                else if (this.operators.Contains(token.ToLower()))
                {
                    this.pif.Add(new Tuple<string, Tuple<int, int>>(token, new Tuple<int, int>(-1, -1)), 3);
                }
                else if (this.separators.Contains(token.ToLower()))
                {
                    this.pif.Add(new Tuple<string, Tuple<int, int>>(token, new Tuple<int, int>(-1, -1)), 4);
                }
                else if (Regex.IsMatch(token, "^'[1-9]'|'[a-zA-Z]'|\"[0-9]*[a-zA-Z ]*\"$") || new FiniteAutomaton("D:\\UBB-Projects\\FLCD\\Lab4\\input_files\\FA_integer_constants.txt").AcceptsSequence(token))
                {
                    this.symbolTable.Add(token);
                    this.pif.Add(new Tuple<string, Tuple<int, int>>("CONST", symbolTable.FindPositionOfTerm(token)), 0);

                }
                else if (new FiniteAutomaton("D:\\UBB-Projects\\FLCD\\Lab4\\input_files\\FA_identifiers.txt").AcceptsSequence(token))
                {
                    this.symbolTable.Add(token);
                    this.pif.Add(new Tuple<string, Tuple<int, int>>("IDENTIFIER", symbolTable.FindPositionOfTerm(token)), 1);
                }
                else
                {
                    Tuple<int, int> pairLineColumn = t.Item2;
                    Console.WriteLine("Error at line: " + pairLineColumn.Item1 + " and column: " + pairLineColumn.Item2 + ", invalid token: " + t.Item1);
                    lexicalErrorExists = true;
                }
            });

            if (!lexicalErrorExists)
            {
                Console.WriteLine("Program is lexically correct!");
            }
        }



        public ProgramInternalForm GetPif()
        {
            return pif;
        }

        public SymbolTable GetSymbolTable()
        {
            return symbolTable;
        }
    }
}