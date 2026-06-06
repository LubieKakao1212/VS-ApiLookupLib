using TransactApiLib.Temperature.Api;
using TransactApiLib.Transact.Api;
using Vintagestory.API.Common;

namespace TransactApiLib.Temperature.Impl;

public abstract class MutableTemperatureProviderBase : TransactionParticipant<float>, IMutableTemperatureProvider {

    public abstract float GetTemperature();

    public void SetTemperature(ITransactionContext ctx, float temp) {
        TakeSnapshot(ctx);
        SetTemperatureInternal(temp);
    }

    public override void CloseFinal(TransactionCloseResult closeResult) {
        if (closeResult == TransactionCloseResult.Commited) {
            ApplyChanges();   
        }
    }

    protected override sealed float CreateSnapshot() {
        return GetTemperature();
    }

    protected override sealed void RestoreSnapshot(float snapshot) {
        SetTemperatureInternal(snapshot);
    }

    protected abstract void SetTemperatureInternal(float temp);

    protected abstract void ApplyChanges();

}