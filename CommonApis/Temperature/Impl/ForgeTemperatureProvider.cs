using CommonApis.Temperature.Api;
using Vintagestory.GameContent;

namespace TemperatureApi.Impl;

public class ForgeTemperatureProvider(BlockEntityForge be) : ITemperatureProvider {

    public void Dispose() { }

    public float GetTemperature() {
        return be.Lit ? 1100f : 20f;
    }
}