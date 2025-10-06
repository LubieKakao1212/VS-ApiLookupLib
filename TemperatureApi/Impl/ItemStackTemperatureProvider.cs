using TemperatureApi.Api;
using Vintagestory.API.Common;

namespace TemperatureApi.Impl;

public class ItemStackTemperatureProvider(IWorldAccessor world, ItemStack stack) : IMutableTemperatureProvider {

    private float _temperature = stack.Collectible.GetTemperature(world, stack);
    
    public float GetTemperature() {
        return _temperature;
    }

    public void SetTemperature(float temp) {
        _temperature = temp;
    }

    public void Dispose() {
        stack.Collectible.SetTemperature(world, stack, _temperature);
    }
}