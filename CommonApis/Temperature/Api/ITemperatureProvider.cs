using System;

namespace CommonApis.Temperature.Api;

public interface ITemperatureProvider : IDisposable {
    
    public float GetTemperature();

}