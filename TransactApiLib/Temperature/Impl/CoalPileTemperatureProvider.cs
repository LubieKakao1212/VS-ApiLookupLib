using TransactApiLib.Temperature.Api;
using Vintagestory.GameContent;

namespace TransactApiLib.Temperature.Impl;

public class CoalPileTemperatureProvider(BlockEntityCoalPile be) : ITemperatureProvider {
    
    public float GetTemperature() {
        return be.IsBurning ? be.BurnTemperature : 20;
    }

}