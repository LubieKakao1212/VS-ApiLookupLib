# CommonApis (formerly ApiLookupLob)
***

CommonApis is a collection of related semi-independent apis (or modules) for modders.  

### List of current modules with dependencies:
- [ApiLookup](#apilookup) -> no deps[*](#readme-star-one)
- [Transact](#transact) -> no deps
- [IStorage](#) -> ApiLookup**, Transact
- [Temperature](#) -> ApiLookup, Transact, IStorage

### General Advice:
It is recommended to use this library with nullability checks enabled.
To enable nullability checks add this to project properties:
```msbuild
<Nullable>enable</Nullable>
```
In new mod templates this is enabled by default.

Every module in this mod has a separation of `Api` and `Implementation`, because of that you should never have to access anything outside of `*.Api` and sometimes `*.Helper` namespaces.

The code is partially documented, however if you are unsure how to use something feel free to contact me on discord.

***
## Module Descriptions
### ApiLookup
In general this module works similarly to one from [FabricApi](https://github.com/FabricMC/fabric-api/tree/26.2/fabric-api-lookup-api-v1).  
The main use cases are as follows:
- Adding functionality to existing features, without reimplementing/replacing them (or allow people to do the same)
- Easly expose different kinds of th esame interaction base on a specific context e.g. direction

Examples:
```csharp
TODO
```

<a name="readme-star-one">*To use ItemStackApiLookup using IStorage is required</a>


### Transact
This module provides `Transactions` to replace methods like `TryExtract()` or `SimulateInsert()`.
All of the above operations have similar issues:
- They get messy really quickly as operation complexity increses
- They rely on an assumption that either the **result is always correct** or that the **state does not change between a check and an action**.
    However these can't always be easly enforced so you have to work you way around them.

Transactions solve this issue by making it trivial to `rollback` an unwanted state.

Examples:
```csharp
//Assume we have an object which has a "fluid" which can be drained
IFluidSource fluidSource = ...;
//Keyword `using` is important so that the transaction closes correctly
using var transaction = Transaction.OpenRoot();

//We want to drain between 70 and 100 units of fluid
var fluidDrained = fluidSource.DrainFluid(transactio, 100);
if(fluidDrained < 70) {
    //We did not managed to drain enough.
    //Maybe there wasn't enough, maybe it is impossible to drain more thatn 50 at onece, we don't care.
    //Since we use `using` transaction is disposed automatically revirting the unwanted state
    return;
}
//We drained enough so commit the transaction
transaction.Commit();
//Since the transaction was commited the state is not reverted
return;
```

```csharp
//Similar case as before, but we also have a fluid sink
IFluidSource fluidSource = ...;
IFluidSink fluidSink = ...;

using var transaction = Transaction.OpenRoot();
//Again we attempt to drain 100 units
var fluidDrained = fluidSource.DrainFluid(transactio, 100);
if(fluidDrained < 70) {
    //Same as before
    return;
}

//Differnt syntax for using, similar behavior
using(var nestedTransaction = transaction.OpenNested()) {
    var inserted = fluidSink.InsertFluid(nestedTransaction, fluidDrained);
    if(inserted > 20) {
        //Commit only if we extracted at least 20
        nestedTransaction.Commit()
    }
    //If we did not manage to insert at least 20, the insertion gets rolled back
    //Since this was done using a nested transaction, draining is unaffected
}

//Some random check
if(!prayToRandomGods)
{
    //We failed to please the gods
    //Both drain and insert operations get rolled back
    return;
}
//We did everything fine, transaction gets commmited
//Draining result stayes
//Insert result stays if inserted at least 20 units
transaction.Commit();
return;
```

### IStorage
This module is experimental  
TODO

### Temperature
TODO

***
[OLD README](READMEOLD.md)