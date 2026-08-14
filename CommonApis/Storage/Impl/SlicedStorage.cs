using System;
using System.Diagnostics.CodeAnalysis;
using CommonApis.Storage.Api;
using CommonApis.Transact.Api;

namespace CommonApis.Storage.Impl;

[Experimental("IStorage")]
public class SlicedStorage<TResource> : IStorage<TResource> where TResource : IResource<TResource> {
    
    public int SlotCount { get; }

    private readonly IStorage<TResource> _storage;
    private readonly int _firstSlot;
    
    public SlicedStorage(IStorage<TResource> storage, int firstSlot, int slotCount) {
        if (firstSlot < 0 || slotCount < 1) {
            throw new IndexOutOfRangeException();
        }
        
        var bound = firstSlot + slotCount;
        if (bound > storage.SlotCount) {
            throw new IndexOutOfRangeException();
        }
        _storage = storage;
        _firstSlot = firstSlot;
        SlotCount = slotCount;
    }

    public ResourceStack<TResource> GetContentInSlot(int slot) {
        return _storage.GetContentInSlot(GetSlotIdx(slot));
    }

    public void SetContentInSlot(ITransactionContext transaction, int slot, TResource resource, long amount, bool force = false) {
        _storage.SetContentInSlot(transaction, GetSlotIdx(slot), resource, amount, force);
    }

    public long Insert(ITransactionContext transaction, int slot, TResource resource, long amount) {
        return _storage.Insert(transaction, GetSlotIdx(slot), resource, amount);
    }

    public long InsertMerging(ITransactionContext transaction, int slot, TResource resource, long amount) {
        return _storage.InsertMerging(transaction, GetSlotIdx(slot), resource, amount);
    }

    public ResourceStack<TResource> Extract(ITransactionContext transaction, int slot, long maxAmount, IStorage<TResource>.ExtractPredicate extractPredicate) {
        return _storage.Extract(transaction, GetSlotIdx(slot), maxAmount, extractPredicate);
    }

    private int GetSlotIdx(int slot) {
        slot -= _firstSlot;
        ((IStorage<TResource>)this).AssertSlotIndex(slot);
        return slot;
    }
}