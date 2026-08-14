using System;
using CommonApis.ApiLookup.API;
using CommonApis.ApiLookup.Helper;
using CommonApis.Storage.Helper;
using CommonApis.Temperature.Api;
using CommonApis.Temperature.Helper;
using CommonApis.Transact.Api;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace CommonApis.Temperature.Impl;

#pragma warning disable IStorage

public class ForgeTemperatureProvider(BlockEntityForge blockEntity) : IMutableTemperatureProvider {
    
    /// <summary>
    /// From <see cref="ITemperatureProvider.GetTemperature"/>
    /// </summary>
    /// <returns></returns>
    public float GetTemperature() {
        return (InternalStackTemperature()?.GetTemperature()).GetValueOrDefault(ITemperatureProvider.AmbientTemperature);
    }

    public void SetTemperature(ITransactionContext transaction, float temp) {
        InternalStackTemperature()?.SetTemperature(transaction, temp);
    }

    private IMutableTemperatureProvider? InternalStackTemperature() {
        var api = blockEntity.Api;
        var world = api.World;
        var pos = blockEntity.Pos;
        var lookups = api.ApiLookups();
        var collectibleStorage = lookups.Storage().Collectible().BlockSided;
        var temperature = lookups.TemperatureProviders().ItemStack;
        var workPieceStorage = collectibleStorage.Get(world, pos, BlockFacing.UP) ?? throw new NullReferenceException();
        var ctx = ItemStorageContext.GenericVoidOverflow(workPieceStorage, 0);
        var temp = ctx.Find(temperature, world, default); 
        
        if (temp == null) {
            return null;
        }
        if (temp is IMutableTemperatureProvider mut) {
            return mut;
        }
        api.Logger.Warning("Item with non-mutable temperature found on a forge");
        return null;
    }
}

#pragma warning restore IStorage