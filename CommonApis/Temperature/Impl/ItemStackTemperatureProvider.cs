using System;
using CommonApis.ApiLookup.API;
using CommonApis.Storage.Api;
using CommonApis.Storage.Api.Resource;
using CommonApis.Temperature.Api;
using CommonApis.Transact.Api;
using Vintagestory.API.Common;

#pragma warning disable IStorage

namespace CommonApis.Temperature.Impl;

//Temporarily disabled
public class ItemStackTemperatureProvider(IWorldAccessor world, IItemStorageContext itemCtx) : IMutableTemperatureProvider {
    
    private ItemStack? CurrentStack => itemCtx.GetStack().AsItemStack();

    public float GetTemperature() {
        var stack = CurrentStack ?? throw new NullReferenceException("Cannot get temperature from a null ItemStack");
        return stack.Collectible.GetTemperature(world, stack);
    }

    public void SetTemperature(ITransactionContext transaction, float temp) {
        var stack = CurrentStack ?? throw new NullReferenceException("Cannot set temperature to a null ItemStack");
        stack.Collectible.SetTemperature(world, stack, temp);
        itemCtx.MainSlot.TrySwapResource(transaction, CollectibleResource.From(stack));
    }
}