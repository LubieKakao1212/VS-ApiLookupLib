using System;
using CommonApis.ApiLookup.API;
using CommonApis.ApiLookup.Helper;
using CommonApis.ApiLookup.Impl.Block;
using CommonApis.ApiLookup.Impl.Item;
using CommonApis.Temperature.Api;
using Vintagestory.API.Common;

#pragma warning disable ItemLookup_Experimental

namespace CommonApis.Temperature.Systems;

public class TemperatureApiModSystem : ModSystem {

    public IItemStackApiLookup<ITemperatureProvider, NoContext> ItemStack => GetApiOrFail(_itemStackLookup);
    public IBlockApiLookup<ITemperatureProvider, NoContext> Block => GetApiOrFail(_blockLookup);

    public override double ExecuteOrder() {
        return 0.0;
    }
        
    private IItemStackApiLookup<ITemperatureProvider, NoContext>? _itemStackLookup = null;
    //Consider using BlockFacing as context or Internal/External
    private IBlockApiLookup<ITemperatureProvider, NoContext>? _blockLookup = null;
        
    public override void AssetsFinalize(ICoreAPI api) {
        _itemStackLookup = new SimpleItemApiLookup<ITemperatureProvider, NoContext>(api);
        _blockLookup = new SimpleBlockApiLookup<ITemperatureProvider, NoContext>(api);
    }
        
    private static TValue GetApiOrFail<TValue>(TValue? value) {
        return value ?? throw new ApplicationException("TemperatureAPI lookups not yet initialized, available only After AssetsFinalize[0.0]");
    }
}