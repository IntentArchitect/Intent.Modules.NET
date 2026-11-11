### Version 1.1.0

- Improvement: Updated NuGet package versions:
  - `Microsoft.Extensions.Configuration.Abstractions` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0, netstandard2.0)
  - `Microsoft.Extensions.Configuration.Binder` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0, netstandard2.0)
  - `Microsoft.Extensions.DependencyInjection` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0, netstandard2.1)
  - `Microsoft.Extensions.Hosting` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0, netstandard2.1)
  - `Microsoft.Extensions.Hosting.WindowsServices` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0, netstandard2.1)

> ⚠️ **BREAKING CHANGES**
>
> This module update may change behaviour or cause compilation breaks in custom code you have written that relies on Microsoft.Extensions.Hosting v11, where a failing BackgroundService now makes IHost.RunAsync and StopAsync throw. For details see the [Microsoft.Extensions.Hosting 11 breaking change](https://github.com/dotnet/docs/blob/main/docs/core/compatibility/extensions/11/ihost-runasync-stopasync-throw-backgroundservice-failure.md).

### Version 1.0.17

- Improvement: Updated to use `Intent.VisualStudio.Projects`'s `Program.cs` template instead of one inside this module.
- Improvement: Updated NuGet package versions.

### Version 1.0.16

- Improvement: Updated NuGet package versions.

### Version 1.0.15

- Improvement: Updated NuGet package versions.

### Version 1.0.14

- Improvement: Updated module documentation to use centralized documentation site.

### Version 1.0.13

- Improvement: Updated NuGet package versions.

### Version 1.0.12

- Improvement: Updated NuGet package versions.

### Version 1.0.11

- Improvement: Updated NuGet package versions.

### Version 1.0.10

- Improvement: Updated NuGet package versions.

### Version 1.0.9

- Improvement: Updated NuGet package versions.

### Version 1.0.8

- Improvement: Updated NuGet package versions.

### Version 1.0.7

- Improvement: Updated NuGet package versions.

### Version 1.0.6

- Improvement: Updated NuGet package versions.
- Improvement: Added OS Check to `LoggerProviderOptions.RegisterProviderOptions`.

### Version 1.0.5

- Improvement: When Top level Statements are enabled, the Program class can be exposed for Type reference purposes.

### Version 1.0.4

- Fixed: When using minimal hosting model some service registration statements would be generated after application configuration statements.

### Version 1.0.3

- Improvement: Updated module icon
- Improvement: Updated NuGet package versions.

### Version 1.0.2

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.0.1

- Improvement: Updated NuGet packages to latest stables.

### Version 1.0.0

New Feature: Module release.
