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
        public override void Start(ICoreAPI api) {
            var harmony = new Harmony("TestMod");
            harmony.PatchAll();
        }
    }
}