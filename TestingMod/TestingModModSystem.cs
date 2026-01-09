using System.Linq;
using ApiLookupLib.API;
using ApiLookupLib.Helper;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

[assembly: ModInfo("TestingMod", "testingmod",
    Authors = ["Unknown"],
    Description = "This is a sample mod",
    Version = "1.0.0")]
namespace TestingMod {
    public class TestingModModSystem : ModSystem {

        private readonly Harmony _harmony = new Harmony("TestMod");

        public override double ExecuteOrder() => 1.0;
        
        public override void StartClientSide(ICoreClientAPI api) {
            base.StartClientSide(api);
            _harmony.PatchAll();
        }

        public override void Dispose() {
            base.Dispose();
            _harmony.UnpatchAll(_harmony.Id);
        }

        public override void AssetsFinalize(ICoreAPI api) {
            base.AssetsFinalize(api);

            var lookup = api.ApiLookups().TestApi();
            lookup.RegisterForBlockEntityBehaviors(SidesGetterBE("Sensitive", BlockFacing.NORTH, BlockFacing.SOUTH), true, typeof(BEBehaviorBurning), typeof(BEBehaviorTemperatureSensitive));
            
            lookup.RegisterForBlockEntityTypes(SidesGetterBE("Container or forge", BlockFacing.UP), true, typeof(BlockEntityContainer), typeof(BlockEntityForge));
            
            lookup.RegisterForCollectibleBehaviors(SidesGetter("Falling", BlockFacing.EAST, BlockFacing.NORTH, BlockFacing.SOUTH, BlockFacing.WEST), true, typeof(BlockBehaviorUnstableFalling));

            lookup.RegisterForBlocks(BlockPosGetter(BlockFacing.UP, BlockFacing.DOWN), AssetLocation.Create("*clay*"));
            lookup.RegisterFallback(AllSidesGetter("Hello"));
        }

        public IBlockApiLookup<ITestApi, BlockFacing>.Getter AllSidesGetter(string message) {
            return (world, source, context) => new TestApiImpl(message);
        }
        
        public IBlockApiLookup<ITestApi, BlockFacing>.Getter SidesGetter(string message, params BlockFacing[] sides) {
            return (world, source, context) => {
                if (sides.Contains(context)) {
                    return new TestApiImpl(message);
                }
                return null;
            };
        }
        
        public IBlockApiLookup<ITestApi, BlockFacing>.Getter BlockPosGetter(params BlockFacing[] sides) {
            return (world, source, context) => {
                if (sides.Contains(context)) {
                    return new TestApiBlockPos(source);
                }
                return null;
            };
        }
        
        public IBlockApiLookup<ITestApi, BlockFacing>.GetterBlockEntity SidesGetterBE(string message, params BlockFacing[] sides) {
            return (be, context) => {
                if (sides.Contains(context)) {
                    return new TestApiImpl(message);
                }
                return null;
            };
        }
    }
}