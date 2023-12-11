using System;
using System.Collections.Generic;
using System.Linq;

public class Item
{
    public string Lhs { get; private set; }
    public List<char> Rhs { get; private set; }
    public int DotPosition { get; set; }

    public Item(string lhs, List<char> rhs, int dotPosition)
    {
        Lhs = lhs;
        Rhs = rhs;
        DotPosition = dotPosition;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Item other = (Item)obj;
        return Rhs.SequenceEqual(other.Rhs) &&
               Lhs == other.Lhs &&
               DotPosition == other.DotPosition;
    }

    public override string ToString()
    {
        string result = "[" + Lhs + " -> ";
        for (int i = 0; i < Rhs.Count; i++)
        {
            if (i == DotPosition)
            {
                result += ". ";
            }
            result += Rhs[i] + " ";
        }
        if (DotPosition == Rhs.Count)
        {
            result += ".";
        }
        return result.Trim() + "]";
    }
}
