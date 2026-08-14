using System.Diagnostics.CodeAnalysis;
using CommonApis.ApiLookup.Helper;
using CommonApis.Storage.Api;
using CommonApis.Storage.Api.Resource;
using CommonApis.Storage.Helper;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace CommonApis.Storage.Systems;

[Experimental("IStorage")]
public class CollectibleStorageApiDefaults : ModSystem {

    public override double ExecuteOrder() {
        return 1.0;
    }

    public override void AssetsFinalize(ICoreAPI api) {
        RegisterDefaults(api);
    }

    public void RegisterDefaults(ICoreAPI api) {
        var apiSys = api.ApiLookups().Storage().Collectible();

        var blockLookup = apiSys.BlockSided;
        blockLookup.RegisterForBlockEntityTypes((be, context) => ((BlockEntityContainer)be).Inventory.AsStorage(be.Api.World),
            "GenericContainer", "GenericTypedContainer");

        blockLookup.RegisterForBlockEntityTypes(BlockEntityForgeGetter, false, typeof(BlockEntityForge));
    }
    
    private static IStorage<CollectibleResource> BlockEntityForgeGetter(BlockEntity be, BlockFacing? facing) {
        var forge = (BlockEntityForge)be;
        var world = forge.Api.World;
        var mainStorage = forge.Inventory.AsStorage(world);

        if (facing == null) {
            return mainStorage;
        }

        if (facing == BlockFacing.UP || facing == BlockFacing.DOWN) {
            //work item
            return mainStorage.Slot(0);
        }
        
        //Fuel
        return mainStorage.Slot(1);
    }

}