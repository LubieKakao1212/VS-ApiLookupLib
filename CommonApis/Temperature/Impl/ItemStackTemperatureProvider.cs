using ApiLookupLib.API;
using CommonApis.Temperature.Api;
using Vintagestory.API.Common;
#pragma warning disable ItemLookup_Experimental

namespace CommonApis.Temperature.Impl;

public class ItemStackTemperatureProvider(IWorldAccessor world, IItemAccess itemAccess) : MutableTemperatureProviderBase(world.Logger) {

    private float _temperature = itemAccess.Collectible.GetTemperature(world, itemAccess.CurrentStack);
    
    public override float GetTemperature() {
        return _temperature;
    }

    public override void SetTemperature(float temp) {
        _temperature = temp;
    }

    protected override void ApplyChanges() {
        if (!itemAccess.IsValid) {
            world.Logger.Warning($"{nameof(IItemAccess)} is no longer valid, value will not be set");
            return;
        }
        var stack = itemAccess.CurrentStack;
        stack.Collectible.SetTemperature(world, stack, _temperature);
        itemAccess.SetStack(stack);
    }
}