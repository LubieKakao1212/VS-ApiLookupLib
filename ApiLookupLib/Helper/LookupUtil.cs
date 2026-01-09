using System;
using System.Linq;
using Vintagestory.API.Common;

namespace ApiLookupLib.Helper;

public static class LookupUtil {

    public static bool HasBlockEntityBehaviorType(this Block block, IClassRegistryAPI registry, Type behaviorType, bool inherited) {
        return block.BlockEntityBehaviors.Any(behavior => {
            var type = registry.GetBlockEntityBehaviorClass(behavior.Name);
            if (inherited) {
                return type.IsAssignableTo(behaviorType);
            }
            return type == behaviorType;
        });
    }


    public static bool TypeMatches(this Type self, Type? target, bool allowInherited) {
        return allowInherited ? target?.IsAssignableTo(self) ?? false : target == self;
    }
    
}