using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ApiLookupLib.Helper;
using Vintagestory.API.Common;

namespace ApiLookupLib.Systems;

public class LookupCacheSystem : ModSystem {

    [NotNull] public MultiCache<Type, Block>? BlocksByCollectibleBehavior { get; private set; }
    [NotNull] public MultiCache<Type, Block>? BlocksByCollectibleBehaviorInherited { get; private set; }
    [NotNull] public MultiCache<Type, Block>? BlocksByEntityBehavior { get; private set; }
    [NotNull] public MultiCache<Type, Block>? BlocksByEntityBehaviorInherited { get; private set; }

    public override void Start(ICoreAPI api) {
        var world = api.World;
        var registry = api.ClassRegistry;
        BlocksByCollectibleBehavior = new MultiCache<Type, Block>(GetBlocksFunc(world, (block, type) => block.HasBehavior(type, false)));
        BlocksByCollectibleBehaviorInherited = new MultiCache<Type, Block>(GetBlocksFunc(world, (block, type) => block.HasBehavior(type, true)));
        BlocksByEntityBehavior = new MultiCache<Type, Block>(GetBlocksFunc(world, (block, type) => block.HasBlockEntityBehaviorType(registry, type, false)));
        BlocksByEntityBehaviorInherited = new MultiCache<Type, Block>(GetBlocksFunc(world, (block, type) => block.HasBlockEntityBehaviorType(registry, type, true)));
    }

    private System.Func<Type, List<Block>> GetBlocksFunc(IWorldAccessor world, System.Func<Block, Type, bool> hasBehaviorFuncIn) {
        return type => {
            var result = new List<Block>();
            foreach (var block in world.Blocks) {
                if (hasBehaviorFuncIn(block, type)) {
                    result.Add(block);
                }
            }
            return result;
        };
    }
    
}