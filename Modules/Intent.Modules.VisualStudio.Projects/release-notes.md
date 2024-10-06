### Version 3.8.1

- Feature: It is now possible to model custom implicit usings for a project in the Visual Studio designer. Right-click a project and select the `Add Custom Implicit Usings` option, then right-click on that and add as many `Implicit Using`s as desired.
- Improvement: Added support for external project references on CSharpProject.
- Improvement: `.csproj` templates for .NET Core / 5+ Projects now implement `ICanContainGlobalUsings`.

### Version 3.8.0

- Improvement: 4.3 upgrade

### Version 3.7.8

- Improvement: Added support for modeled Implicit NuGet package dependencies.

### Version 3.7.7

- Improvement: When generating CSProj file, the template now not install direct NuGet package dependencies if there is a transitive dependency on the package through a dependent CSProj file.

### Version 3.7.6

- Improvement: Updated module NuGet packages infrastructure.

### Version 3.7.5

- Fixed: A `Could not load file or assembly 'NuGet.Versioning...` exception would occur when using this module with Intent Architect version 4.3.0-beta.4 or higher.

### Version 3.7.4

- Improvement: Added `launchSettings.json` for `Microsoft.NET.Sdk.Worker` projects.

### Version 3.7.3

- Improvement: Added support for appsettings.json respecting `.editorconfig` files.

### Version 3.7.2

- Improvement: Support added for `AddUserSecretsEvent` and `AddProjectPropertyEvent`.

### Version 3.7.1

- Fixed: Nuget version consolidation would break under certain circumstances.

### Version 3.7.0

