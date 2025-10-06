using TemperatureApi.Api;

namespace TemperatureApi.Impl;

public class ConstantTemperatureProvider(float temperature) : ITemperatureProvider {
    
    public float GetTemperature() {
        return temperature;
    }

    public void Dispose() { }
}