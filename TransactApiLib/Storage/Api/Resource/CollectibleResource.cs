using System;
using System.Diagnostics.CodeAnalysis;
using TransactApiLib.Storage.Api;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace TransactApiLib.Storage.Impl;

[Experimental("IStorage")]
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
        
        var stack = new ItemStack(Collectible, amount) {
            Attributes = attribs
        };
        return stack;
    }
    
    public ItemStack AsItemStack(long amount, ILogger? logger = null) {
        int amt = (int) amount;
        if (amount > int.MaxValue) {
            amt = int.MaxValue;
            logger?.Debug($"Amount {amount} is too big for and ItemStack stack size, limit={int.MaxValue}. Amount will be clamped");
        }
        return AsItemStack(amt);
    }
    
    public bool Equals(CollectibleResource? other) {
        return other != null && Collectible == other.Collectible && Attributes.Equals(other.Attributes);
    }
}

[Experimental("IStorage")]
public static class CollectibleResourceExtensions {

    /// <summary>
    /// Safe to call on nullable instances
    /// </summary>
    /// <returns></returns>
    public static ResourceStack<CollectibleResource> AsResourceStack(this ItemStack? stackIn) {
        if (stackIn == null) {
            return ResourceStack<CollectibleResource>.Empty;
        }
        return new ResourceStack<CollectibleResource>(CollectibleResource.From(stackIn), stackIn.StackSize);
    }
    
    
    /// <summary>
    /// Will return null if <paramref name="stackIn"/> is considered empty
    /// </summary>
    /// <returns></returns>
    public static ItemStack? AsItemStack(this ResourceStack<CollectibleResource> stackIn) {
        if (stackIn.IsEmpty) {
            return null;
        }

        var amt = checked((int)stackIn.amount);
        return stackIn.resource.AsItemStack(amt);
    }

}