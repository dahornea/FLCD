using System.Collections.Generic;

public class Parser
{
    private Grammar grammar;
    private List<State> canonicalCollection;

    public Parser(Grammar grammar)
    {
        this.grammar = grammar;
        this.canonicalCollection = new List<State>();
    }

    public static bool IsItemInClosure(Item item, List<Item> closure)
    {
        foreach (var itemInClosure in closure)
        {
            if (item.Lhs == itemInClosure.Lhs &&
                item.Rhs == itemInClosure.Rhs &&
                item.DotPosition == itemInClosure.DotPosition)
            {
                return true;
            }
        }
        return false;
    }

    public State Closure(List<Item> items)
    {
        var currentClosure = new List<Item>(items);

        bool finished = false;
        while (!finished)
        {
            var oldClosure = new List<Item>(currentClosure);
            foreach (var closureItem in currentClosure)
            {
                if (closureItem.DotPosition < closureItem.Rhs.Count &&
                    grammar.N.Contains(closureItem.Rhs[closureItem.DotPosition]))
                {
                    foreach (var production in grammar.P[closureItem.Rhs[closureItem.DotPosition]])
                    {
                        var newItem = new Item(
                            closureItem.Rhs[closureItem.DotPosition],
                            production,
                            0
                        );
                        if (!IsItemInClosure(newItem, currentClosure))
                        {
                            currentClosure.Add(newItem);
                        }
                    }
                }
            }
            if (ListsAreEqual(currentClosure, oldClosure))
            {
                finished = true;
            }
        }

        return new State(items, currentClosure);
    }

    public State Goto(State state, string symbol)
    {
        var itemsForSymbol = new List<Item>();
        foreach (var item in state.Closure)
        {
            if (item.DotPosition < item.Rhs.Count &&
                item.Rhs[item.DotPosition] == symbol)
            {
                itemsForSymbol.Add(new Item(item.Lhs, item.Rhs, item.DotPosition + 1));
            }
        }

        foreach (var theState in canonicalCollection)
        {
            if (ListsAreEqual(theState.ClosureItems, itemsForSymbol))
            {
                return theState;
            }
        }

        return Closure(itemsForSymbol);
    }

    public void CreateCanonicalCollection()
    {
        canonicalCollection = new List<State>
        {
            Closure(new List<Item>
            {
                new Item(grammar.S, grammar.P[grammar.S][0], 0)
            })
        };

        int index = 0;
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
            }
            index++;
        }
    }

    private bool ListsAreEqual<T>(List<T> list1, List<T> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }

        for (int i = 0; i < list1.Count; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(list1[i], list2[i]))
            {
                return false;
            }
        }

        return true;
    }
}
