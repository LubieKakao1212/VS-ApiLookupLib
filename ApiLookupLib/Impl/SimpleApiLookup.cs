using System.Collections.Generic;
using ApiLookupLib.API;
using Vintagestory.API.Common;

namespace ApiLookupLib.Impl;

public class SimpleApiLookup<TValue, TContext, TSourceArg> : IApiLookupBase<TValue, TContext, TSourceArg> {

    private readonly List<IApiLookupBase<TValue, TContext, TSourceArg>.Getter> _fallbacks = new();

    public virtual TValue? Get(IWorldAccessor world, TSourceArg pos, TContext context) {
        return GetFallback(world, pos, context);
    }
    
    public void RegisterFallback(IApiLookupBase<TValue, TContext, TSourceArg>.Getter getter) {
        _fallbacks.Add(getter);
    }

    private TValue? GetFallback(IWorldAccessor world, TSourceArg pos, TContext context) {
        foreach (var getter in _fallbacks) {
            var value = getter(world, pos, context);
            if (value != null) {
                return value;
            }
        }
        return default;
    }
}