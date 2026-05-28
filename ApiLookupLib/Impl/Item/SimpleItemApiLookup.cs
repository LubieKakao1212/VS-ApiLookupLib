using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ApiLookupLib.API;
using ApiLookupLib.Helper;
using ApiLookupLib.Systems;
using Vintagestory.API.Common;

namespace ApiLookupLib.Impl.Item;

[Experimental("ItemLookup_Experimental")]
public class SimpleItemApiLookup<TValue, TContext>(ICoreAPI api) : SimpleApiLookup<TValue, TContext, ItemStack>, IItemStackApiLookup<TValue, TContext> {
    
    private readonly LookupCacheSystem _cache = api.ModLoader.GetModSystem<LookupCacheSystem>();
    private readonly IWorldAccessor _worldIds = api.World;
    private readonly IClassRegistryAPI _classes = api.ClassRegistry;
    
    private readonly MultiDictionary<CollectibleObject, IApiLookupBase<TValue, TContext, ItemStack>.Getter> _itemsLookup = new();

    public override TValue? Get(IWorldAccessor world, ItemStack source, TContext context) {
        var collectible = source.Collectible;

        foreach (var getter in _itemsLookup.GetAllOrEmpty(collectible)) {
            var value = getter(world, source, context);
            if (value != null) {
                return value;
            }
        }
        return base.Get(world, source, context);
    }

    public void RegisterForCollectibles(IApiLookupBase<TValue, TContext, ItemStack>.Getter getter, params CollectibleObject[] collectibles) {
        _itemsLookup.AddToAll(collectibles, getter);
    }

    public void RegisterForCollectibles(IApiLookupBase<TValue, TContext, ItemStack>.Getter getter, AssetLocation wildcard) {
        var blocks = _worldIds.SearchBlocks(wildcard);
        var items = _worldIds.SearchItems(wildcard);
        
        if (blocks.Length == 0 && items.Length == 0) {
            _worldIds.Logger.Warning($"No blocks or items found for wildcard: {wildcard}");
            return;
        }
        
        RegisterForCollectibles(getter, Enumerable.Empty<CollectibleObject>()
            .Concat(blocks)
            .Concat(items)
            .ToArray());
    }

    public void RegisterForBehaviors(IApiLookupBase<TValue, TContext, ItemStack>.Getter getter, bool inherited, params Type[] behaviorTypes) {
        var blocksFiltered = new List<CollectibleObject>();
        
        blocksFiltered.AddRange(inherited
            ? _cache.CollectiblesByCollectibleBehavior.GetWithAny(behaviorTypes)
            : _cache.CollectiblesByCollectibleBehaviorInherited.GetWithAny(behaviorTypes));
        
        RegisterForCollectibles(getter, blocksFiltered.ToArray());
    }

    public void RegisterForTypes(IApiLookupBase<TValue, TContext, ItemStack>.Getter getter, bool inherited, params Type[] collectibleTypes) {
        RegisterForCollectibles(getter, 
            _worldIds.Collectibles
                .Where(
                    col => collectibleTypes.Any(colType => colType.TypeMatches(col.GetType(), inherited))
                )
                .ToArray()
        );
    }
}