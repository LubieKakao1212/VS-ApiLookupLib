using ApiLookupLib.Helper;
using TransactApiLib.Temperature.Systems;

namespace TransactApiLib.Temperature.Helper;

public static class TemperatureApiGetters {

    public static TemperatureApis TemperatureProviders(this ApiLookupRegistry reg) {
        var sys = reg.modLoader.GetModSystem<TemperatureApiModSystem>();
        return new TemperatureApis(reg.modLoader, sys.ItemStack, sys.Block);
    }
}