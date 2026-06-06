using System;
using System.Diagnostics.CodeAnalysis;
using TransactApiLib.Transact.Api;

namespace TransactApiLib.Storage.Api;

[Experimental("IStorage")]
public interface IStorage<TResource> where TResource : IResource<TResource> {

    delegate bool ExtractPredicate(TResource resource);
    
    int SlotCount { get; }
    
    ResourceStack<TResource> GetContentInSlot(int slot);

    /// <summary>
    /// Sets the contents of a given slot, may not always behave as expected
    /// </summary>
    /// <param name="transaction">Transaction this is part of</param>
    /// <param name="slot"></param>
    /// <param name="resource"></param>
    /// <param name="amount"></param>
    /// <param name="force">If set to true, operation will never fail, however it may cause unexpected behavior, use with caution</param>
    void SetContentInSlot(ITransactionContext transaction, int slot, TResource resource, long amount, bool force = false);
    
    /// <summary>
    /// Inserts a given amount of a given resource into a specific slot of this storage DOES NOT ATTEMPT to merge with similar resources
    /// </summary>
    /// <param name="transaction">Transaction this is part of</param>
    /// <param name="slot">Slot to insert into, must be in bound [0, <see cref="SlotCount"/>)</param>
    /// <param name="resource">Resource to be inserted</param>
    /// <param name="amount">Maximum amount to be inserted</param>
    /// <returns>Actual amount inserted</returns>
    long Insert(ITransactionContext transaction, int slot, TResource resource, long amount);

    //TODO move to an extension of IStorage<CollectibleResource>
    // /// <summary>
    // /// Inserts a given amount of a given resource  into a specific slot of this storage DOES ATTEMPT to merge with similar resources 
    // /// </summary>
    // /// <param name="transaction">Transaction this is operation part of</param>
    // /// <param name="slot">Slot to insert into, must be in bound [0, <see cref="SlotCount"/>)</param>
    // /// <param name="resource">Resource to be inserted, matched using</param>
    // /// <param name="amount">Maximum amount to be inserted</param>
    // /// <returns>Actual amount inserted</returns>
    // long InsertMerging(ITransactionContext transaction, TResource resource, long amount);

    /// <summary>
    /// Extracts from a given slot of this storage, if <paramref name="extractPredicate"/> returns false the result will be <see cref="ResourceStack{TResource}.Empty">ResourceStack.Empty</see>
    /// </summary>
    /// <param name="transaction">Transaction this is operation part of</param>
    /// <param name="slot">Slot to extract from, must be in bound [0, <see cref="SlotCount"/>)</param>
    /// <param name="maxAmount">Maximum amount to extract</param>
    /// <param name="extractPredicate">Predicate checking if the extraction should happen</param>
    /// <returns><see cref="ResourceStack{TResource}">ResourceStack</see> containing the extracted resource and amount</returns>
    ResourceStack<TResource> Extract(ITransactionContext transaction, int slot, long maxAmount, ExtractPredicate extractPredicate);
    
    void AssertSlotIndex(int slot) {
        if (slot < 0 || slot >= SlotCount) {
            throw new IndexOutOfRangeException($"{slot} < 0 ||  {slot} >= SlotCount");
        }
    }
    
}