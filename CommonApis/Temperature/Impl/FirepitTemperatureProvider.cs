using CommonApis.Temperature.Api;
using Vintagestory.GameContent;

namespace CommonApis.Temperature.Impl;

public class FirepitTemperatureProvider(BlockEntityFirepit blockEntity) : MutableTemperatureProviderBase {

    public override float GetTemperature() {
        return blockEntity.furnaceTemperature;
    }

    protected override void SetTemperatureInternal(float temp) {
        blockEntity.furnaceTemperature = temp;
    }
}