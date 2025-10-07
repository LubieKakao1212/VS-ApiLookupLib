using TemperatureApi.Api;
using Vintagestory.GameContent;

namespace TemperatureApi.Impl;

public class CoalPileTemperatureProvider(BlockEntityCoalPile be) : ITemperatureProvider {
    
    public float GetTemperature() {
        return be.IsBurning ? be.BurnTemperature : 20;
    }

    public void Dispose() { }

}