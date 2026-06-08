using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TransactApiLib.Storage.Impl;
using TransactApiLib.Transact.Api;

namespace TransactApiLib.Storage.Api;

[Experimental("IStorage")]
public static class StorageExtensions {

    /// <summary>
    /// Inserts a given <paramref name="amount"/> of a given <paramref name="resource"/> into <paramref name="storage"/> <br/>
    /// Prioritizes slots according to <paramref name="insertOrder"/>
    /// </summary>
    /// <param name="storage">Storage to insert into</param>
    /// <param name="transaction">Transaction this operation is part of</param>
    /// <param name="resource">Resource to be inserted</param>
    /// <param name="amount">Maximum amount to be inserted</param>
    /// <param name="insertOrder">Slot priority order</param>
    /// <returns>Actual amount inserted</returns>
    public static long Insert<TResource>(this IStorage<TResource> storage, ITransactionContext transaction, TResource resource, long amount, Storage.SlotSelectionOrder insertOrder = Storage.SlotSelectionOrder.First) where TResource : IResource<TResource> {
        return DoInsert(amount, 
            (stor, toInsert) => stor.Insert(transaction, 0, resource, toInsert),
            storage.GetSlotsInOrder(insertOrder)
            );
    }

    /// <summary>
    /// TODO Missing Documentation
    /// </summary>
    public static long InsertMerging<TResource>(this IStorage<TResource> storage, ITransactionContext transaction, TResource resource, long amount, Storage.SlotSelectionOrder insertOrder = Storage.SlotSelectionOrder.First) where TResource : IResource<TResource> {
        return DoInsert(amount, 
            (stor, toInsert) => stor.InsertMerging(transaction, 0, resource, toInsert),
                storage.GetSlotsInOrder(insertOrder)
            );
    }
    
    /// <summary>
    /// Extracts from the first slot of <paramref name="storage"/> with resource matched by <paramref name="extractPredicate"/>
    /// </summary>
    /// <param name="storage">Storage to extract from</param>
    /// <param name="transaction">Transaction this is operation part of</param>
    /// <param name="maxAmount">Maximum amount to extract</param>
    /// <param name="extractPredicate">Predicate checking what to extract</param>
    /// <returns><see cref="ResourceStack{TResource}">ResourceStack</see> containing the extracted resource and amount</returns>
    public static ResourceStack<TResource> ExtractFirst<TResource>(this IStorage<TResource> storage, ITransactionContext transaction, long maxAmount, IStorage<TResource>.ExtractPredicate extractPredicate)
        where TResource : IResource<TResource> {
        for (int i = 0; i<storage.SlotCount; i++) {
            var extracted = storage.Extract(transaction, i, maxAmount, extractPredicate);
            if (!extracted.IsEmpty) {
                return extracted;
            }
        }
        return ResourceStack<TResource>.Empty;
    }

    /// <summary>
    /// Extracts from all slots of <paramref name="storage"/> containing a resource matched by <paramref name="extractPredicate"/> <br/>
    /// Prioritizes lower index slots 
    /// </summary>
    /// <param name="storage">Storage to extract from</param>
    /// <param name="transaction">Transaction this is operation part of</param>
    /// <param name="maxAmount">Maximum amount to extract</param>
    /// <param name="extractPredicate">Predicate checking what to extract</param>
    /// <returns>Amount extracted</returns>
    public static long ExtractAny<TResource>(this IStorage<TResource> storage, ITransactionContext transaction, long maxAmount, IStorage<TResource>.ExtractPredicate extractPredicate) where TResource : IResource<TResource> {
        long totalExtracted = 0;
        long toExtract = maxAmount;
        for (int i = 0; i<storage.SlotCount; i++) {
            var extracted = storage.Extract(transaction, i, toExtract, extractPredicate);
            totalExtracted += extracted.amount;
            toExtract -= extracted.amount;
            if (toExtract == 0) {
                return maxAmount;
            }
        }
        return totalExtracted;
    }

