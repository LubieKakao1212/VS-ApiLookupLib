using CommonApis.Temperature.Api;

namespace CommonApis.Temperature.Impl;

public class ConstantTemperatureProvider(float temperature) : ITemperatureProvider {
    
    public float GetTemperature() {
        return temperature;
    }
}