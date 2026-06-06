using TransactApiLib.Temperature.Api;
using Vintagestory.GameContent;

namespace TransactApiLib.Temperature.Impl;

public class FirepitTemperatureProvider(BlockEntityFirepit blockEntity) : ITemperatureProvider {

    public float GetTemperature() {
        return blockEntity.furnaceTemperature;
    }
}