- Improvement: Aligned VS Solution Project Type Ids to the new project system ids (https://github.com/dotnet/project-system/blob/main/docs/opening-with-new-project-system.md).
- Improvement: Updated generated VS `Sln` file versioning Info

### Version 3.6.2

- Improvement: `appsettings.json` added for `CSProject` using the `Microsoft.NET.Sdk.Worker` SDK e.g. Windows Services.
- Improvement: Added the `Microsoft.AspNetCore`  logging level to be `Warning` in-line with VS Templates for Asp.Net projects.
- Fixed: Modified default logging behavior from Warning to Information to align with Serilog's default behavior.
 
### Version 3.6.1

- Improvement: Modernized the Visual Studio icon to the 2022 version.

### Version 3.6.0

- Feature: Added support for [JavaScript Projects](https://learn.microsoft.com/visualstudio/javascript/javascript-project-system-msbuild-reference).
- Improvement: Deprecated ".NET Core" project types are no longer visible on the context menu, use `C# Project (.NET)` instead and change the `SDK` and other options on it to create equivalent projects.
- Improvement: Options to create ".NET Framework" projects are now hidden by default, to have these options show use toggle the `Enable .NET Framework project creation` application setting.
- Improvement: `Add .NET Core Version` and `Add .NET Framework Version` options are now hidden unless you have the `Intent.ModuleBuilder` module installed.
- Improvement: Solution Folders will now be generated in `.sln` files for templates with a relative output location when their output is in the package root or a Solution Folder.
- Fixed: NuGet package removals weren't being processed.
- Fixed: Certain `.sln` files could cause exceptions to be thrown during Software Factory execution.

### Version 3.5.1

- Fixed: Fix an issue where `Root Namespace` project setting was not working.

### Version 3.5.0

- New Feature: It is now possible to specify `Use minimal hosting model` and `Use top-level statements` on `.NET Project`s with their `SDK` set to `Microsoft.NET.Sdk.Web`.
- New Feature: Added support for working with program and startup files with combination of "use minimal host model" and "use top-level-statements".
- Improvement: Final .NET 8 support.
- Improvement: Sending multiple `AppSettingRegistrationRequest` and `ConnectionStringRegistrationRequest` requests will only log a warning if the key and values are **not** the same.
- Fixed: Environment Variables, originating from other modules, targeting specific Launch Setting Profiles will now populate correctly in the launchsettings.json
- Fixed: Validation checks added to ensure no duplicate Template Outputs and no duplicate Folder names exist within the same folder. Error message takes you straight to the Visual Studio designer where the violation occurs.

### Version 3.4.2

- Fixed: Target Framework for `.NET 8` will output correct version number in `csproj` file. 

### Version 3.4.1

- Improvement: Removed `Could not determine framework element for project "<project path>". If you're using a "Directory.Build.props" file, change the project's Target Framework to "(unspecified)"` warning.
- Improvement: Added `WSL2` as a CommandName option for `launchsettings.json`.
- Fixed: `launchSettings.json` was incorrectly generated as `launchsettings.json` which would cause issues on case-sensitive file systems (thank you to [@shainegordon](https://github.com/shainegordon) for their [PR](https://github.com/IntentArchitect/Intent.Modules.NET/pull/5) for this).

### Version 3.4.0

- New Feature: It is now possible to enable [Central Package Management (CPM)](https://learn.microsoft.com/nuget/consume-packages/central-package-management) for a Solution in the Visual Studio designer. Refer to the [README](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.VisualStudio.Projects/README.md#central-package-management) for more information.

### Version 3.3.32

- Fixed: Folder Options no longer targets `Folder`s from `Intent.Common.Types` which causes conflicts with the `Folder` type in the Visual Studio module.

### Version 3.3.30

- Improvement : Added Blazor WebAssembly configuration options for `launchsettings.json`.

### Version 3.3.30

- Improvement: `launchsettings.json` now listens for an event named `LaunchProfileHttpPortRequired` and if received will add a regular `http://...` entry to a profile's `applicationUrl` when it only contains a single `https://...` item.

### Version 3.3.29

- Improvement: Added the following as available SDK options for .NET Projects making the list [complete](https://learn.microsoft.com/dotnet/core/project-sdk/overview#available-sdks):
  - `Microsoft.NET.Sdk.Razor`
  - `Microsoft.NET.Sdk.Worker`
  - `Microsoft.NET.Sdk.WindowsDesktop`
- Added" Support for `RemoveNugetPackageEvent` event to un-install removed / redundant Nuget packages.

### Version 3.3.28

- Improvement: Added `Microsoft.NET.Sdk.BlazorWebAssembly` as an available SDK option for .NET Projects.

### Version 3.3.27

- Improvement: Added `10.0`, `11.0` and `12.0` as selectable C# Project `Language Version`s.
- Improvement: Added `.NET 8.0` as a selectable `.NET Version` option.
- The template for `local.settings.json` for Azure Functions projects will now automatically flatten hierarchical settings from `AppSettingRegistrationRequest`s into a discrete key per field with `:` used to represent its hierarchical location which .NET's configuration system is able to reassemble into a hierarchical object at runtime.

### Version 3.3.26

- Fixed : Software Factory would throw "Duplicate Key" exception if a NuGet package, for the same package, existed more than once. If now works with the first one it finds.
- Improvement: Manage Dependency Versions for nuget packages found in C# projects used to create Intent Architect Modules.
- Improvement: Newly generated `launchsettings.json` profile for API profiles will no longer have HTTP urls since there are just too many instances where the startup application breaks due to conflicting ports being detected. Sticking to HTTPS alleviates that. Also its best practice to design your APIs for HTTPS.

### Version 3.3.25

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.3.24
- Update : Adjusting `launchSettings.json` to start as Https, made `isSSL` setting more inline with `VS` behaviour.

### Version 3.3.22

- Fixed: Updates to `.csproj` files would not always be applied.

### Version 3.3.21

- Improvement: Added support for Implicit Usings for projects.
- Fixed: Added SSL URI in `applicationUrl` for `Project` Laumch Profiles.

### Version 3.3.19

- Fixed: Exceptions would get thrown when certain aspects of a `.csproj` file were declared in or used properties defined in a `Directory.Build.props` file:
    - When `<TargetFramework(s) />` is not defined in the `.csproj` file, a warning will now occur with advice to use the `(unspecified)` option in the Visual Studio designer.
    - When a `<PackageReference />`'s `Version` is unparseable as a semantic version (for example due to being an MSBuild variable), it is now ignored.

### Version 3.3.18

- Improvement: Added .NET 7.0 version option.
- Improvement: Added support for generating `Remove` element types for file items in .NET `.csproj` files.
- Improvement: The `Root Namespace` value of a project in the Visual Studio designer is now made available on `IOutputTarget.Metadata` under the `Root Namespace` key.

### Version 3.3.17

- Improvement: Updated description of module.

### Version 3.3.16

 - Fixed: Nullable types not being indicated when `Nullable` is set to `enable`.

### Version 3.3.15

- Improvement: Updated `Nullable` stereotype property for the `C# Project Options` stereotype to have all available options for `.csproj` files.

### Version 3.3.14

- Improvement: If a `.csproj` file has a defined value when its corresponding property in the `.NET Core Settings` stereotype is blank, it will no longer be removed from the `.csproj` file.
- `IntentIgnore="true"` can now be applied to file item sub-elements in `.csproj` files.
- Fixed: `No project found for id "{projectId}"` exception would be thrown when a `Template Output` was in a solution folder and not a project.

### Version 3.3.13

- Fixed: local.settings.json doesn't support object values and better measures have been applied. 

### Version 3.3.12

- Fixed: Folder Options stereotype not applicable to normal folders in other designers.
- Fixed: An exception would occur when running the software factory after renaming a project in the Visual Studio designer.
- Fixed: When `WithBuildAction("None")` was specified for a template targetting an `.sqlproj` file, instead of it having an `ItemType` of `None`, it would exclude it completely.
- Fixed: An exception would occur in `LocalSettingsJsonTemplate` when an `AppSettingRegistrationRequest` had an anonymous object for its `Value`.
