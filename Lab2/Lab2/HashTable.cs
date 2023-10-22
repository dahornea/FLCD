using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2bun
{
    using System;
    using System.Collections.Generic;

    public class HashTable<TKey, TValue>
    {
        private const int Capacity = 30;  // Adjust the capacity as needed
        private LinkedList<KeyValuePair<TKey, TValue>>[] table;

        public HashTable(int capacity = Capacity)
        {
            table = new LinkedList<KeyValuePair<TKey, TValue>>[capacity];
        }

        // Hash function to calculate the index for the given key
        private int Hash(object key)
        {
            int hash = key switch
            {
                int intKey => intKey.GetHashCode() % Capacity,
                string stringKey => stringKey.GetHashCode() % Capacity,
                _ => key.GetHashCode() % Capacity
            };

            return hash < 0 ? hash + Capacity : hash;
        }

        // Add a key-value pair to the hash table
        public void Add(TKey key, TValue value)
        {
            int index = Hash(key);
            if (table[index] == null)
            {
                table[index] = new LinkedList<KeyValuePair<TKey, TValue>>();
            }

            table[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
        }

        // Get the value associated with a key
        public TValue Get(TKey key)
        {
            int index = Hash(key);
            var list = table[index];

            if (list != null)
            {
                foreach (var pair in list)
                {
                    if (pair.Key.Equals(key))
                    {
                        return pair.Value;
                    }
                }
            }

            throw new KeyNotFoundException("Key not found in the hash table");
        }

        // Find a key-value pair in the hash table and return it
        public KeyValuePair<TKey, TValue>? FindByPair(TKey key)
        {
            int index = Hash(key);
            var list = table[index];

            if (list != null)
            {
                foreach (var pair in list)
                {
                    if (pair.Key.Equals(key))
                    {
                        return pair;
                    }
                }
            }

            return null;
        }

        public int getSize()
        {
            return Capacity;
        }
    }

}

