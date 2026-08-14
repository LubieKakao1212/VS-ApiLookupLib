using System;
using CommonApis.ApiLookup.Helper;
using CommonApis.Temperature.Api;
using CommonApis.Temperature.Helper;
using CommonApis.Transact.Api;
using HarmonyLib;
using Vintagestory.GameContent;

namespace TestingMod.HarmonyPatches;

[HarmonyPatch(typeof(BlockEntityFirepit))]
public class BlockEntityFirepit_Patches {
    
    [HarmonyPatch("OnBurnTick")]
    [HarmonyPostfix]
    static void BurnTick_Postfix(BlockEntityFirepit __instance, float dt) {
        var api = __instance.Api;
        var tempLookup = api.ApiLookups().TemperatureProviders().Block;
        var pos = __instance.Pos;
        var upPos = pos.UpCopy();
        var upTempProvider = tempLookup.Get(api.World, upPos, default);
        var selfTempProvider = tempLookup.Get(api.World, pos, default) ?? throw new ApplicationException($"Invalid state detected, failed to retrieve ");

        if (upTempProvider is IMutableTemperatureProvider upTempMut) {
            using var transaction = Transaction.OpenRoot();
            var selfTemp = selfTempProvider.GetTemperature();
            upTempMut.ChangeTemperature(transaction, selfTemp, dt);
            transaction.Commit();
        }
    }
}