using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using ApiLookupLib.API;
using ApiLookupLib.Helper;
using TemperatureApi.Api;

namespace TemperatureApi.Helper;

public static class TemperatureApiGetters {

    public static TemperatureApiModSystem TemperatureProviders(this ApiLookupRegistry reg) {
        return reg.modLoader.GetModSystem<TemperatureApiModSystem>();
    }
}