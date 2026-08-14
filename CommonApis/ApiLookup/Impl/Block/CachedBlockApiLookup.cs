using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace CommonApis.ApiLookup.Impl.Block;

public class CachedBlockApiLookup<TValue, TContext>(ICoreAPI api) : SimpleBlockApiLookup<TValue, TContext>(api) {

    private readonly Dictionary<BlockPos, (LookupEntryValidator validator, List<(TValue? value, TContext context)> values)> _cache = new();
    
    public override TValue? Get(IWorldAccessor world, BlockPos source, TContext context) {
        bool hasResult = false;
        var result = default(TValue?);
        if (_cache.TryGetValue(source, out var cachedValue)) {
            var validator = cachedValue.validator;
            if (validator.IsValid(world, source)) {
                var vc = cachedValue.values
                    .Select(entry => ((TValue? value, TContext context)?)entry)
                    .FirstOrDefault(entry => entry?.context.Equals(context) ?? false, null);
                if (vc != null) {
                    hasResult = true;
                    result = vc.Value.value;
                }
            }
            else {
                _cache.Remove(source);
            }
        }
        
        if (hasResult) {
            result = base.Get(world, source, context);
            
            var cache = _cache.GetValueOrDefault(source);
            if (cache.values == null) {
                cache = (
                    LookupEntryValidator.Create(world, source),
                    new()
                    );
            }
            cache.values.Add((result, context));
            _cache[source] = cache;
        }

        return result;
    }

    private readonly struct LookupEntryKey(BlockPos pos, TContext context) : IEquatable<LookupEntryKey> {
        public BlockPos Pos { get; } = pos;
        public TContext Context { get; } = context;

        public bool Equals(LookupEntryKey other) {
            return Pos.Equals(other.Pos) && EqualityComparer<TContext>.Default.Equals(Context, other.Context);
        }

        public override bool Equals(object? obj) {
            return obj is LookupEntryKey other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Pos, Context);
        }
    }
    
    private readonly struct LookupEntryValidator(Vintagestory.API.Common.Block block, BlockEntity? blockEntity) {
        public Vintagestory.API.Common.Block Block { get; } = block;
        public BlockEntity? BlockEntity { get; } = blockEntity;

        public bool IsValid(IWorldAccessor world, BlockPos pos) {
            var access = world.BlockAccessor;
            var block = access.GetBlock(pos);
            var entity = access.GetBlockEntity(pos);

            //Not using Equals on purpose
            return Block == block && BlockEntity == entity;
        }

        public static LookupEntryValidator Create(IWorldAccessor world, BlockPos pos) {
            var access = world.BlockAccessor;
            var block = access.GetBlock(pos);
            var entity = access.GetBlockEntity(pos);

            return new LookupEntryValidator(block, entity);
        }
    }
}