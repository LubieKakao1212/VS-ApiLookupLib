using System;
using System.Diagnostics.CodeAnalysis;
using TransactApiLib.Storage.Api;
using TransactApiLib.Transact.Api;
using Vintagestory.API.Common;

namespace TransactApiLib.Storage.Impl;

//Contains TODO
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
        return DoInsert(resource, amount);
    }

    // TODO figure out how to implement this via extensions
    // public long InsertMerging(ITransactionContext transaction, CollectibleResource resource, long amount) {
    //     TakeSnapshot(transaction);
    //     
    //     var amountInt = amount > int.MaxValue ? int.MaxValue : (int) amount;
    //     
    //     if (!ItemSlot.Empty) {
    //         var existingStack = ItemSlot.Itemstack;
    //         var existingStackSize = existingStack.StackSize;
    //         var existingResource = CollectibleResource.From(existingStack);
    //         
    //         //TODO merging large stacks may result in incorrect resource after merge, due to actual slot not being able to accept the requested amount
    //         if (!resource.TryMergeOnto(world, existingResource, new IntRatio(amountInt, existingStackSize), out var merged)) {
    //             return 0;
    //         }
    //
    //         using (var swapTransaction = transaction.OpenNested()) {
    //             //Removes old stack
    //             ItemSlot.TakeOutWhole();
    //             //Reinsert with new resource
    //             var reinsertedAmount = DoInsert(merged, existingStackSize);
    //             if (reinsertedAmount != existingStackSize || 
    //                 ItemSlot.StackSize != existingStackSize || 
    //                 !CollectibleResource.From(ItemSlot.Itemstack).Equals(merged)) {
    //                 //We failed to properly swap the stack type -> swapTransaction ends without being commited and we return 0
    //                 return 0;
    //             }
    //             swapTransaction.Commit();
    //         }
    //         return DoInsert(merged, amountInt);
    //     }
    //     
    //     return DoInsert(resource, amount);
    // }
    
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

    private long DoInsert(CollectibleResource resource, long amount) {
        var amountInt = amount > int.MaxValue ? int.MaxValue : (int) amount;
        
        var stack = resource.AsItemStack(amountInt);
        var slotTmp = new DummySlot(stack);
        
        slotTmp.TryPutInto(world, ItemSlot, amountInt);
        return amountInt - slotTmp.StackSize;
    }
    
    private IStorage<CollectibleResource> AsStorage() {
        return this;
    }
}