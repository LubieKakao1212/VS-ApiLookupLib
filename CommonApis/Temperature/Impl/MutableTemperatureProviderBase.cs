using CommonApis.Temperature.Api;
using Vintagestory.API.Common;

namespace CommonApis.Temperature.Impl;

public abstract class MutableTemperatureProviderBase(ILogger logger) : IMutableTemperatureProvider {

    private bool _disposed;

    ~MutableTemperatureProviderBase() {
        if (!_disposed) {
            logger.Warning($"{GetType()} has not been disposed, changes were not applied");
        }
    }

    public abstract float GetTemperature();

    public abstract void SetTemperature(float temp);

    public void Dispose() {
        _disposed = true;
        ApplyChanges();
    }

    protected abstract void ApplyChanges();

}