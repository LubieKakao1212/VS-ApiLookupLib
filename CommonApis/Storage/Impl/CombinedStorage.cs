using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommonApis.Storage.Api;
using CommonApis.Transact.Api;
using Vintagestory.API.Common;

namespace CommonApis.Storage.Impl;

[Experimental("IStorage")]
public class CombinedStorage<TResource> : IStorage<TResource> where TResource : IResource<TResource> {
    
    public int SlotCount { get; }
    
    private readonly List<IStorage<TResource>> _storages;
    private readonly SlotMapping[] _slotMappings;

    private delegate TResult SlotAccessor<out TResult>(IStorage<TResource> storage, int inStorage);
    
    private CombinedStorage(IEnumerable<IStorage<TResource>> storages) {
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

    public long Insert(ITransactionContext transaction, TResource resource, long amount) {
        return DoInsert(amount, (storage, toInsert) => storage.Insert(transaction, resource, toInsert));
    }

    public long InsertMerging(ITransactionContext transaction, TResource resource, long amount) {
        return DoInsert(amount, (storage, toInsert) => storage.InsertMerging(transaction, resource, toInsert));
    }

    public ResourceStack<TResource> ExtractFirst(ITransactionContext transaction, long maxAmount, IStorage<TResource>.ExtractPredicate extractPredicate) {
        foreach (var storage in _storages) {
            var extracted = storage.ExtractFirst(transaction, maxAmount, extractPredicate);
            if (!extracted.IsEmpty) {
                return extracted;
            }
        }
        return ResourceStack<TResource>.Empty;
    }

    public long ExtractAny(ITransactionContext transaction, long maxAmount, IStorage<TResource>.ExtractPredicate extractPredicate) {
        long amountExtracted = 0;
        foreach (var storage in _storages) {
            amountExtracted += storage.ExtractAny(transaction, maxAmount, extractPredicate);
        }
        return amountExtracted;
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

    private long DoInsert(long amount, Func<IStorage<TResource>, long, long> inserter) {
        var toInsert = amount;
        foreach (var storage in _storages) {
            toInsert = inserter(storage, toInsert);

            if (toInsert == 0) {
                break;
            }
        }
        return amount - toInsert;
    }
    
    private IStorage<TResource> AsStorage() {
        return this;
    }
    
    private readonly struct SlotMapping {
        public required int StorageIdx { get; init; }
        public required int SlotIdx { get; init; }
    }
}