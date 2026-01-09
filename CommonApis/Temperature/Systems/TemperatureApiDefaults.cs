using ApiLookupLib.API;
using ApiLookupLib.Helper;
using CommonApis.Temperature.Api;
using CommonApis.Temperature.Helper;
using CommonApis.Temperature.Impl;
using TemperatureApi.Impl;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace CommonApis.Temperature.Systems;

public class TemperatureApiDefaults : ModSystem {
    
    private static readonly ConstantTemperatureProvider _litTorchProvider = new(600f);

    public override double ExecuteOrder() {
        return 1.0;
    }

    public override void AssetsFinalize(ICoreAPI api) {
        RegisterDefaults(api);
    }

    public void RegisterDefaults(ICoreAPI api) {
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