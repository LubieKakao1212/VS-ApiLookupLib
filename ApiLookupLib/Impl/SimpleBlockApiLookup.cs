using System;
using System.Collections.Generic;
using System.Linq;
using ApiLookupLib.API;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ApiLookupLib.Impl;

public class SimpleBlockApiLookup<TValue, TContext>(IWorldAccessor worldIds) : SimpleApiLookup<TValue, TContext, BlockPos>, IBlockApiLookup<TValue, TContext> {

    private readonly MultiDictionary<Block, IApiLookupBase<TValue, TContext, BlockPos>.Getter> _blockLookup = new();
    private readonly MultiDictionary<Block, IBlockApiLookup<TValue, TContext>.GetterBlockEntity> _blockEntityLookup = new();
    
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

        return Get(world, pos, context);
    }

    public void RegisterForBlocks(IApiLookupBase<TValue, TContext, BlockPos>.Getter getter, params Block[] blocks) {
        _blockLookup.AddToAll(blocks, getter);
    }

    public void RegisterForBlocks(IApiLookupBase<TValue, TContext, BlockPos>.Getter getter, AssetLocation wildcard) {
        var blocks = worldIds.SearchBlocks(wildcard);

        if (blocks.Length == 0) {
            worldIds.Logger.Warning($"No blocks found for wildcard: {wildcard}");
            return;
        }
        
        RegisterForBlocks(getter, blocks);
    }

    public void RegisterForBlocks(IBlockApiLookup<TValue, TContext>.GetterBlockEntity getter, params Block[] blocks) {
        _blockEntityLookup.AddToAll(blocks, getter);
    }

    public void RegisterForBlocks(IBlockApiLookup<TValue, TContext>.GetterBlockEntity getter, AssetLocation wildcard) {
        var blocks = worldIds.SearchBlocks(wildcard);

        if (blocks.Length == 0) {
            worldIds.Logger.Warning($"No blocks found for wildcard: {wildcard}");
            return;
        }
        
        RegisterForBlocks(getter, blocks);
    }
}