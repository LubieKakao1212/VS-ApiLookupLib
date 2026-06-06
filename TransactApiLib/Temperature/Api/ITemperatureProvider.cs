using System;

namespace TransactApiLib.Temperature.Api;

public interface ITemperatureProvider {
    
    public float GetTemperature();

}