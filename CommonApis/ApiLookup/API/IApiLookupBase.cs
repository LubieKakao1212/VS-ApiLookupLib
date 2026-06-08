using Vintagestory.API.Common;

namespace CommonApis.ApiLookup.API;

public interface IApiLookupBase<TValue, TContext, TSourceArg> {
    delegate TValue? Getter(IWorldAccessor world, TSourceArg source, TContext context);
    
    TValue? Get(IWorldAccessor world, TSourceArg source, TContext context);
    
    void RegisterFallback(Getter getter);
}