using System;
using System.Diagnostics.CodeAnalysis;
using CommonApis.Storage.Api;
using CommonApis.Storage.Api.Resource;
using CommonApis.Transact.Api;

namespace CommonApis.ApiLookup.API;

/// <summary>
/// TODO I may need to rethink this, there a re several issues:
/// 1) If OnSlotDirty is triggered on every change, what should it do with snapshots?
/// 2) 
/// </summary>
[Experimental("IStorage")]
public interface IItemStorageContext {

    public event Action OnSlotDirty;

    public int SlotInStorage { get; }
    
    IStorage<CollectibleResource> OwnerStorage { get; }

    IStorage<CollectibleResource> MainSlot => OwnerStorage.Slot(SlotInStorage);
    
    IStorage<CollectibleResource> OtherSlots =>
        SlotInStorage == 0 ? OwnerStorage.Sliced(1) :
        SlotInStorage == OwnerStorage.SlotCount - 1 ? OwnerStorage.Sliced(0, OwnerStorage.SlotCount - 1) :
        Storages.Combined(
            OwnerStorage.Sliced(0,  SlotInStorage - 1), 
            OwnerStorage.Sliced(SlotInStorage + 1, OwnerStorage.SlotCount - (SlotInStorage + 1)));

    ResourceStack<CollectibleResource> GetStack() => OwnerStorage.GetContentInSlot(SlotInStorage);
    
    long InsertMainFirst(ITransactionContext transaction, CollectibleResource resource, long amount, bool allowMerge = true, Storages.SlotSelectionOrder nonMainSlotOrder = Storages.SlotSelectionOrder.First) {
        var toInsert = amount;
        var mainStorage = MainSlot;
        toInsert -= allowMerge
            ? mainStorage.InsertMerging(transaction, resource, amount)
            : mainStorage.Insert(transaction, resource, amount);

        if (toInsert == 0) {
            return amount;
        }
        
        var otherStorage = OtherSlots;
        toInsert -= allowMerge
            ? otherStorage.InsertMerging(transaction, resource, amount, nonMainSlotOrder)
            : otherStorage.Insert(transaction, resource, amount, nonMainSlotOrder);

        return amount - toInsert;
    }
}