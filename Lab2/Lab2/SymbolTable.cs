using System;
using Lab2bun;

using System;
using System.Collections.Generic;

public class SymbolTable<TValue>
{
    private HashTable<object, TValue> hashTable;

    public SymbolTable(int size)
    {
        hashTable = new HashTable<object, TValue>(size);
    }

    public void AddSymbol(object symbol, TValue value)
    {
        hashTable.Add(symbol, value);
    }

    public TValue GetByKey(object key)
    {
        return hashTable.Get(key);
    }

    public KeyValuePair<object, TValue> GetPair (object key)
    {
        return (KeyValuePair<object, TValue>)hashTable.FindByPair(key);
    }

    public int getSize()
    {
        return hashTable.getSize();
    }

}


