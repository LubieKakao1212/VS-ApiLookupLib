using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace ApiLookupLib.API;

public interface IBlockApiLookup<TValue, TContext> : IApiLookupBase<TValue, TContext, BlockPos> {
    
    delegate TValue? GetterBlockEntity(BlockEntity be, TContext context);

    void RegisterForBlocks(Getter getter, params Block[] blocks);

    void RegisterForBlocks(Getter getter, AssetLocation wildcard);

    void RegisterForBlocks(GetterBlockEntity getter, params Block[] blocks);

    void RegisterForBlocks(GetterBlockEntity getter, AssetLocation wildcard);
}