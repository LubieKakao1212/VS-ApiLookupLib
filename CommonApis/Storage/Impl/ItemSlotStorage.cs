using System;
using System.Diagnostics.CodeAnalysis;
using CommonApis.Storage.Api;
using CommonApis.Storage.Api.Resource;
using CommonApis.Transact.Api;
using Vintagestory.API.Common;

namespace CommonApis.Storage.Impl;

//Contains TODO
//TODO Mark dirty on final close
[Experimental("IStorage")]
public class ItemSlotStorage(IWorldAccessor world, ItemSlot itemSlot) : TransactionParticipant<ResourceStack<CollectibleResource>>, IStorage<CollectibleResource> {

    private ItemSlot ItemSlot => itemSlot;
    
    public int SlotCount => 1;

    public ResourceStack<CollectibleResource> GetContentInSlot(int slot) {
        AsStorage().AssertSlotIndex(slot);
        return ItemSlot.Itemstack.AsResourceStack();
    }

    public void SetContentInSlot(ITransactionContext transaction, int slot, CollectibleResource resource, long amount, bool force) {
        AsStorage().AssertSlotIndex(slot);
        TakeSnapshot(transaction);

        var amt = Math.Min(amount, ItemSlot.MaxSlotStackSize);

        if (amt < amount) {
            world.Logger.Debug($"While setting {nameof(ItemSlotStorage)} slot contents: Amount {amount} got trimmed due to underlying slot MaxSlotStackSize {ItemSlot.MaxSlotStackSize}");
        }
        var dummy = new DummySlot {
            Itemstack = resource.AsItemStack(amt)
        };

        if (!ItemSlot.TryFlipWith(dummy)) {
            world.Logger.Debug($"While setting {nameof(ItemSlotStorage)} slot contents: Failed to perform dummy slot swap");
            if (force) {
                world.Logger.Debug($"While setting {nameof(ItemSlotStorage)} slot contents: {nameof(force)} is set to true");
                ItemSlot.Itemstack = resource.AsItemStack(amt);
            }
        }
    }

    public long Insert(ITransactionContext transaction, int slot, CollectibleResource resource, long amount) {
        AsStorage().AssertSlotIndex(slot);
        TakeSnapshot(transaction);
        
        if (!ItemSlot.Empty) {
            var existingResource = CollectibleResource.From(ItemSlot.Itemstack);
            if (!existingResource.Equals(resource)) {
                return 0;
            }
        }
        var toInsert = new ResourceStack<CollectibleResource>(resource, amount);
        return toInsert.MergeOntoSlot(world, ItemSlot);
    }

    public long InsertMerging(ITransactionContext transaction, int slot, CollectibleResource resource, long amount) {
        AsStorage().AssertSlotIndex(slot);
        TakeSnapshot(transaction);
        
        var toInsert = new ResourceStack<CollectibleResource>(resource, amount);
        return toInsert.MergeOntoSlot(world, ItemSlot);
    }
    
    public ResourceStack<CollectibleResource> Extract(ITransactionContext transaction, int slot, long maxAmount, IStorage<CollectibleResource>.ExtractPredicate extractPredicate) {
        AsStorage().AssertSlotIndex(slot);
        TakeSnapshot(transaction);
        
        var amountInt = maxAmount > int.MaxValue ? int.MaxValue : (int) maxAmount;
        var extracted = ItemSlot.TakeOut(amountInt);
        return extracted.AsResourceStack();
    }

    protected override ResourceStack<CollectibleResource> CreateSnapshot() {
        return ItemSlot.Itemstack.AsResourceStack();
    }

    protected override void RestoreSnapshot(ResourceStack<CollectibleResource> snapshot) {
        ItemSlot.Itemstack = snapshot.AsItemStack();
    }
    
    private IStorage<CollectibleResource> AsStorage() {
        return this;
    }
}