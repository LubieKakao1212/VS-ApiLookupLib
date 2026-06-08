using CommonApis.ApiLookup.API;
using CommonApis.ApiLookup.Helper;
using Vintagestory.API.MathTools;

namespace TestingMod;

public static class ApiLookupRegistryExtensions {

    public static IBlockApiLookup<ITestApi, BlockFacing> TestApi(this ApiLookupRegistry reg) {
        return reg.modLoader.GetModSystem<TestApiLookupSystem>().BlockSided;
    }
    
}