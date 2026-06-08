using System.Diagnostics.CodeAnalysis;
using CommonApis.ApiLookup.API;
using CommonApis.ApiLookup.Impl.Block;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace TestingMod;

public class TestApiLookupSystem : ModSystem {
    
    [NotNull]
    public IBlockApiLookup<ITestApi, BlockFacing>? BlockSided { get; private set; }

    public override double ExecuteOrder() {
        return 0.0;
    }

    public override void AssetsFinalize(ICoreAPI api) {
        base.AssetsFinalize(api);

        BlockSided = new SimpleBlockApiLookup<ITestApi, BlockFacing>(api);
    }
}