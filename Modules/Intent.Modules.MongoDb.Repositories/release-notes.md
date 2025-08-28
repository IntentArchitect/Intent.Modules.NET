### Version 2.0.0

- Deprecated: Made module obsolete as all templates are now contained within the MongoDb module

### Version 1.2.5

- Improvement: Methods with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).

### Version 1.2.4

- Improvement: Added support for extending an entity repository as well as generating custom entity-less repositories.

### Version 1.2.3

- Improvement: Module version bumps from MongoDb Module Dependency.

### Version 1.2.2

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.2.1

- Improvement: Updated NuGet packages to latest stables.

### Version 1.2.0

- Improvement: Changed repository contract to only have 1 generic parameter <TDomain> vs `<TDamain, TPersistence>`.
- Improvement: Added additional repository methods overloads to better support `IQueryable` similar to EF patterns.

### Version 1.1.1

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 1.0.1

- Improvement: Support for the renaming of the `IPagedResult` to `IPagedList` from the `Intent.Entities.Repositories.Api` module

### Version 1.0.0

- New: Repository pattern added for MongoDB provider.
