using System;

namespace CommonApis.Temperature.Api;

public interface IMutableTemperatureProvider : ITemperatureProvider, IDisposable {

    public void SetTemperature(float temp);
    
}