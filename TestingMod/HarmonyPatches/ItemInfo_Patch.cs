using System.Runtime.InteropServices.ComTypes;
using System.Text;
using ApiLookupLib.Helper;
using ApiLookupLib.Impl.Item;
using CommonApis.Temperature.Helper;
using HarmonyLib;
using Vintagestory.API.Common;

#pragma warning disable ItemLookup_Experimental

namespace TestingMod.HarmonyPatches;

// a.GetHeldItemInfo();

[HarmonyPatch(typeof(CollectibleObject))]
[HarmonyPatch(nameof(CollectibleObject.GetHeldItemInfo))]
public class ItemInfo_Patch {

    static void Postfix(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo) {
        var temperatureLookup = world.ApiLookups().TemperatureProviders().ItemStack;
        var tempApi = temperatureLookup.Get(world, new SlotItemAccess(inSlot), default);
        if (tempApi != null) {
            dsc.Append($"[Debug] Temperaute: {tempApi.GetTemperature():N0}");
        }
    }
    
}