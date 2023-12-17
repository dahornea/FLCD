using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace Lab7
{
    public class ParserOutputEntry
    {
        private int _index;
        private int _symbol;
        private int _father;
        private int _sibling;

        public int Index
        {
            get => _index;
            set => _index = value;
        }

        public int Symbol
        {
            get => _symbol;
            set => _symbol = value;
        }

        public int Father
        {
            get => _father;
            set => _father = value;
        }

        public int Sibling
        {
            get => _sibling;
            set => _sibling = value;
        }

        public ParserOutputEntry(int index, int symbol, int father, int sibling)
        {
            Index = index;
            Symbol = symbol;
            Father = father;
            Sibling = sibling;
        }

        public override string ToString()
        {
            return $"({_index}, {_symbol}, {_father}, {_sibling})";
        }
    }

    public class ParserOutput
    {
        private readonly List<ParserOutputEntry> _parsingTree = new List<ParserOutputEntry>();
        private readonly List<int> _outputBand;
        private readonly Grammar _grammar;

        public ParserOutput(List<int> outputBand, Grammar grammar)
        {
            _outputBand = outputBand;
            _grammar = grammar;
        }

        private bool CheckHasChildren(int node)
        {
            foreach (var item in _parsingTree)
            {
                if (item.Father == node)
                {
                    return true;
                }
            }
            return false;
        }

        public void ComputeParsingTree()
        {
            int current_index = 0;
            _parsingTree.Add(new ParserOutputEntry(current_index, _grammar.STARTING_SYMBOL, -1, -1));

            foreach (var production_id in _outputBand)
            {
                var production = _grammar.GetProductionById(production_id);

                foreach (var parsingTreeItem in _parsingTree)
                {
                    if (parsingTreeItem.Symbol.ToString() == production.Item1 && !CheckHasChildren(parsingTreeItem.Index))
                    {
                        int father = parsingTreeItem.Index;
                        current_index++;

                        _parsingTree.Add(new ParserOutputEntry(current_index, production.Item2[0], father, -1));

                        for (int index = 1; index < production.Item2.Length; index++)
                        {
                            current_index++;
                            _parsingTree[current_index - 1].Sibling = current_index;
                            _parsingTree.Add(new ParserOutputEntry(current_index, production.Item2[index], father, -1));
                        }

                        break;
                    }
                }
            }
        }

        public void PrintToFile(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                foreach (var item in _parsingTree)
                {
                    writer.WriteLine(item);
                }
            }
        }
    }
}

