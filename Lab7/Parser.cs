using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab7
{
    public class Connection
    {
        public State StartingState { get; }
        public State FinalState { get; }
        public string Symbol { get; }

        public Connection(State startingState, State finalState, string symbol)
        {
            StartingState = startingState;
            FinalState = finalState;
            Symbol = symbol;
        }

        public override string ToString()
        {
            return "Starting state: " + StartingState + " " +
                   "Final state: " + FinalState + " " +
                   "Symbol: " + Symbol;
        }
    }

    public class Parser
    {
        private readonly Grammar grammar;
        public List<State> canonicalCollection;
        private readonly List<Connection> connections;
        public readonly Dictionary<int, (ACTION, object)> parsingTable;

        public Parser(Grammar grammar)
        {
            this.grammar = grammar;
            canonicalCollection = new List<State>();
            connections = new List<Connection>();
            parsingTable = new Dictionary<int, (ACTION, object)>();
        }

        private static bool IsItemInClosure(Item item, List<Item> closure)
        {
            return closure.Any(itemInClosure =>
                item.Lhs == itemInClosure.Lhs &&
                item.Rhs.SequenceEqual(itemInClosure.Rhs) &&
                item.DotPosition == itemInClosure.DotPosition);
        }

        private State Closure(List<Item> items)
        {
            var currentClosure = new List<Item>(items);

            var finished = false;
            while (!finished)
            {
                var oldClosure = new List<Item>(currentClosure);
                foreach (var closureItem in currentClosure.ToList())
                {
                    if (closureItem.DotPosition < closureItem.Rhs.Count &&
                        grammar.N.Contains(closureItem.Rhs[closureItem.DotPosition].ToString()))
                    {
                        foreach (var production in grammar.P[closureItem.Rhs[closureItem.DotPosition].ToString()])
                        {
                            string productionValue = production.Count > 0 ? production[0] : string.Empty;
                            List<char> characters = !string.IsNullOrEmpty(productionValue) ? productionValue.ToList() : new List<char>();
                            if (!IsItemInClosure(new Item(
                                    closureItem.Rhs[closureItem.DotPosition].ToString(),
                                    characters,
                                    0),
                                currentClosure))
                            {
                                currentClosure.Add(new Item(
                                    closureItem.Rhs[closureItem.DotPosition].ToString(),
                                    characters,
                                    0));
                            }
                        }
                    }
                }

                if (currentClosure.SequenceEqual(oldClosure))
                {
                    finished = true;
                }
            }

            return new State(items, currentClosure, grammar.S);
        }

        private State Goto(State state, string symbol)
        {
            var itemsForSymbol = state.Closure
                .Where(item =>
                    item.DotPosition < item.Rhs.Count &&
                    item.Rhs[item.DotPosition].ToString() == symbol)
                .Select(item => new Item(item.Lhs, item.Rhs, item.DotPosition + 1))
                .ToList();

            var existingState = canonicalCollection.FirstOrDefault(s => s.ClosureItems.SequenceEqual(itemsForSymbol));
            return existingState ?? Closure(itemsForSymbol);
        }

        public void CreateCanonicalCollection()
        {
            // Assuming grammar.P[grammar.S][0] is a List<string>
            List<string> production = grammar.P[grammar.S][0];

            // Extract the first string from the list
            string productionValue = production.Count > 0 ? production[0] : string.Empty;

            List<char> characters = !string.IsNullOrEmpty(productionValue) ? productionValue.ToList() : new List<char>();
            // Create the Item using the extracted value
            Item newItem = new Item(grammar.S, characters, 0);

            canonicalCollection = new List<State>
            {
                Closure(new List<Item>
                {
                    newItem
                })
            };

            var index = 0;
            while (index < canonicalCollection.Count)
            {
                var state = canonicalCollection[index];
                var symbols = state.GetAllSymbolsAfterDot();
                foreach (var symbol in symbols)
                {
                    var newState = Goto(state, symbol);
                    if (!canonicalCollection.Contains(newState))
                    {
                        canonicalCollection.Add(newState);
                    }

                    connections.Add(new Connection(state, newState, symbol));
                }

                index++;
            }
        }

        public void CreateParsingTable()
        {
            foreach (var state in canonicalCollection)
            {
                var stateConnections = GetStateInConnections(state);
                if (stateConnections.Count == 0)
                {
                    if (state.Action == ACTION.ACCEPT)
                    {
                        parsingTable[state.Id] = (ACTION.ACCEPT, null);
                    }
                    else if (state.Action == ACTION.REDUCE)
                    {
                        var prodId = GetProductionNumberFromGrammar(state);
                        if (prodId == null)
                        {
                            throw new Exception("Something went wrong!");
                        }

                        parsingTable[state.Id] = (ACTION.REDUCE, prodId);
                    }
                }
                else if (state.Action == ACTION.SHIFT || state.Action == ACTION.SHIFT_REDUCE_CONFLICT)
                {
                    if (!parsingTable.ContainsKey(state.Id))
                    {
                        parsingTable[state.Id] = (state.Action, new Dictionary<string, int>());
                    }

                    foreach (var conn in stateConnections)
                    {
                        ((Dictionary<string, int>)parsingTable[state.Id].Item2)[conn.Symbol] = conn.FinalState.Id;
                    }
                }
                else
                {
                    if (state.Action == ACTION.REDUCE_REDUCE_CONFLICT)
                    {
                        throw new Exception("Reduce reduce conflict!");
                    }
                }
            }
        }

        private string GetProductionNumberFromGrammar(State state)
        {
            foreach (var prod in grammar.P.Keys)
            {
                foreach (var prodValue in grammar.P[prod])
                {
                    if (state.Closure[0].Lhs == prod && state.Closure[0].Rhs.SequenceEqual(prodValue[0]))
                    {
                        return prodValue[1];
                    }
                }
            }

            return null;
        }
        
        private List<Connection> GetStateInConnections(State state)
        {
            return connections.Where(conn => conn.StartingState == state).ToList();
        }

        public State GetStateById(int stateId)
        {
            foreach (var state in canonicalCollection)
            {
                if (state.Id == stateId)
                {
                    return state;
                }
            }
            return null;
        }

        private Item GetItemWithDotAtEnd(State state)
        {
            return state.Closure.FirstOrDefault(item => item.DotPosition == item.Rhs.Count);
        }

        private string GetProductionNumberShiftReduceConflict(int stateId)
        {
            var state = GetStateById(stateId);
            if (state == null)
            {
                return null;
            }

            var item = GetItemWithDotAtEnd(state);
            if (item == null)
            {
                return null;
            }

            foreach (var prod in grammar.P.Keys)
            {
                foreach (var prodValue in grammar.P[prod])
                {
                    if (item.Lhs == prod && item.Rhs.SequenceEqual(prodValue[0]))
                    {
                        return prodValue[0];
                    }
                }
            }

            return null;
        }

        public List<object> ParseSequence(List<string> words)
        {
            const string END_SIGN = "$";
            var outputBand = new List<object>();

            var workStack = new Stack<object>();
            workStack.Push(END_SIGN);
            workStack.Push(0);

            var inputStack = new Stack<string>();
            inputStack.Push(END_SIGN);
            foreach (var word in words)
            {
                inputStack.Push(word);
            }

            var idx = 0;
            while ((int)workStack.Peek() != 0 || (string)inputStack.Peek() != END_SIGN)
            {
                if (parsingTable[(int)workStack.Peek()].Item1 == ACTION.ACCEPT)
                {
                    while ((int)workStack.Peek() != 0)
                    {
                        workStack.Pop();
                    }
                }
                else if (parsingTable[(int)workStack.Peek()].Item1 == ACTION.SHIFT)
                {
                    idx++;
                    var topState = (int)workStack.Peek();
                    var symbol = inputStack.Pop();
                    workStack.Push(symbol);

                    if (!((Dictionary<string, int>)parsingTable[topState].Item2).ContainsKey(symbol))
                    {
                        throw new Exception($"Index {idx} -> Invalid symbol: {symbol} for goto of state {topState}");
                    }

                    var newTopState = ((Dictionary<string, int>)parsingTable[topState].Item2)[symbol];
                    workStack.Push(newTopState);
                }
                else if (parsingTable[(int)workStack.Peek()].Item1 == ACTION.SHIFT_REDUCE_CONFLICT)
                {
                    var possibleSymbol = inputStack.Peek();
                    if ((inputStack.Count == 1 && possibleSymbol == END_SIGN) ||
                        !((Dictionary<string, int>)parsingTable[(int)workStack.Peek()].Item2).ContainsKey(possibleSymbol))
                    {
                        var prodId = GetProductionNumberShiftReduceConflict((int)workStack.Peek());
                        var prodIdInt = int.Parse(prodId);
                        var prod = grammar.GetProductionById(prodIdInt);
                        outputBand.Add(prodId);
                        var index = 0;
                        while (index < prod.Item1.Length)
                        {
                            workStack.Pop();
                            workStack.Pop();
                            index++;
                        }

                        var topState = (int)workStack.Peek();
                        workStack.Push(prod.Item1);
                        var newTopState = ((Dictionary<string, int>)parsingTable[topState].Item2)[prod.Item1];
                        workStack.Push(newTopState);
                    }
                    else
                    {
                        idx++;
                        var topState = (int)workStack.Peek();
                        var symbol = inputStack.Pop();
                        workStack.Push(symbol);

                        if (!((Dictionary<string, int>)parsingTable[topState].Item2).ContainsKey(symbol))
                        {
                            throw new Exception($"Index {idx} -> Invalid symbol: {symbol} for goto of state {topState}");
                        }

                        var newTopState = ((Dictionary<string, int>)parsingTable[topState].Item2)[symbol];
                        workStack.Push(newTopState);
                    }
                }
                else if (parsingTable[(int)workStack.Peek()].Item1 == ACTION.REDUCE)
                {
                    var prod = grammar.GetProductionById((int)parsingTable[(int)workStack.Peek()].Item2);
                    outputBand.Add((int)parsingTable[(int)workStack.Peek()].Item2);
                    var index = 0;
                    while (index < prod.Item2.Length)
                    {
                        workStack.Pop();
                        workStack.Pop();
                        index++;
                    }

                    var topState = (int)workStack.Peek();
                    workStack.Push(prod.Item1);
                    var newTopState = ((Dictionary<string, int>)parsingTable[topState].Item2)[prod.Item1];
                    workStack.Push(newTopState);
                }
            }

            outputBand.Reverse();
            return outputBand;
        }
    }
}
