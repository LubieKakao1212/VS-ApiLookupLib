using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommonApis.Storage.Api;
using CommonApis.Transact.Api;

namespace CommonApis.Storage.Impl;

[Experimental("IStorage")]
public class CombinedStorage<TResource> : IStorage<TResource> where TResource : IResource<TResource> {
    
    public int SlotCount { get; }
    
    private readonly List<IStorage<TResource>> _storages;
    private readonly SlotMapping[] _slotMappings;

    private delegate TResult SlotAccessor<out TResult>(IStorage<TResource> storage, int inStorage);
    
    internal CombinedStorage(IEnumerable<IStorage<TResource>> storages) {
        _storages = new List<IStorage<TResource>>(storages);

        var sum = 0;
        _slotMappings = _storages.Select(storage => {
            sum += storage.SlotCount;
            return storage.SlotCount;
        }).SelectMany(
            (count, i) => Enumerable.Range(0, count).Select(slotIdx => new SlotMapping {
                StorageIdx = i,
                SlotIdx = slotIdx
            })
        ).ToArray();
        SlotCount = sum;
    }
    
    public ResourceStack<TResource> GetContentInSlot(int slot) {
        AsStorage().AssertSlotIndex(slot);

        return AccessSlot(slot, (storage, inStorage) => storage.GetContentInSlot(inStorage));
    }

    public void SetContentInSlot(ITransactionContext transaction, int slot, TResource resource, long amount, bool force = false) {
        AsStorage().AssertSlotIndex(slot);

        AccessSlot<object?>(slot, (storage, inStorage) => {
            storage.SetContentInSlot(transaction, inStorage, resource, amount, force);
            return null;
        });
    }

    public long Insert(ITransactionContext transaction, int slot, TResource resource, long amount) {
        AsStorage().AssertSlotIndex(slot);

        return AccessSlot(slot, (storage, inStorage) => storage.Insert(transaction, inStorage, resource, amount));
    }

    public long InsertMerging(ITransactionContext transaction, int slot, TResource resource, long amount) {
        AsStorage().AssertSlotIndex(slot);

        return AccessSlot(slot, (storage, inStorage) => storage.InsertMerging(transaction, inStorage, resource, amount));
    }

    public ResourceStack<TResource> Extract(ITransactionContext transaction, int slot, long maxAmount, IStorage<TResource>.ExtractPredicate extractPredicate) {
        AsStorage().AssertSlotIndex(slot);
        
        return AccessSlot(slot, (storage, inStorage) => storage.Extract(transaction, inStorage, maxAmount, extractPredicate));
    }

    /// <summary>
    /// Does not perform a bounds check, <paramref name="slot"/> must be in range
    /// </summary>
    /// <param name="slot">Must be in range</param>
    /// <param name="action"></param>
    /// <typeparam name="TResult"></typeparam>
    private TResult AccessSlot<TResult>(int slot, SlotAccessor<TResult> action) {
        var mapping = _slotMappings[slot]; 
        return action(_storages[mapping.StorageIdx], mapping.SlotIdx);
    }
    
    private IStorage<TResource> AsStorage() {
        return this;
    }
    
    private readonly struct SlotMapping {
        public required int StorageIdx { get; init; }
        public required int SlotIdx { get; init; }
    }
}