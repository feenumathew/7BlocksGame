using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    
}
public class TwoKeyDictionary<TKey1, TKey2, TValue>
{
    private Dictionary<(TKey1, TKey2), TValue> dictionary = new Dictionary<(TKey1, TKey2), TValue>();

    public TValue this[TKey1 key1, TKey2 key2]
    {
        get { return dictionary[(key1, key2)]; }
        set { dictionary[(key1, key2)] = value; }
    }

    public void Add(TKey1 key1, TKey2 key2, TValue value)
    {
        dictionary.Add((key1, key2), value);
    }

    public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue value)
    {
        return dictionary.TryGetValue((key1, key2), out value);
    }

    // Returns all values as a List<TValue>
    public List<TValue> GetValues()
    {
        return new List<TValue>(dictionary.Values);
    }

    // Returns all keys as a List of tuples (TKey1, TKey2)
    public List<(TKey1, TKey2)> GetKeys()
    {
        return new List<(TKey1, TKey2)>(dictionary.Keys);
    }
}

