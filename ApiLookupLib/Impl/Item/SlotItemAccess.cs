using System;
using System.Diagnostics.CodeAnalysis;
using ApiLookupLib.API;
using Vintagestory.API.Common;

namespace ApiLookupLib.Impl.Item;

[Experimental("ItemLookup_Experimental")]
public class SlotItemAccess : IItemAccess {

    public bool IsValid { get; private set; }

    private readonly ItemSlot _slot;
    
    public ItemStack CurrentStack { get; }
    
    public SlotItemAccess(ItemSlot slot) {
        _slot = slot;
        _slot.MarkedDirty += () => IsValid = false;
        var inv = slot.Inventory;
        if (inv != null) {
            inv.SlotModified += i => IsValid &= i != inv.GetSlotId(slot);
        }
        CurrentStack = _slot.Itemstack.Clone();
    }

    public ItemStack? SetStack(ItemStack newStack) {
        if (!_slot.Itemstack.Equals(_slot.Inventory.Api.World, CurrentStack)) {
            throw new InvalidOperationException("Slot content has changed, this is not allowed");
        }
        if (!_slot.CanTake()) {
            return null;
        }
        var dummySlot = new DummySlot(newStack);
        if (!_slot.CanHold(dummySlot)) {
            return null;
        }
        var it = _slot.TakeOutWhole();
        _slot.Itemstack = newStack;
        _slot.MarkDirty();
        return it;
    }
}