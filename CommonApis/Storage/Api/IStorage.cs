using System;
using System.Diagnostics.CodeAnalysis;
using CommonApis.Transact.Api;

namespace CommonApis.Storage.Api;

[Experimental("IStorage")]
public interface IStorage<TResource> where TResource : IResource<TResource> {

    delegate bool ExtractPredicate(TResource resource);
    
    int SlotCount { get; }
    
    ResourceStack<TResource> GetContentInSlot(int slot);

    /// <summary>
    /// Inserts a given amount of a given resource into this storage DOES NOT ATTEMPT to merge with similar resources
    /// </summary>
    /// <param name="transaction">Transaction this is part of</param>
    /// <param name="resource">Resource to be inserted</param>
    /// <param name="amount">Maximum amount to be inserted</param>
    /// <returns>Actual amount inserted</returns>
    long Insert(ITransactionContext transaction, TResource resource, long amount);
    
    /// <summary>
    /// Inserts a given amount of a given resource into this storage DOES ATTEMPT to merge with similar resources 
    /// </summary>
    /// <param name="transaction">Transaction of which this is operation part of</param>
    /// <param name="resource">Resource to be inserted, matched using</param>
    /// <param name="amount">Maximum amount to be inserted</param>
    /// <returns>Actual amount inserted</returns>
    long InsertMerging(ITransactionContext transaction, TResource resource, long amount);
    
    /// <summary>
    /// Extracts from this Storage slots containing given <paramref name="extractPredicate"/> (matching exactly)
    /// </summary>
    /// <param name="transaction"></param>
    /// <param name="extractPredicate"></param>
    /// <param name="maxAmount"></param>
    /// <returns></returns>
    ResourceStack<TResource> ExtractFirst(ITransactionContext transaction, long maxAmount, ExtractPredicate extractPredicate);
    
    /// <summary>
    /// Extracts from this Storage from slots containing a resource matched by <paramref name="extractPredicate"/>
    /// </summary>
    /// <param name="transaction"></param>
    /// <param name="extractPredicate"></param>
    /// <param name="maxAmount"></param>
    /// <returns></returns>
    long ExtractAny(ITransactionContext transaction, long maxAmount, ExtractPredicate extractPredicate);

    void AssertSlotIndex(int slot) {
        if (slot < 0 || slot >= SlotCount) {
            throw new IndexOutOfRangeException($"{slot} < 0 ||  {slot} >= SlotCount");
        }
    }
    
}