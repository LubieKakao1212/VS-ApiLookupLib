using System.Collections.Generic;
using System.Linq;

namespace CommonApis.ApiLookup.Helper;

public class MultiDictionary<TKey, TValue> where TKey : notnull {

    private readonly Dictionary<TKey, List<TValue>> _dictionary = new();
    
    public IEnumerable<TValue> GetAllOrEmpty(TKey key) {
        if (_dictionary.TryGetValue(key, out var value)) {
            return value;
        }
        return [];
    }
    
    public IEnumerable<TValue>? GetAllOrNull(TKey key) {
        return _dictionary.GetValueOrDefault(key);
    }
    
    public void Add(TKey key, TValue value) { 
        GetOrCreateBucket(key).Add(value);
    }

    public void AddToAll(IEnumerable<TKey> keys, TValue value) {
        foreach (var key in keys) {
            Add(key, value);
        }
    }

    public void AddMulti(TKey key, IEnumerable<TValue> values) {
        GetOrCreateBucket(key).AddRange(values);
    }

    public void AddMultiToAll(IEnumerable<TKey> keys, IEnumerable<TValue> values) {
        var enumerable = values as TValue[] ?? values.ToArray();
        foreach (var key in keys) {
            AddMulti(key, enumerable);
        }
    }
    
    private List<TValue> GetOrCreateBucket(TKey key) {
        if (!_dictionary.TryGetValue(key, out var values)) {
            values = new List<TValue>();
            _dictionary.Add(key, values);
        }
        return values;
    }
}