using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;

[assembly: ModInfo("Api-Lookup Lib", "ApiLookupLib",
    Authors = new string[] { "LubieKakao1212" },
    Description = "TODO",
    Version = "1.0.0")]

namespace ApiLookupLib {
    public class ApiLookupLibModSystem : ModSystem {
        
        public override void Start(ICoreAPI api) {
            Mod.Logger.Notification("Hello from template mod: " + api.Side);
        }

        public override void StartServerSide(ICoreServerAPI api) {
            Mod.Logger.Notification("Hello from template mod server side: " + Lang.Get("ApiLookupLib:hello"));
        }

        public override void StartClientSide(ICoreClientAPI api) {
            Mod.Logger.Notification("Hello from template mod client side: " + Lang.Get("ApiLookupLib:hello"));
        }
    }
}