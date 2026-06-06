using ApiLookupLib.Helper;
using TransactApiLib.Temperature.Helper;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace TestingMod.HarmonyPatches;

[HarmonyPatch(typeof(Block))]
[HarmonyPatch(nameof(Block.GetPlacedBlockInfo))]
public class BlockInfo_Patch {

    static string Postfix(string infoIn, IWorldAccessor world, BlockPos pos, IPlayer forPlayer) {
        var temperatureLookup = world.ApiLookups().TemperatureProviders().Block;
        var tempApi = temperatureLookup.Get(world, pos, default);
        
        if (tempApi != null) {
            infoIn += $"\n[Debug] Temperaute: {tempApi.GetTemperature():N0}";
        }
        
        var testLookup = world.ApiLookups().TestApi();

        var sel = forPlayer.CurrentBlockSelection;
        var testApi = testLookup.Get(world, pos, sel.Face);
        
        if (testApi != null) {
            infoIn += $"\n[Debug] Test: {testApi.Message}";
        }
        return infoIn;
    }
}