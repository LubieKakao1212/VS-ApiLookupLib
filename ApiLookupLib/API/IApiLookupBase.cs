using Vintagestory.API.Common;

namespace ApiLookupLib.API;

public interface IApiLookupBase<TValue, TContext, TSourceArg> {
    delegate TValue? Getter(IWorldAccessor world, TSourceArg source, TContext context);
    
    TValue? Get(IWorldAccessor world, TSourceArg source, TContext context);
    
    void RegisterFallback(Getter getter);
}