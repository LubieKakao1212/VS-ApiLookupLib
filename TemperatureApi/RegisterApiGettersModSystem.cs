using ApiLookupLib.Helper;
using TemperatureApi.Api;
using TemperatureApi.Helper;
using TemperatureApi.Impl;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace TemperatureApi;

public class RegisterApiGettersModSystem : ModSystem {

    public override double ExecuteOrder() {
        return 1.0;
    }

    public override void AssetsFinalize(ICoreAPI api) {
        var apiSys = api.ApiLookups().TemperatureProviders();

        var itemLookup = apiSys.ItemStack;
        itemLookup.RegisterFallback(ItemStackFallbackGetter);
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