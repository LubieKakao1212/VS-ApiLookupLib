using Vintagestory.API.Common;

namespace ApiLookupLib.Helper;

public static class ApiLookupRegistryGetters {

    public static ApiLookupRegistry ApiLookups(this IWorldAccessor world) {
        return world.Api.ApiLookups();
    }
    
    public static ApiLookupRegistry ApiLookups(this ICoreAPI api) {
        return api.ModLoader.ApiLookups();
    }
    
    public static ApiLookupRegistry ApiLookups(this IModLoader modLoader) {
        return new ApiLookupRegistry(modLoader);
    }
    
}