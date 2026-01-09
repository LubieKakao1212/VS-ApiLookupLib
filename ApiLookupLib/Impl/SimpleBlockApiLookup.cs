using System;
using System.Collections.Generic;
using System.Linq;
using ApiLookupLib.API;
using ApiLookupLib.Helper;
using ApiLookupLib.Systems;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace ApiLookupLib.Impl;

public class SimpleBlockApiLookup<TValue, TContext>(ICoreAPI api) : SimpleApiLookup<TValue, TContext, BlockPos>, IBlockApiLookup<TValue, TContext> {

    private readonly LookupCacheSystem _cache = api.ModLoader.GetModSystem<LookupCacheSystem>();
    private readonly IWorldAccessor _worldIds = api.World;
    private readonly IClassRegistryAPI _classes = api.ClassRegistry;
    
    private readonly MultiDictionary<Block, IApiLookupBase<TValue, TContext, BlockPos>.Getter> _blockLookup = new();
    private readonly MultiDictionary<Block, IBlockApiLookup<TValue, TContext>.GetterBlockEntity> _blockEntityLookup = new();
    
    // private readonly MultiDictionary<Block, IApiLookupBase<TValue, TContext, BlockPos>.Getter> _collectibleBehaviorLookup = new();
    // private readonly MultiDictionary<Block, IBlockApiLookup<TValue, TContext>.GetterBlockEntity> _blockEntityBehaviorLookup = new();
    
    public override TValue? Get(IWorldAccessor world, BlockPos pos, TContext context) {
        var accessor = world.BlockAccessor;
        var block = accessor.GetBlock(pos);
        var be = accessor.GetBlockEntity(pos);
        if (be != null) {
            foreach (var getter in _blockEntityLookup.GetAllOrEmpty(block)) {
                var value = getter(be, context);
                if (value != null) {
                    return value;
                }
            }
        }
        
        foreach (var getter in _blockLookup.GetAllOrEmpty(block)) {
            var value = getter(world, pos, context);
            if (value != null) {
                return value;
            }
        }

        return base.Get(world, pos, context);
    }

    public void RegisterForBlocks(IApiLookupBase<TValue, TContext, BlockPos>.Getter getter, params Block[] blocks) {
        _blockLookup.AddToAll(blocks, getter);
    }

    public void RegisterForBlocks(IApiLookupBase<TValue, TContext, BlockPos>.Getter getter, AssetLocation wildcard) {
        var blocks = _worldIds.SearchBlocks(wildcard);

        if (blocks.Length == 0) {
            _worldIds.Logger.Warning($"No blocks found for wildcard: {wildcard}");
            return;
        }
        
        RegisterForBlocks(getter, blocks);
    }

    public void RegisterForCollectibleBehaviors(IApiLookupBase<TValue, TContext, BlockPos>.Getter getter, bool inherited = false, params Type[] behaviorTypes) {
        var blocksFiltered = new List<Block>();
        
        blocksFiltered.AddRange(inherited
            ? _cache.BlocksByCollectibleBehaviorInherited.GetWithAny(behaviorTypes)
            : _cache.BlocksByCollectibleBehavior.GetWithAny(behaviorTypes));
        
        RegisterForBlocks(getter, blocksFiltered.ToArray());
    }

    public void RegisterForBlocks(IBlockApiLookup<TValue, TContext>.GetterBlockEntity getter, params Block[] blocks) {
        _blockEntityLookup.AddToAll(blocks.Where(block => block.EntityClass != null), getter);
    }

    public void RegisterForBlocks(IBlockApiLookup<TValue, TContext>.GetterBlockEntity getter, AssetLocation wildcard) {
        var blocks = _worldIds.SearchBlocks(wildcard);

        if (blocks.Length == 0) {
            _worldIds.Logger.Warning($"No blocks found for wildcard: {wildcard}");
            return;
        }
        
        RegisterForBlocks(getter, blocks);
    }

    public void RegisterForBlockEntityTypes(IBlockApiLookup<TValue, TContext>.GetterBlockEntity getter, params string[] beClasses) {
        RegisterForBlocks(getter, 
            _worldIds.Blocks
                .Where(
                    block => beClasses
                        .Any(beClass => Equals(block.EntityClass, beClass))
                    )
                .ToArray()
            );
    }

    public void RegisterForBlockEntityTypes(IBlockApiLookup<TValue, TContext>.GetterBlockEntity getter, bool inherited = false, params Type[] beTypes) {
        RegisterForBlocks(getter, 
            _worldIds.Blocks
                .Where(
                    block => beTypes
                        .Any(beType => beType.TypeMatches(_classes.GetBlockEntity(block.EntityClass ?? ""), inherited))
                )
                .ToArray()
        );
    }

    public void RegisterForBlockEntityBehaviors(IBlockApiLookup<TValue, TContext>.GetterBlockEntity getter, bool inherited = false, params Type[] behaviorTypes) {
        var blocksFiltered = new List<Block>();

        blocksFiltered.AddRange(inherited
            ? _cache.BlocksByEntityBehaviorInherited.GetWithAny(behaviorTypes)
            : _cache.BlocksByEntityBehavior.GetWithAny(behaviorTypes));


        RegisterForBlocks(getter, blocksFiltered.ToArray());
    }
}