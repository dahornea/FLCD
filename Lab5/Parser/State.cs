using System.Collections.Generic;
using System.Linq;

public class State
{
    private static int count = 0;

    public int Id { get; }
    public List<Item> ClosureItems { get; }
    public List<Item> Closure { get; }

    public State(List<Item> closureItems, List<Item> closure)
    {
        Id = count++;
        ClosureItems = closureItems;
        Closure = closure;
    }

    public List<string> GetAllSymbolsAfterDot()
    {
        List<string> result = new List<string>();
        foreach (var item in Closure)
        {
            if (item.DotPosition < item.Rhs.Count)
            {
                result.Add(item.Rhs[item.DotPosition].ToString());
            }
        }
        return result;
    }


    public override bool Equals(object obj)
    {
        return obj is State other && ClosureItems.SequenceEqual(other.ClosureItems);
    }

    public override int GetHashCode()
    {
        return ClosureItems.GetHashCode(); // Consider a better hash code implementation if necessary
    }

    public override string ToString()
    {
        var result = $"s{Id} = closure({{{string.Join(", ", ClosureItems)}}}) = {{{string.Join(", ", Closure)}}}";
        return result;
    }
}
