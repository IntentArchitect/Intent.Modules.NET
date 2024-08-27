### Version 4.2.16

- Improvement: Transformed the `UnhandledExceptionBehaviour` template from T4 to File Builder for easier extensibility.

### Version 4.2.15

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.2.14

- Improvement: Updated NuGet packages to latest stables.
- Improvement: Added support for different UserId Types (guid, long, int).

> ⚠️ **NOTE**
> 
> `LoggingBehaviour`, `PerformanceBehaviour` - simplified the code removing unnecessary null coalescing to empty strings.

### Version 4.2.13

- Fixed: Duplicate `IDistributedCacheWithUnitOfWork` would be generated into classes under certain circumstances.

### Version 4.2.12

- Improvement: Added support for `IDistributedCacheWithUnitOfWork` to unit of work implementation.

### Version 4.2.11

- Improvement: Updated Interoperable dependency versions.

### Version 4.2.10

- Improvement: Added `ArgumentNullException` check on `IUnitOfWork` dependency injection.
- Improvement: Allow support for Redis OM Unit of Work.

### Version 4.2.7

- Improvement: Updated to be compatible with .NET 8.

### Version 4.2.6

- Improvement: Decoupled hard requirement on `IUnitOfWork` being available in an EF context. Will fall back on `IApplicationDbContext` if it's not available.

### Version 4.2.3

- Fixed: The `Intent.CosmosDB` "peer" dependency would prevent installation of any other modules if the `Intent.CosmosDB` module was already installed.

### Version 4.2.2

- Improvement: `UnitOfWorkBehaviour` will now universally save changes for all of the following modules without separate `Behaviour`s being created for each:
  - `Intent.CosmosDB`
  - `Intent.Dapr.AspNetCore.StateManagement`
  - `Intent.MongoDb`

### Version 4.1.3

- Upgrade *Breaking Changes*: Upgraded to MediatR v12, which has breaking changes from.

### Version 4.1.3

- Upgrade: Made the `Microsoft.Extensions.Logging` Nuget Package dependency .Net version aware.

### Version 4.1.2

- Fixed: Removed some warnings from the `LoggingBehaviour`.

### Version 4.1.0

- Update: EventBusPublisherBehaviour added.

### Version 4.0.1

- Fixed: UnitOfWorkBehaviour would incorrectly run and register itself even when there was no module generating an implementation.
