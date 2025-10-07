using ApiLookupLib.API;
using ApiLookupLib.Helper;
using TemperatureApi.Api;
using TemperatureApi.Helper;
using TemperatureApi.Impl;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace TemperatureApi;

public static class TemperatureApiDefaults {
    
    private static readonly ConstantTemperatureProvider _litTorchProvider = new(600f);
    
    public static void RegisterDefaults(ICoreAPI api) {
        var apiSys = api.ApiLookups().TemperatureProviders();

        var itemLookup = apiSys.ItemStack;
        itemLookup.RegisterFallback(ItemStackFallbackGetter);

        var blockLookup = apiSys.Block;
        blockLookup.RegisterForBlocks(FirepitGetter, new AssetLocation("firepit-lit"));
        blockLookup.RegisterForBlocks(FirepitGetter, new AssetLocation("firepit-extinct"));
        blockLookup.RegisterForBlocks(FirepitGetter, new AssetLocation("firepit-cold"));
        
        blockLookup.RegisterForBlocks(ForgeGetter, new AssetLocation("forge"));
        
        blockLookup.RegisterForBlocks(CoalPileGetter, new AssetLocation("coalpile"));
        
        blockLookup.RegisterForBlocks(Constant(_litTorchProvider), new AssetLocation("torch-*-lit-*"));
    }
    
    private static ITemperatureProvider CoalPileGetter(BlockEntity be, NoContext context) {
        return new CoalPileTemperatureProvider((BlockEntityCoalPile)be);
    }

    private static ITemperatureProvider FirepitGetter(BlockEntity be, NoContext context) {
        return new FirepitTemperatureProvider((BlockEntityFirepit)be);
    }

    private static ITemperatureProvider ForgeGetter(BlockEntity be, NoContext context) {
        return new ForgeTemperatureProvider((BlockEntityForge)be);
    }

    private static IApiLookupBase<ITemperatureProvider, NoContext, BlockPos>.Getter Constant(ConstantTemperatureProvider provider) {
        return (world, pos, ctx) => provider;
    }
    
    private static ITemperatureProvider? ItemStackFallbackGetter(IWorldAccessor world, ItemStack stack, NoContext context) {
        if (CanItemStackHeat(stack)) {
            return new ItemStackTemperatureProvider(world, stack);
        }
        return null;
    }

    //Know I this name incorrect is
    private static bool CanItemStackHeat(ItemStack stack) {
        return stack.ItemAttributes?["allowHeating"] != null && stack.ItemAttributes["allowHeating"].AsBool();
    }
}