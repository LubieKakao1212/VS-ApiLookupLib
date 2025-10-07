using ApiLookupLib.API;
using ApiLookupLib.Helper;
using TemperatureApi.Api;
using Vintagestory.API.Common;

namespace TemperatureApi.Helper;

public ref struct TemperatureApis(IModLoader modLoader, IItemStackApiLookup<ITemperatureProvider, NoContext> itemStack, IBlockApiLookup<ITemperatureProvider, NoContext> block) {

    public IModLoader modLoader = modLoader;

    public IItemStackApiLookup<ITemperatureProvider, NoContext> ItemStack { get; } = itemStack;

    public IBlockApiLookup<ITemperatureProvider, NoContext> Block { get; } = block;


}