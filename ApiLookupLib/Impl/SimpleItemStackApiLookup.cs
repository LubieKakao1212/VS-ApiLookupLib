using System.Linq;
using ApiLookupLib.API;
using Vintagestory.API.Common;

namespace ApiLookupLib.Impl;

public class SimpleItemStackApiLookup<TValue, TContext>(IWorldAccessor worldIds) : SimpleApiLookup<TValue, TContext, ItemStack>, IItemStackApiLookup<TValue, TContext> {
    
    private readonly MultiDictionary<CollectibleObject, IApiLookupBase<TValue, TContext, ItemStack>.Getter> _itemsLookup = new();
    
    public override TValue? Get(IWorldAccessor world, ItemStack stack, TContext context) {
        var collectible = stack.Collectible;

        foreach (var getter in _itemsLookup.GetAllOrEmpty(collectible)) {
            var value = getter(world, stack, context);
            if (value != null) {
                return value;
            }
        }

        return Get(world, stack, context);
    }
    
    public void RegisterFor(IApiLookupBase<TValue, TContext, ItemStack>.Getter getter, params CollectibleObject[] collectibles) {
        _itemsLookup.AddToAll(collectibles, getter);
    }
    
    public void RegisterFor(IApiLookupBase<TValue, TContext, ItemStack>.Getter getter, AssetLocation wildcard) {
        var blocks = worldIds.SearchBlocks(wildcard);
        var items = worldIds.SearchItems(wildcard);
        
        if (blocks.Length == 0 && items.Length == 0) {
            worldIds.Logger.Warning($"No blocks or items found for wildcard: {wildcard}");
            return;
        }
        
        RegisterFor(getter, Enumerable.Empty<CollectibleObject>()
            .Concat(blocks)
            .Concat(items)
            .ToArray());
    }
}