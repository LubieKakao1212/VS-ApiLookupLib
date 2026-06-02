using System;
using CommonApis.Transact.Api;

namespace CommonApis.Temperature.Api;

public interface IMutableTemperatureProvider : ITemperatureProvider {

    public void SetTemperature(ITransactionContext transaction, float temp);
    
}