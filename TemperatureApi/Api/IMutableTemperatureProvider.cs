using System;

namespace TemperatureApi.Api;

public interface IMutableTemperatureProvider : ITemperatureProvider {

    public void SetTemperature(float temp);
    
}