using System.Diagnostics.CodeAnalysis;
using TransactApiLib.Transact.Api;

namespace TransactApiLib.Storage.Api;

[Experimental("IStorage")]
public static class StorageExtensions {

    /// <summary>
    /// Inserts a given <paramref name="amount"/> of a given <paramref name="resource"/> into <paramref name="storage"/> <br/>
    /// Prioritizes lower index slots
    /// </summary>
    /// <param name="storage">Storage to insert into</param>
    /// <param name="transaction">Transaction this operation is part of</param>
    /// <param name="resource">Resource to be inserted</param>
    /// <param name="amount">Maximum amount to be inserted</param>
    /// <returns>Actual amount inserted</returns>
    public static long Insert<TResource>(this IStorage<TResource> storage, ITransactionContext transaction, TResource resource, long amount) where TResource : IResource<TResource> {
        var toInsert = amount;
        for (int i = 0; i<storage.SlotCount; i++) {
            toInsert = storage.Insert(transaction, i, resource, toInsert);

            if (toInsert == 0) {
                break;
            }
        }
        return amount - toInsert;
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
    
}