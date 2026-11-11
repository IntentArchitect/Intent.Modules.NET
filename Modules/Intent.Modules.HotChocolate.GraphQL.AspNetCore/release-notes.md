### Version 6.1.0

- Improvement: Updated NuGet package versions:
  - `HotChocolate.AspNetCore` 16.4.0 → 16.7.5 (net10.0, net9.0, net8.0); 14.3.1 → 14.3.5 (net7.0, net6.0)
  - `HotChocolate.AspNetCore.Authorization` 16.4.0 → 16.7.5 (net10.0, net9.0, net8.0); 14.3.1 → 14.3.5 (net7.0, net6.0)

> ⚠️ **BREAKING CHANGES**
>
> This module update may change behaviour or cause compilation breaks in custom code you have written that relies on HotChocolate 16.7, which turns variable batching on by default and adds a member to IServerDiagnosticEvents. For details see the [HotChocolate 16.6 to 16.7 migration guide](https://chillicream.com/docs/hotchocolate/v16/migrating/migrate-from-16-6-to-16-7).

### Version 6.0.9

- Improvement: Updated NuGet package versions.

### Version 6.0.8

- Improvement: Updated module dependencies.

### Version 6.0.7

- Improvement: Updated NuGet package versions.

### Version 6.0.6

- Improvement: Updated NuGet package versions.

### Version 6.0.5

- Improvement: Updated NuGet package versions.

### Version 6.0.4

- Improvement: Updated NuGet package versions.

### Version 6.0.3

- Improvement: Updated NuGet package versions.

### Version 6.0.2

- Improvement: Updated NuGet package versions.

### Version 6.0.1

- Improvement: Updated NuGet package versions.

### Version 6.0.0

- Improvement: Migrated to using new `Intent.Metadata.Security` package.
- Improvement: Updated NuGet package versions.

### Version 5.0.5

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.0.4

- Improvement: Updated NuGet packages to latest stables.

### Version 5.0.3

- Improvement: Bumped module version dependency for core HotChoclate module.

### Version 5.0.2

- Improvement: Updated Interoperable dependency versions.

### Version 5.0.1

- Fixed: Various hosting registration issues for minimal hosting model.

### Version 5.0.0

- Improvement: Updated to support use of top-level statements and minimal hosting model improvements introduced in [`Intent.AspNetCore` version 6.0.0](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.AspNetCore/release-notes.md#version-600).

### Version 4.0.1

- Update: bumping Hotchocolate versions.

### Version 4.0.0

- Support for the latest version of the Intent.HotChocolate.GraphQL module.
- Uses the v13 HotChocolate framework APIs.

### Version 3.3.8

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.3.7

- Updated supported client version to [3.3.0-pre.0, 5.0.0).
