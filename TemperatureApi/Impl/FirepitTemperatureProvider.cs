using TemperatureApi.Api;
using Vintagestory.GameContent;

namespace TemperatureApi.Impl;

public class FirepitTemperatureProvider(BlockEntityFirepit blockEntity) : ITemperatureProvider {

    public float GetTemperature() {
        return blockEntity.furnaceTemperature;
    }

    public void Dispose() { }
}