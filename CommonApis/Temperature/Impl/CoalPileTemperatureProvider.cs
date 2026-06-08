using CommonApis.Temperature.Api;
using Vintagestory.GameContent;

namespace CommonApis.Temperature.Impl;

public class CoalPileTemperatureProvider(BlockEntityCoalPile be) : ITemperatureProvider {
    
    public float GetTemperature() {
        return be.IsBurning ? be.BurnTemperature : 20;
    }

}