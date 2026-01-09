using System;

namespace TemperatureApi.Helper;
using Vintagestory.GameContent;

public static class TemperatureUtility {

    /// <summary>
    /// Calculates and returns new temperature based on <paramref name="fromTemp"/> adjusted towards <paramref name="toTemp"/> with given <paramref name="deltaTime"/><br/>
    /// Copied from <see cref="BlockEntityFirepit"/> because original is not static.
    /// </summary>
    /// <param name="fromTemp">Current Temperature</param>
    /// <param name="toTemp">Target Temperature</param>
    /// <param name="delta">Temperature change factor, usually deltaTime * constant</param>
    /// <seealso cref="BlockEntityFirepit.changeTemperature"/>
    /// <returns>Adjusted Temperature</returns>
    public static float ChangeTemperature(float fromTemp, float toTemp, float delta) {
        float deltaT = Math.Abs(fromTemp - toTemp);
        delta += delta * (deltaT / 28f);
        if (deltaT < delta)
            return toTemp;
        if (fromTemp > toTemp)
            delta = -delta;
        return Math.Abs(fromTemp - toTemp) < 1.0 ? toTemp : fromTemp + delta;
    }

    /// <summary>
    /// Similar to <see cref="ChangeTemperature"/> but changes the temperature in linear fashion
    /// </summary>
    /// <param name="fromTemp">Current temperature</param>
    /// <param name="toTemp">Target Temperature</param>
    /// <param name="delta">Temperature change, usually deltaTime * constant</param>
    /// <returns></returns>
    public static float ChangeTemperatureLinear(float fromTemp, float toTemp, float delta) {
        float deltaT = Math.Abs(fromTemp - toTemp); // For termination condition
        if (deltaT < delta)
            return toTemp;
        if (fromTemp > toTemp)
            delta = -delta;
        return Math.Abs(fromTemp - toTemp) < 1.0 ? toTemp : fromTemp + delta;
    }
    
}