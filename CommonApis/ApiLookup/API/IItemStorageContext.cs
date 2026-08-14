using System.Diagnostics.CodeAnalysis;
using CommonApis.ApiLookup.Impl.Item;
using CommonApis.Storage.Api;
using CommonApis.Storage.Api.Resource;
using CommonApis.Transact.Api;
using Vintagestory.API.Common;

namespace CommonApis.ApiLookup.API;

[Experimental("IStorage")]
public interface IItemStorageContext {

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

    void AcceptOverflow(ITransactionContext transaction, CollectibleResource resource, long amount);

    TValue? Find<TValue, TContext>(IItemStackApiLookup<TValue, TContext> lookup, IWorldAccessor world, TContext context) {
        return lookup.Get(world, OtherSlots.GetContentInSlot(SlotInStorage).AsItemStack()!, new ItemLookupContext<TContext>() {
            Context = context,
            Storage = this
        });
    }
}

[Experimental("IStorage")]
public static class ItemStorageContext {

    public static IItemStorageContext GenericVoidOverflow(IStorage<CollectibleResource> storage, int slot) {
        return new SimpleItemStorageContext(storage, slot);
    }
    
}