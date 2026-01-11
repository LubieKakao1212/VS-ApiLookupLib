using System;
using System.Diagnostics.CodeAnalysis;
using ApiLookupLib.API;
using Vintagestory.API.Common;

namespace ApiLookupLib.Impl.Item;

[Experimental("ItemLookup_Experimental")]
public class StackItemAccess(ItemStack stack, bool immutable = false) : IItemAccess {
    public bool IsValid => true;

    public ItemStack CurrentStack => _currentStack;

    private ItemStack _currentStack = stack.Clone();
    
    public ItemStack? SetStack(ItemStack newStack) {
        if (immutable) {
            return null;
        }
        var old = CurrentStack;
        _currentStack = newStack;
        return old;
    }
}