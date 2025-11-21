### Version 4.5.5

- Improvement: Updated NuGet package versions.

### Version 4.5.4

- Improvement: Updated NuGet package versions.

### Version 4.5.3

- Improvement: Updated NuGet package versions.

### Version 4.5.2

- Improvement: Updated Shared Module.

### Version 4.5.1

- Improvement: Updated NuGet package versions.
- Fixed: Removed unnecessary `MediatR` using directive for Dependency Injection.

### Version 4.5.0

- Improvement: Select whether to lock the version of the MediatR Nuget package to the one prior to the commercial version or proceed to use the commercial version accepting its license. Read the article [here](https://www.jimmybogard.com/automapper-and-mediatr-commercial-editions-launch-today/).

> ⚠️ NOTE
>
> If you decide to go with the commercial version you will need to obtain and specify the license key.
> This can be done by requesting one as indicated in the article above and then inserting it into the `appsettings.json` under `MediatR:LicenseKey` (or as an environment variable `MediatR__LicenseKey`).

### Version 4.4.0

- Improvement: Upgraded to new ICurrentUserService interface.

### Version 4.3.8

- Improvement: Added support for DynamoDB unit of work.

### Version 4.3.7

- Improvement: Updated NuGet package versions.
- Improvement: SQLite ambient transaction suppression.

### Version 4.3.6

- Improvement: Locked MediatR NuGet package version

### Version 4.3.5

- Improvement: Updated NuGet package versions.
- Fixed: Issue with unit of work not being correctly injected into the constructor

### Version 4.3.4

- Improvement: Updated NuGet package versions.

### Version 4.3.3

- Improvement: Request Payload logging is now optional through the `CqrsSettings:LogRequestPayload` setting.
- Improvement: `CancellationToken` is now used as an argument to the `next()` delegate in the MediatR behaviour templates.

### Version 4.3.2

- Improvement: Updated NuGet package versions.

### Version 4.3.1

- Improvement: Updated NuGet package versions.

### Version 4.3.0

- Improvement: `AuthorizationBehaviour` now supports having multiple attributes being applied and treats them as an `AND` security requirement.
- Improvement: Updated NuGet package versions.

### Version 4.2.18

- Improvement: Updated `LoggingBehavior` and `PerformanceBehavior` to use Builder pattern.
- Improvement: `ILogger<{ClassName}>` is now injected into `LoggingBehavior`, `PerformanceBehavior` and `UnhandledExceptionBehaviour`, instead of `ILogger<TRequest>`.

### Version 4.2.17

- Improvement: ToList IEnumerable to prevent possible multiple enumerations.
- Improvement: Small updated to improve readibility of `LoggingBehaviour` and `PerformanceBehaviour`
- Fixed: Cleaned up some warnings on `UnhandledExceptionBehaviour`.

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
