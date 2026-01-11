# Api-Lookup Lib for Vintage Story
FabricApi-like api-lookup library for Vintage Story.

## Why I decided to make this library?
In general, I got frustrated at the lack of consistency in several parts of the games api. More precisely:
- Temperature - Firepit, Forge, CoalPiles and ItemStacks have different ways of going about how it is stored and how it should change over time.
Also `BlockEntityFirepit.changeTemperature` is not static even though it doesn't access any non-static field or functions
- Inventories and Item/Fluid Storage - This is even more messy, because sometimes subclasses of `InventoryBase` are being used for item storage and sometimes it's `ItemStack[]`

## What this does?
This library allows to "look up" an api from a given item/block (soon also entities). Different lookups may have arbitrary "contexts" such as `BlockFacing` for directional access.
## How to use?
### Use Nullability checks!
It is recommended to use this library with nullability checks enabled.
To enable nullability checks add this tou project properties:
```msbuild
<Nullable>enable</Nullable>
```
I will not answer any issues regarding `NullReferenceException` if you don't have these checks enabled.

### Getting a value (Example using CommonApis)
```csharp
//Get a lookupInstance
IBlockApiLookup<ITemperatureProvider, NoContext> lookup = world.ApiLookups().TemperatureProviders().Block;
//Get an api from the lookup
var temp = lookup.Get(world, pos, default);
```
#### Note on ItemStackApiLookups:  
Contrary to what the name suggests, they do not take ItemStacks directly, but a wrapper called `IItemAccess`. 
Currently `IItemAccess` can be created using an ItemStack directly or by using a `Slot` from an `Inventory`.
This allows a given api to directly interact with the Inventory it is attached to.

### Registering new value getters
Registering getters must be done not sooner than in AssetsFinalize();
- #### Generic Options:
```csharp 
IApiLookupBase<TApi, TContext, TSource> lookup;
lookup.RegisterFallback(); // Used to register getter for all objects, fallbacks are searched last
```
- #### For Blocks:
```csharp
//Block and ItemStack lookups have more registrations options
IBlockApiLookup<TApi, TContext> blockLookup; //For blocks
blockLookup.RegisterForBlocks(Getter, AssetLocation); //By locations/ids, you can use wildcards
blockLookup.RegisterForBlocks(Getter, params Block[]); //For specific blocks
blockLookup.RegisterForBlocks(BlockEntityGetter, /* Any from above */); //With provided BlockEntity

//You can also register by Behaviors (Both Collectible and BlockEntity), BlockEntity classCodes, BlockEntity Types
//All options Requesting Type
```
- #### For Items
```csharp
IItemStackApiLookup<TValue, TContext> itemLookup; //For items
itemLookup.RegisterFor(Getter, /* AssetLocation or params Block[] */); //Similar to BlockApiLookups
itemLookup.RegisterForBehaviors(Getter, bool, params Type[]) // Registers for all collectibles with any Behavior of/deriving from given types
itemLookup.RegisterForTypes(Getter, bool, params Type[]) // Registers for all collectibles of/deriving from given types
```

### Creating Custom APIs
An api should be an interface.  
If the given api is mutable i.e. allows for persistent changes it should also implement `IDisposable`, all changes sould be applied in `IDisposable.Dispose()`.

### Creating Custom Lookups
```csharp
//For ItemStacks
IItemStackApiLookup<TApi, TContext> Items = new SimpleItemStackApiLookup<TApi, TContext>();
//For Blocks
IBlockApiLookup<TApi, TContext> Blocks = new SimpleBlockApiLookup<TApi, TContext>();
```
#### Adding easy access to your lookups:
There is an extension method attached to `IWorldAccessor`, `ICoreAPI` and `IModLoader` which returns `ApiLookupRegistry`.

You encouraged to add extension methods to `ApiLookupRegistry` which return a class with access to your lookups, group these extensions by feature and not by mod.
See example in CommonApis.

### Example context usage
- `NoContext`: There is no context, always pass default
- `BlockFacing`: This lookup is sided i.e. can be queried for different sides, different sides may return different results
- `BlockFacing?`: Same as above but also accepts `null` as "Internal"

Contexts are not limited to these values, you can use any type if you need to