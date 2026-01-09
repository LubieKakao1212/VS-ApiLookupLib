using ApiLookupLib.API;
using ApiLookupLib.Helper;
using Vintagestory.API.MathTools;

namespace TestingMod;

public static class ApiLookupRegistryExtensions {

    public static IBlockApiLookup<ITestApi, BlockFacing> TestApi(this ApiLookupRegistry reg) {
        return reg.modLoader.GetModSystem<TestApiLookupSystem>().BlockSided;
    }
    
}