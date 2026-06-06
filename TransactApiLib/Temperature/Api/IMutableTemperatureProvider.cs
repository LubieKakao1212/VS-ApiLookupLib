using System;
using TransactApiLib.Transact.Api;

namespace TransactApiLib.Temperature.Api;

public interface IMutableTemperatureProvider : ITemperatureProvider {

    public void SetTemperature(ITransactionContext transaction, float temp);
    
}