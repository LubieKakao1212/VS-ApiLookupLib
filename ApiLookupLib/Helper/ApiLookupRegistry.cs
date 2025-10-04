using Vintagestory.API.Common;
using Vintagestory.Common;

namespace ApiLookupLib.Helper;

/// <summary>
/// Used for extension methods
/// </summary>
public ref struct ApiLookupRegistry(IModLoader modLoader) {

    public readonly IModLoader modLoader = modLoader;

}