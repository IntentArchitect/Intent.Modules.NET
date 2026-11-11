### Version 5.1.0

- Improvement: Updated NuGet package versions:
  - `HotChocolate` 16.4.0 → 16.7.5 (net10.0, net9.0, net8.0); 14.3.1 → 14.3.5 (net7.0, net6.0, netstandard2.0)

> ⚠️ **BREAKING CHANGES**
>
> This module update may change behaviour or cause compilation breaks in custom code you have written that relies on HotChocolate 16.7, which turns variable batching on by default and adds a member to IServerDiagnosticEvents. For details see the [HotChocolate 16.6 to 16.7 migration guide](https://chillicream.com/docs/hotchocolate/v16/migrating/migrate-from-16-6-to-16-7).

### Version 5.0.11

- Improvement: Updated NuGet package versions.

### Version 5.0.10

- Fixed: Minimum client version.

### Version 5.0.9

- Improvement: Updated module documentation to use centralized documentation site.

### Version 5.0.8

- Improvement: Updated NuGet package versions.

### Version 5.0.7

- Improvement: Updated NuGet package versions.

### Version 5.0.6

- Improvement: Updated NuGet package versions.

### Version 5.0.5

- Improvement: Updated NuGet package versions.

### Version 5.0.4

- Improvement: Updated NuGet package versions.

### Version 5.0.3

- Improvement: Updated NuGet package versions.

### Version 5.0.2

- Improvement: Updated NuGet package versions.

### Version 5.0.1

- Improvement: Included module help topic.

### Version 5.0.0

- Improvement: Migrated to using new `Intent.Metadata.Security` package.
- Improvement: Updated NuGet package versions.

### Version 4.0.6

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.0.5

- Improvement: Updated NuGet packages to latest stables.

### Version 4.0.4

- Improvement: Added some basic docs.

### Version 4.0.3

- Improvement: Upgraded module to support new 4.1 SDK features.
 
### Version 4.0.2

- Update : Added `System.Linq` namespace to `MutationType` and `QueryType` templates.

### Version 4.0.0

- Update : Bumped `HotChoclate` nuget version to 13.1.0.

### Version 4.0.0

- Overhaul of GraphQL approach to support explicit Queries, Mutations and Subscriptions from the Intent.Modelers.Services.GraphQL module.
- Uses the v13 HotChocolate framework APIs.

### Version 3.3.7

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.3.6

- Updated supported client version to [3.3.0-pre.0, 5.0.0).
