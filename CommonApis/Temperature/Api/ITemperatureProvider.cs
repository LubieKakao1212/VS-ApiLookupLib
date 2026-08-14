namespace CommonApis.Temperature.Api;

public interface ITemperatureProvider {

    static float AmbientTemperature => 20f;
    
    public float GetTemperature();

}