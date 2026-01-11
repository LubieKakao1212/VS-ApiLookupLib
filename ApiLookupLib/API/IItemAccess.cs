using System;
using System.Diagnostics.CodeAnalysis;
using ApiLookupLib.Impl.Item;
using Vintagestory.API.Common;

namespace ApiLookupLib.API;

[Experimental("ItemLookup_Experimental")]
public interface IItemAccess {
    
    /// <summary>
    /// Is this access considered valid i.e. did not change
    /// </summary>
    bool IsValid { get; }
    
    /// <summary>
    /// Current stack provided by this context, returns a copy
    /// </summary>
    ItemStack CurrentStack { get; }

    /// <summary>
    /// Set the stack in this context
    /// </summary>
    /// <param name="newStack"></param>
    /// <returns>Previous stack or null if was unsuccessful</returns>
    ItemStack? SetStack(ItemStack newStack);

    #region Helpers
    CollectibleObject Collectible => CurrentStack.Collectible;

    void AssertValid() {
        if (!IsValid) {
            throw new InvalidOperationException($"This {GetType()} instance is no longer valid");
        }
    }
    #endregion

    static IItemAccess OfStack(ItemStack stack, bool immutable = false) {
        return new StackItemAccess(stack, immutable);
    }
    
    static IItemAccess OfSlot(ItemSlot slot) {
        return new SlotItemAccess(slot);
    }
}