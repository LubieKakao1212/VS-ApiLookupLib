using CommonApis.Temperature.Api;
using Vintagestory.GameContent;

namespace CommonApis.Temperature.Impl;

public class ForgeTemperatureProvider(BlockEntityForge be) : ITemperatureProvider {

    public float GetTemperature() {
        return 20f;
        // return  ? 1100f : 20f;
    }
}