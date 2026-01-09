using TemperatureApi;
using Vintagestory.API.Common;

namespace ApiLookupImplSystems;

public class RegisterDefaultsModSystem : ModSystem {

    public override double ExecuteOrder() {
        return 1.0;
    }

    public override void AssetsFinalize(ICoreAPI api) {
        TemperatureApiDefaults.RegisterDefaults(api);
    }

}