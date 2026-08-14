using System;
using System.Diagnostics.CodeAnalysis;
using CommonApis.ApiLookup.API;
using CommonApis.ApiLookup.Helper;
using CommonApis.ApiLookup.Impl.Block;
using CommonApis.ApiLookup.Impl.Item;
using CommonApis.Storage.Api;
using CommonApis.Storage.Api.Resource;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace CommonApis.Storage.Systems;

[Experimental("IStorage")]
public class CollectibleStorageApiModSystem : ModSystem {
    
    public IItemStackApiLookup<IStorage<CollectibleResource>, NoContext> ItemStack => GetApiOrFail(_itemStack);
    public IBlockApiLookup<IStorage<CollectibleResource>, BlockFacing?> BlockSided => GetApiOrFail(_blockSided);
    
    private IItemStackApiLookup<IStorage<CollectibleResource>, NoContext>? _itemStack = null;
    private IBlockApiLookup<IStorage<CollectibleResource>, BlockFacing?>? _blockSided = null;

    public override double ExecuteOrder() {
        return 0.0;
    }

    public override void AssetsLoaded(ICoreAPI api) {
        _itemStack = new SimpleItemApiLookup<IStorage<CollectibleResource>, NoContext>(api);
        _blockSided = new CachedBlockApiLookup<IStorage<CollectibleResource>, BlockFacing?>(api);
    }
    
    private static TValue GetApiOrFail<TValue>(TValue? value) {
        return value ?? throw new ApplicationException("Storage APIs lookups not yet initialized, available only After AssetsFinalize[0.0]");
    }
}