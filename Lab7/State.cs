using System;
using System.Collections.Generic;
using System.Linq;

namespace Parser
{

    public enum ACTION
    {
        SHIFT = 1,
        ACCEPT = 2,
        REDUCE = 3,
        REDUCE_REDUCE_CONFLICT = 4,
        SHIFT_REDUCE_CONFLICT = 5
    }

    public class State
    {
        private static readonly IEnumerator<int> idCounter = Enumerable.Range(0, int.MaxValue).GetEnumerator();
        
        public int Id { get; }
        public ACTION Action { get; private set; }
        public List<Item> ClosureItems { get; }
        public List<Item> Closure { get; }

        public State(List<Item> closureItems, List<Item> closure, string enrichedSymbol)
        {
            Id = idCounter.MoveNext() ? idCounter.Current : throw new InvalidOperationException("ID counter exhausted");
            ClosureItems = closureItems;
            Closure = closure;
            SetAction(enrichedSymbol);
        }

        private void SetAction(string enrichedSymbol)
        {
            if (Closure.Count == 1 &&
                Closure[0].Rhs.Count == Closure[0].DotPosition &&
                Closure[0].Lhs == enrichedSymbol)
            {
                Action = ACTION.ACCEPT;
            }
            else if (Closure.Count == 1 && Closure[0].DotPosition == Closure[0].Rhs.Count)
            {
                Action = ACTION.REDUCE;
            }
            else if (Closure.Count != 0 && CheckAllNotDotEnd())
            {
                Action = ACTION.SHIFT;
            }
            else
            {
                if (Closure.Count > 1 && CheckAllDotEnd())
                {
                    Action = ACTION.REDUCE_REDUCE_CONFLICT;
                }
                else
                {
                    Action = ACTION.SHIFT_REDUCE_CONFLICT;
                }
            }
        }

        private bool CheckAllNotDotEnd()
        {
            return Closure.All(c => c.Rhs.Count > c.DotPosition);
        }

        private bool CheckAllDotEnd()
        {
            return Closure.All(c => c.Rhs.Count <= c.DotPosition);
        }

        public List<string> GetAllSymbolsAfterDot()
        {
            var result = new List<string>();
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
            if (obj is State other)
            {
                return ClosureItems.SequenceEqual(other.ClosureItems);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ClosureItems.GetHashCode();
        }

        public override string ToString()
        {
            var result = $"s{Id} = closure({{";
            result += string.Join(", ", ClosureItems.Select(item => item.ToString()));
            result = result.TrimEnd(',', ' ') + "}}) = {";
            result += string.Join(", ", Closure.Select(item => item.ToString()));
            result = result.TrimEnd(',', ' ') + "}";
            return result;
        }
    }
}