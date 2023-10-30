using System;
using System.Collections.Generic;

namespace Lab2bun
{
    public class ProgramInternalForm
    {
        private List<Tuple<string, Tuple<int, int>>> tokenPositionPair;
        private List<int> types;

        public ProgramInternalForm()
        {
            this.tokenPositionPair = new List<Tuple<string, Tuple<int, int>>>();
            this.types = new List<int>();
        }

        public void Add(Tuple<string, Tuple<int, int>> pair, int type)
        {
            this.tokenPositionPair.Add(pair);
            this.types.Add(type);
        }

        public override string ToString()
        {
            var computedString = new System.Text.StringBuilder();
            for (int i = 0; i < this.tokenPositionPair.Count; i++)
            {
                computedString.Append(this.tokenPositionPair[i].Item1)
                              .Append(" - ")
                              .Append(this.tokenPositionPair[i].Item2)
                              .Append(" -> ")
                              .Append(types[i])
                              .Append("\n");
            }

            return computedString.ToString();
        }
    }
}
