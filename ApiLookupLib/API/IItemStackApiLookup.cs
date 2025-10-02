using Vintagestory.API.Common;

namespace ApiLookupLib.API;

public interface IItemStackApiLookup<TValue, TContext> : IApiLookupBase<TValue, TContext, ItemStack> {

    void RegisterFor(Getter getter, params CollectibleObject[] collectibles);
    
    void RegisterFor(Getter getter, AssetLocation wildcard);
    
}