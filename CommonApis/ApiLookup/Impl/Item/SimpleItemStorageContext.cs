using System.Diagnostics.CodeAnalysis;
using CommonApis.ApiLookup.API;
using CommonApis.Storage.Api;
using CommonApis.Storage.Api.Resource;
using CommonApis.Transact.Api;

namespace CommonApis.ApiLookup.Impl.Item;

/// <summary>
/// Does not implement overflow handling
/// </summary>
[Experimental("IStorage")]
public class SimpleItemStorageContext : IItemStorageContext {

    public int SlotInStorage { get; }

    public IStorage<CollectibleResource> OwnerStorage { get; }

    internal SimpleItemStorageContext(IStorage<CollectibleResource> storage, int slot) {
        OwnerStorage = storage;
        SlotInStorage = slot;
    }

    public void AcceptOverflow(ITransactionContext transaction, CollectibleResource resource, long amount) {
        //Do nothing in this impl
    }
}