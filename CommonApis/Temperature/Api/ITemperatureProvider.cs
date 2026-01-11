using System;

namespace CommonApis.Temperature.Api;

public interface ITemperatureProvider {
    
    public float GetTemperature();

}