using System;
using System.Diagnostics.CodeAnalysis;
using Vintagestory.API.Common;

namespace ApiLookupLib.API;

[Experimental("ItemLookup_Experimental")]
public interface IItemStackApiLookup<TValue, TContext> : IApiLookupBase<TValue, TContext, IItemAccess> {
    
    void RegisterForCollectibles(Getter getter, params CollectibleObject[] collectibles);
    void RegisterForCollectibles(Getter getter, AssetLocation wildcard);

    void RegisterForBehaviors(Getter getter, bool inherited, params Type[] behaviorTypes);
    void RegisterForTypes(Getter getter, bool inherited, params Type[] collectibleTypes);

}