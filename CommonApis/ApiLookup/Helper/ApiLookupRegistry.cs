using Vintagestory.API.Common;

namespace CommonApis.ApiLookup.Helper;

/// <summary>
/// Used for extension methods
/// </summary>
public ref struct ApiLookupRegistry(IModLoader modLoader) {

    public readonly IModLoader modLoader = modLoader;

}