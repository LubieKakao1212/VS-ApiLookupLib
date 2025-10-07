using HarmonyLib;
using Vintagestory.API.Common;

namespace ApiLookupImplSystems;

public class HarmonyModSystem : ModSystem {

    public override void StartPre(ICoreAPI api) {
        base.StartPre(api);

        var harmony = new Harmony("ApiLookupLib_Internal");
        harmony.PatchAll();
    }

}