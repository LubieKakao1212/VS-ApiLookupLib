using System;
using System.Collections.Generic;

namespace CommonApis.ApiLookup.Helper;

public class MultiCache<TKey, TReg>(Func<TKey, List<TReg>> valueProvider)
    where TKey : notnull {

    private readonly MultiDictionary<TKey, TReg> _cache = new();

    public IEnumerable<TReg> Get(TKey key) {
        var values = _cache.GetAllOrNull(key);
        if (values == null) {
            _cache.AddMulti(key, valueProvider(key));
            values = _cache.GetAllOrEmpty(key);
        }
        return values;
    }

    public IEnumerable<TReg> GetWithAny(IEnumerable<TKey> keys) {
        var set = new HashSet<TReg>();
        foreach (var key in keys) {
            set.UnionWith(Get(key));
        }
        return set;
    }
}