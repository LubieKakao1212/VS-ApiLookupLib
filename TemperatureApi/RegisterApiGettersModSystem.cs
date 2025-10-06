using ApiLookupLib.API;
using ApiLookupLib.Helper;
using TemperatureApi.Api;
using TemperatureApi.Helper;
using TemperatureApi.Impl;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

namespace TemperatureApi;

public class RegisterApiGettersModSystem : ModSystem {

    private readonly ConstantTemperatureProvider _litTorchProvider = new(600f);
    
    public override double ExecuteOrder() {
        return 1.0;
    }

    public override void AssetsFinalize(ICoreAPI api) {
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
    
    private ITemperatureProvider CoalPileGetter(BlockEntity be, NoContext context) {
        return new CoalPileTemperatureProvider((BlockEntityCoalPile)be);
    }

    private ITemperatureProvider FirepitGetter(BlockEntity be, NoContext context) {
        return new FirepitTemperatureProvider((BlockEntityFirepit)be);
    }

    private ITemperatureProvider ForgeGetter(BlockEntity be, NoContext context) {
        return new ForgeTemperatureProvider((BlockEntityForge)be);
    }

    private IApiLookupBase<ITemperatureProvider, NoContext, BlockPos>.Getter Constant(ConstantTemperatureProvider provider) {
        return (world, pos, ctx) => provider;
    }
    
    private ITemperatureProvider? ItemStackFallbackGetter(IWorldAccessor world, ItemStack stack, NoContext context) {
        if (CanItemStackHeat(stack)) {
            return new ItemStackTemperatureProvider(world, stack);
        }
        return null;
    }

    //Know I this name incorrect is
    private bool CanItemStackHeat(ItemStack stack) {
        return stack.ItemAttributes?["allowHeating"] != null && stack.ItemAttributes["allowHeating"].AsBool();
    }
    
}