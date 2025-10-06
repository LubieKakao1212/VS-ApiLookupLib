using TemperatureApi.Api;

namespace TemperatureApi.Helper;

public static class TemperatureApiExtensions {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="temperatureProvider"></param>
    /// <param name="target"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    /// <seealso cref="TemperatureUtility.ChangeTemperature"/>
    public static float ChangeTemperature(this IMutableTemperatureProvider temperatureProvider, float target, float delta) {
        var temperature = temperatureProvider.GetTemperature();
        var changed = TemperatureUtility.ChangeTemperature(temperature, target, delta);
        temperatureProvider.SetTemperature(changed);
        return changed;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="temperatureProvider"></param>
    /// <param name="target"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    /// <seealso cref="TemperatureUtility.ChangeTemperatureLinear"/>
    public static float ChangeTemperatureLinear(this IMutableTemperatureProvider temperatureProvider, float target, float delta) {
        var temperature = temperatureProvider.GetTemperature();
        var changed = TemperatureUtility.ChangeTemperatureLinear(temperature, target, delta);
        temperatureProvider.SetTemperature(changed);
        return changed;
    }
    
}