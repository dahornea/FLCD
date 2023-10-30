using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2bun
{
    using System;
    using System.Collections.Generic;

    public class HashTable
    {
        private int size;
        private List<List<string>> table;

        public HashTable(int size)
        {
            this.size = size;
            this.table = new List<List<string>>(size);
            for (int i = 0; i < size; i++)
            {
                this.table.Add(new List<string>());
            }
        }

        public string FindByPos(Tuple<int, int> pos)
        {
            if (this.table.Count <= pos.Item1|| this.table[pos.Item1].Count <= pos.Item2)
            {
                throw new IndexOutOfRangeException("Invalid position");
            }

            return this.table[pos.Item1][pos.Item2];
        }

        public int GetSize()
        {
            return size;
        }

        public Tuple<int, int> FindPositionOfTerm(string elem)
        {
            int pos = Hash(elem);

            if (table[pos].Count > 0)
            {
                List<string> elems = table[pos];
                for (int i = 0; i < elems.Count; i++)
                {
                    if (elems[i] == elem)
                    {
                        return new Tuple<int, int>(pos, i);
                    }
                }
            }

            return null;
        }

        private int Hash(string key)
        {
            int sumChars = 0;
            char[] keyCharacters = key.ToCharArray();
            foreach (char c in keyCharacters)
            {
                sumChars += c;
            }
            return sumChars % size;
        }

        public bool ContainsTerm(string elem)
        {
            return FindPositionOfTerm(elem) != null;
        }

        public bool Add(string elem)
        {
            if (ContainsTerm(elem))
            {
                return false;
            }

            int pos = Hash(elem);

            List<string> elems = table[pos];
            elems.Add(elem);

            return true;
        }

        public override string ToString()
        {
            System.Text.StringBuilder computedString = new System.Text.StringBuilder();
            for (int i = 0; i < table.Count; i++)
            {
                if (table[i].Count > 0)
                {
                    computedString.Append(i);
                    computedString.Append(" - ");
                    computedString.Append(string.Join(", ", table[i]));
                    computedString.Append("\n");
                }
            }
            return computedString.ToString();
        }
    }

}

