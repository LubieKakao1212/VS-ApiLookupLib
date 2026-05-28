using System;
using System.Diagnostics.CodeAnalysis;
using CommonApis.Storage.Api;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace CommonApis.Storage.Impl;

public class CollectibleResource(CollectibleObject collectible, ITreeAttribute attributes) : IResource<CollectibleResource> {

    public CollectibleObject Collectible { get; } = collectible;
    //TODO make immutable
    public ITreeAttribute Attributes { get; } = attributes;

    public static CollectibleResource From(ItemStack stack) {
        return new CollectibleResource(stack.Collectible, stack.Attributes.Clone());
    }
    
    /// <summary>
    /// Use to figure out the result of merging two resources at a given ratio, due to technical limitations sum of <see cref="IntRatio.numerator"/> and <see cref="IntRatio.denominator"/> must be less than the stack size sink collectible
    /// </summary>
    /// <param name="world"></param>
    /// <param name="other">Sink resource</param>
    /// <param name="ratio">Ratio at which the merge occurs</param>
    /// <param name="result">Result of merging</param>
    /// <returns></returns>
    public bool TryMergeOnto(IWorldAccessor world, CollectibleResource other, IntRatio ratio, [NotNullWhen(true)] out CollectibleResource? result) {
        var slotFrom = new DummySlot();
        var slotOnto = new DummySlot();
        
        slotFrom.Itemstack = AsItemStack(ratio.numerator);
        slotOnto.Itemstack = other.AsItemStack(ratio.denominator);

        if (slotFrom.TryPutInto(world, slotOnto, ratio.numerator) != 0) {
            result = null;
            return false;
        }

        result = From(slotOnto.Itemstack);
        return true;
    }

    public ItemStack AsItemStack(int amount) {
        var attribs = Attributes.Clone();

        var stack = Collectible switch {
            Item item => new ItemStack(item, amount),
            Block block => new ItemStack(block, amount),
            _ => throw new ApplicationException("Impossible!!!")
        };
        stack.Attributes = attribs;
        
        return stack;
    }
    
    public bool Equals(CollectibleResource? other) {
        return other != null && Collectible == other.Collectible && Attributes.Equals(other.Attributes);
    }
}