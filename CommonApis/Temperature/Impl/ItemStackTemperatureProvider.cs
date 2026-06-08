using System;
using CommonApis.ApiLookup.API;
using CommonApis.Storage.Api.Resource;
using Vintagestory.API.Common;

#pragma warning disable IStorage

namespace CommonApis.Temperature.Impl;

//Temporarily disabled
public class ItemStackTemperatureProvider(IWorldAccessor world, IItemStorageContext itemCtx) : MutableTemperatureProviderBase {
    
    private ItemStack? CurrentStack => itemCtx.GetStack().AsItemStack();

    public override float GetTemperature() {
        var stack = CurrentStack ?? throw new NullReferenceException("Cannot get temperature from a null ItemStack");
        return stack.Collectible.GetTemperature(world, stack);
    }

    protected override void SetTemperatureInternal(float temp) {
        var stack = CurrentStack ?? throw new NullReferenceException("Cannot set temperature to a null ItemStack");
        stack.Collectible.SetTemperature(world, stack, temp);
        //TODO make transaction to update containing storage
    }
}