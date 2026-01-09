using ApiLookupLib.Helper;
using HarmonyLib;
using TemperatureApi;
using TemperatureApi.Helper;

namespace ApiLookupImplSystems.Temperature;

[HarmonyPatch(typeof(TemperatureApiGetters))]
[HarmonyPatch(nameof(TemperatureApiGetters.TemperatureProviders))]
public class TemperatureApiGettersPatch {

    static bool Prefix(ApiLookupRegistry reg, ref TemperatureApis __result) {
        var sys = reg.modLoader.GetModSystem<TemperatureApiModSystem>();
        __result = new TemperatureApis(reg.modLoader, sys.ItemStack, sys.Block);
        return false;
    }
    
}