## Api-Lookup Lib for Vintage Story
FabricApi-like api-lookup library for Vintage Story.

### Why I decided to make this library?
In general, I got frustrated at the lack of consistency in several parts of the games api. More precisely:
- Temperature - Firepit, Forge, CoalPiles and ItemStacks have different ways of going about how it is stored and how it should change over time.
Also `BlockEntityFirepit.changeTemperature` is not static even though it doesn't access any non-static field or functions
- Inventories/Item Storage - This is even more messy, because sometimes subclasses of `InventoryBase` are being used for item storage and sometimes it's `ItemStack[]`
- Fluids - apparently fluids storage is just an inventory, and fluid is an `Item` (which is not terrible but is unintuitive)

### What this does?
This library allows to "look up" an api from a given item/block. Different lookups may have arbitrary "contexts" such as `BlockFacing` for directional access. (How many chests do you want? 1.57 liters!)
### How to use?
#### Getting a value
```csharp
//Get a lookupInstance
IBlockApiLookup<ITemperatureProvider, NoContext> lookup = world.ApiLookups().TemperatureProviders().Block;
//Get an api from the lookup
var temp = lookup.Get(world, pos, null);
```

#### Registering new value getters
```csharp
//Registering getters must be done not sooner than in AssetsFinalize();

IBlockApiLookup<TApi, TContext> blockLookup; //For blocks
blockLookup.RegisterForBlocks((blockEntity, context) => /*value*/, new AssetLocation("some:block-*")); //You can use wildcards
blockLookup.RegisterForBlocks((world, pos, context) => /*value*/, new AssetLocation("some:block-exact"));
blockLookup.RegisterFallback((world, pos, context) => /*value*/); //Checked regardless of block type

IItemStackApiLookup<TValue, TContext> itemLookup; //For items
itemLookup.RegisterFor((world, stack, context) => /*value*/, new AssetLocation("some:block-or-item"));
itemLookup.RegisterFallback((world, stack, context) => /*value*/); //Checked regardless of collectible type
```

#### Creating Custom Lookups
```csharp
//For ItemStacks
IItemStackApiLookup<TApi, TContext> Items =  new SimpleItemStackApiLookup<TApi, TContext>();
//For Blocks
IBlockApiLookup<TApi, TContext> Blocks =  new SimpleBlockApiLookup<TApi, TContext>();
```