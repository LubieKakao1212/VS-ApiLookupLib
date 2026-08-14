using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommonApis.Storage.Api;
using CommonApis.Storage.Api.Resource;
using CommonApis.Storage.Impl;
using Vintagestory.API.Common;

namespace CommonApis.Storage.Helper;

[Experimental("IStorage")]
public static class CollectibleStorage {

    public static IStorage<CollectibleResource> FromInventory(IWorldAccessor world, IInventory inventory) {
        var slots = new List<IStorage<CollectibleResource>>(inventory.Count);
        
        for (int i = 0; i < inventory.Count; i++) {
            slots.Add(FromSlot(world, inventory[i]!));
        }
        return Storages.Combined(slots);
    }

    public static IStorage<CollectibleResource> AsStorage(this IInventory inventory, IWorldAccessor world) {
        return FromInventory(world, inventory);
    }
    
    public static IStorage<CollectibleResource> FromSlot(IWorldAccessor world, ItemSlot slot) {
        return new ItemSlotStorage(world, slot);
    }

    public static IStorage<CollectibleResource> AsStorage(this ItemSlot slot, IWorldAccessor world) {
        return FromSlot(world, slot);
    }
    
}