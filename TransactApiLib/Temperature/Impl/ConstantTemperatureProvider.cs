using TransactApiLib.Temperature.Api;

namespace TransactApiLib.Temperature.Impl;

public class ConstantTemperatureProvider(float temperature) : ITemperatureProvider {
    
    public float GetTemperature() {
        return temperature;
    }
}