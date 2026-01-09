using System;
using ApiLookupLib.API;
using ApiLookupLib.Helper;
using ApiLookupLib.Impl;
using TemperatureApi.Api;
using Vintagestory.API.Common;

namespace TemperatureApi {
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
            _itemStackLookup = new SimpleItemStackApiLookup<ITemperatureProvider, NoContext>(api.World);
            _blockLookup = new SimpleBlockApiLookup<ITemperatureProvider, NoContext>(api);
        }
        
        private static TValue GetApiOrFail<TValue>(TValue? value) {
            return value ?? throw new ApplicationException("TemperatureAPI lookups not yet initialized, available only After AssetsFinalize[0.0]");
        }
    }
}