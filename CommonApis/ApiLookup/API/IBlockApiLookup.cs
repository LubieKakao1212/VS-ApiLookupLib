using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace CommonApis.ApiLookup.API;

public interface IBlockApiLookup<TValue, TContext> : IApiLookupBase<TValue, TContext, BlockPos> {
    
    delegate TValue? GetterBlockEntity(BlockEntity be, TContext context);

    void RegisterForBlocks(Getter getter, params Block[] blocks);

    void RegisterForBlocks(Getter getter, AssetLocation wildcard);
    
    /// <summary>
    /// Registers a <see cref="IApiLookupBase{TValue,TContext,TSourceArg}.Getter">Getter</see> for all blocks with any of the provided behavior types
    /// </summary>
    /// <param name="getter">Getter to be registered</param>
    /// <param name="inherited">Should behaviors inherited from provided types also be accepted</param>
    /// <param name="behaviorTypes">Types of behaviors this Getter should be registered for</param>
    void RegisterForCollectibleBehaviors(Getter getter, bool inherited = false, params Type[] behaviorTypes);
    
    void RegisterForBlocks(GetterBlockEntity getter, params Block[] blocks);

    void RegisterForBlocks(GetterBlockEntity getter, AssetLocation wildcard);

    /// <summary>
    /// Registers a <see cref="GetterBlockEntity">GetterBlockEntity</see> for all blocks with BlockEntities of given types
    /// </summary>
    /// <param name="getter">GetterBlockEntity to be registered</param>
    /// <param name="beClasses">BlockEntity class codes to register this getter for</param>
    void RegisterForBlockEntityTypes(GetterBlockEntity getter, params string[] beClasses);
    
    /// <summary>
    /// Registers a <see cref="GetterBlockEntity">GetterBlockEntity</see> for all blocks with BlockEntities of given types
    /// </summary>
    /// <param name="getter">GetterBlockEntity to be registered</param>\
    /// <param name="inherited">Should BlockEntities inherited from provided types also be accepted</param>
    /// <param name="beTypes">BlockEntity Types to register this getter for</param>
    void RegisterForBlockEntityTypes(GetterBlockEntity getter, bool inherited = false, params Type[] beTypes);
    
    /// <summary>
    /// Registers a <see cref="GetterBlockEntity">GetterBlockEntity</see> for all blocks with any of the provided behavior types
    /// </summary>
    /// <param name="getter">GetterBlockEntity to be registered</param>
    /// <param name="inherited">Should behaviors inherited from provided types also be accepted</param>
    /// <param name="behaviorTypes">Types of behaviors this Getter should be registered for</param>
    void RegisterForBlockEntityBehaviors(GetterBlockEntity getter, bool inherited = false, params Type[] behaviorTypes);

}