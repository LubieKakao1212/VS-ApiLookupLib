using CommonApis.Temperature.Api;
using CommonApis.Transact.Api;

namespace CommonApis.Temperature.Impl;

public abstract class MutableTemperatureProviderBase : TransactionParticipant<float>, IMutableTemperatureProvider {

    public abstract float GetTemperature();

    public void SetTemperature(ITransactionContext ctx, float temp) {
        TakeSnapshot(ctx);
        SetTemperatureInternal(temp);
    }

    protected override sealed float CreateSnapshot() {
        return GetTemperature();
    }

    protected override sealed void RestoreSnapshot(float snapshot) {
        SetTemperatureInternal(snapshot);
    }

    protected abstract void SetTemperatureInternal(float temp);

}