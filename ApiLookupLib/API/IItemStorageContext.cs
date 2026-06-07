using System;
using System.Diagnostics.CodeAnalysis;
using TransactApiLib.Storage.Api;
using TransactApiLib.Storage.Api.Resource;

namespace ApiLookupLib.API;

public interface IItemStorageContext {

    public event Action OnSlotDirty;

    [Experimental("IStorage")] IStorage<CollectibleResource> OwnerStorage { get; }
    
}