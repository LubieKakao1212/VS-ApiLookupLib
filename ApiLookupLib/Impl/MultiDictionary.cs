using System.Collections.Generic;
using System.Linq;

namespace ApiLookupLib.Impl;

public class MultiDictionary<TKey, TValue> where TKey : notnull {

    private readonly Dictionary<TKey, List<TValue>> _dictionary = new();
    
    public IEnumerable<TValue> GetAllOrEmpty(TKey key) {
        if (_dictionary.TryGetValue(key, out var value)) {
            return value;
        }
        return [];
    }
    
    public void Add(TKey key, TValue value) {
        if (!_dictionary.TryGetValue(key, out var values)) {
            values = new List<TValue>();
            _dictionary.Add(key, values);
        }
        values.Add(value);
    }

    public void AddToAll(IEnumerable<TKey> keys, TValue value) {
        foreach (var key in keys) {
            Add(key, value);
        }
    }
}