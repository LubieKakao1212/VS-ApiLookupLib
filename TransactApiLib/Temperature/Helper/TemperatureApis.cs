using ApiLookupLib.API;
using ApiLookupLib.Helper;
using TransactApiLib.Temperature.Api;
using Vintagestory.API.Common;
#pragma warning disable ItemLookup_Experimental

namespace TransactApiLib.Temperature.Helper;

public ref struct TemperatureApis(IModLoader modLoader, IItemStackApiLookup<ITemperatureProvider, NoContext> itemStack, IBlockApiLookup<ITemperatureProvider, NoContext> block) {

    public IModLoader modLoader = modLoader;

    public IItemStackApiLookup<ITemperatureProvider, NoContext> ItemStack { get; } = itemStack;

    public IBlockApiLookup<ITemperatureProvider, NoContext> Block { get; } = block;


}