using ApiLookupLib.Helper;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace TestingMod.HarmonyPatches;

[HarmonyPatch(typeof(Block))]
[HarmonyPatch(nameof(Block.GetPlacedBlockInfo))]
public class BlockInfo_Patch {

    static string Postfix(string infoIn, IWorldAccessor world, BlockPos pos, IPlayer forPlayer) {
        // var lookup = world.ApiLookups().TemperatureProviders().Block;
        // var temp = lookup.Get(world, pos, null);
        //
        // if (temp != null) {
        //     infoIn += $"\n[Debug] Temperaute: {temp.GetTemperature()}";
        // }
        
        var lookup = world.ApiLookups().TestApi();

        var sel = forPlayer.CurrentBlockSelection;
        var testApi = lookup.Get(world, pos, sel.Face);
        
        if (testApi != null) {
            infoIn += $"\n[Debug] Test: {testApi.Message}";
        }
        return infoIn;
    }
}