using System;

namespace TemperatureApi.Api;

public interface ITemperatureProvider : IDisposable {
    
    public float GetTemperature();

}