    public static IEnumerable<IStorage<TResource>> NonEmptySlots<TResource>(this IStorage<TResource> storage) where TResource : IResource<TResource> {
        return storage.EmptyOrNonEmptySlots(false);
    }
    
    public static IEnumerable<IStorage<TResource>> EmptySlots<TResource>(this IStorage<TResource> storage) where TResource : IResource<TResource> {
        return storage.EmptyOrNonEmptySlots(true);
    }
    
    public static IEnumerable<IStorage<TResource>> EmptyOrNonEmptySlots<TResource>(this IStorage<TResource> storage, bool empty) where TResource : IResource<TResource> {
        for (int i = 0; i < storage.SlotCount; i++) {
            if (storage.GetContentInSlot(i).IsEmpty == empty) {
                yield return storage.Slot(i);
            }
        }
    }

    private static IEnumerable<IStorage<TResource>> GetSlotsInOrder<TResource>(this IStorage<TResource> storage, Storage.SlotSelectionOrder order) where TResource : IResource<TResource> => order switch {
        Storage.SlotSelectionOrder.First => Enumerable.Range(0, storage.SlotCount).Select(storage.Slot),
        Storage.SlotSelectionOrder.NonEmptyFirst => storage.NonEmptySlots().Concat(storage.EmptySlots()),
        Storage.SlotSelectionOrder.EmptyFirst => storage.EmptySlots().Concat(storage.NonEmptySlots()),
        Storage.SlotSelectionOrder.NonEmptyOnly => storage.NonEmptySlots(),
        Storage.SlotSelectionOrder.EmptyOnly => storage.EmptySlots(),
        _ => throw new ArgumentOutOfRangeException(nameof(order), order, null)
    };
    
    private static long DoInsert<TResource>(long amount, Func<IStorage<TResource>, long, long> inserter, IEnumerable<IStorage<TResource>> slotProvider) where TResource : IResource<TResource> {
        var toInsert = amount;
        foreach (var slot in slotProvider) {
            toInsert = inserter(slot, toInsert);
            
            if (toInsert == 0) {
                break;
            }
        }
        return amount - toInsert;
    }
}


[Experimental("IStorage")]
public static class Storage {

    /// <summary>
    /// TODO Missing Documentation
    /// </summary>
    public static IStorage<TResource> Combined<TResource>(params IStorage<TResource>[] storages) where TResource : IResource<TResource> {
        return Combined((IEnumerable<IStorage<TResource>>)storages);
    }
    
    /// <summary>
    /// TODO Missing Documentation
    /// </summary>
    public static IStorage<TResource> Combined<TResource>(IEnumerable<IStorage<TResource>> storages) where TResource : IResource<TResource> {
        return new CombinedStorage<TResource>(storages);
    }
    
    /// <summary>
    /// TODO Missing Documentation
    /// </summary>
    public static IStorage<TResource> Sliced<TResource>(this IStorage<TResource> storage, int start) where TResource : IResource<TResource> {
        return new SlicedStorage<TResource>(storage, start, storage.SlotCount - start);
    }
    
    /// <summary>
    /// TODO Missing Documentation
    /// </summary>
    public static IStorage<TResource> Sliced<TResource>(this IStorage<TResource> storage, int start, int slotCount) where TResource : IResource<TResource> {
        return new SlicedStorage<TResource>(storage, start, slotCount);
    }

    /// <summary>
    /// TODO Missing Documentation
    /// </summary>
    public static IStorage<TResource> Slot<TResource>(this IStorage<TResource> storage, int slot) where TResource : IResource<TResource> {
        return storage.Sliced(slot, 1);
    }

    public enum SlotSelectionOrder {
        First,
        NonEmptyFirst,
        EmptyFirst,
        NonEmptyOnly,
        EmptyOnly
    }
}
