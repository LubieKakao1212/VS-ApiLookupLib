using System;
using System.Diagnostics.CodeAnalysis;
using CommonApis.ApiLookup.Helper;
using Vintagestory.API.Common;

namespace CommonApis.ApiLookup.API;

public interface IItemStackApiLookup<TValue, TContext> : IApiLookupBase<TValue, ItemLookupContext<TContext>, ItemStack> {
    
    void RegisterForCollectibles(Getter getter, params CollectibleObject[] collectibles);
    void RegisterForCollectibles(Getter getter, AssetLocation wildcard);

    void RegisterForBehaviors(Getter getter, bool inherited, params Type[] behaviorTypes);
    void RegisterForTypes(Getter getter, bool inherited, params Type[] collectibleTypes);

}

public struct ItemLookupContext<TContext> {
    
    [Experimental("IStorage")]
    public required IItemStorageContext Storage { get; init; }

    public required TContext Context { get; init; }
    
}

[Experimental("IStorage")]
public static class ItemLookupContext {
    
    public static ItemLookupContext<NoContext> NoContext(IItemStorageContext storage) {
        return new ItemLookupContext<NoContext> {
            Context = default,
            Storage = storage
        };
    }
    
}