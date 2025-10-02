using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace ApiLookupLib.API;

public interface IBlockApiLookup<TValue, TContext> : IApiLookupBase<TValue, TContext, BlockPos> {
    
    delegate TValue? GetterBlockEntity<in TEntity>(TEntity be, TContext context) where TEntity : BlockEntity;

    void RegisterForBlocks(Getter getter, params Block[] blocks);

    void RegisterForBlocks(Getter getter, AssetLocation wildcard);

    void RegisterForBlocks<TEntity>(GetterBlockEntity<TEntity> getter, params Block[] blocks) where TEntity : BlockEntity;

    void RegisterForBlocks<TEntity>(GetterBlockEntity<TEntity> getter, AssetLocation wildcard) where TEntity : BlockEntity;
}