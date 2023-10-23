using System;

using System;
using System.Collections.Generic;

namespace Lab2bun
{
    public class SymbolTable
    {
        private int size;
        private HashTable hashTable;

        public SymbolTable(int size)
        {
            this.size = size;
            this.hashTable = new HashTable(size);
        }

        public string FindByPos(Pair<int, int> pos)
        {
            return hashTable.FindByPos(pos);
        }

        public HashTable GetHashTable()
        {
            return hashTable;
        }

        public int GetSize()
        {
            return hashTable.GetSize();
        }

        public Pair<int, int> FindPositionOfTerm(string term)
        {
            return hashTable.FindPositionOfTerm(term);
        }

        public bool ContainsTerm(string term)
        {
            return hashTable.ContainsTerm(term);
        }

        public bool Add(string term)
        {
            return hashTable.Add(term);
        }

        public override string ToString()
        {
            return hashTable.ToString();
        }
    }
}

