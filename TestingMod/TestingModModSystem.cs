using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;

[assembly: ModInfo("TestingMod", "testingmod",
    Authors = new string[] { "Unknown" },
    Description = "This is a sample mod",
    Version = "1.0.0")]

namespace TestingMod {
    public class TestingModModSystem : ModSystem {

        private readonly Harmony _harmony = new Harmony("TestMod");
            
        public override void StartClientSide(ICoreClientAPI api) {
            base.StartClientSide(api);
            _harmony.PatchAll();
        }

        public override void Dispose() {
            base.Dispose();
            _harmony.UnpatchAll(_harmony.Id);
        }
    }
}