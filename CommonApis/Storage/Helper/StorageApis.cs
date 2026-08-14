using System;
using System.Diagnostics.CodeAnalysis;
using CommonApis.ApiLookup.API;
using CommonApis.ApiLookup.Helper;
using CommonApis.Storage.Api;
using CommonApis.Storage.Api.Resource;
using CommonApis.Storage.Systems;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace CommonApis.Storage.Helper;

[Experimental("IStorage")]
public ref struct StorageApis(IModLoader modLoader) {
    public IModLoader ModLoader { get; } = modLoader;

    public ref struct CollectibleStorage(CollectibleStorageApiModSystem sys) {
        public readonly IBlockApiLookup<IStorage<CollectibleResource>, BlockFacing?> BlockSided => sys.BlockSided;
        public readonly IItemStackApiLookup<IStorage<CollectibleResource>, NoContext> ItemStack => sys.ItemStack;
    }
    
}

[Experimental("IStorage")]
public static class StorageApiExtensions {
    public static StorageApis Storage(this ApiLookupRegistry lookups) {
        return new StorageApis(lookups.modLoader);
    }

    public static StorageApis.CollectibleStorage Collectible(this StorageApis apis) {
        var sys = apis.ModLoader.GetModSystem<CollectibleStorageApiModSystem>(false);
        return new StorageApis.CollectibleStorage(sys);
    }
}