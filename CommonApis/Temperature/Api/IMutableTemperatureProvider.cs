namespace CommonApis.Temperature.Api;

public interface IMutableTemperatureProvider : ITemperatureProvider {

    public void SetTemperature(float temp);
    
}