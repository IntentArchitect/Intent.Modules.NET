### Version 5.1.6

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.1.5

- Fixed: Program.cs generation more robust for Top-level statements and Minimal Hosting model configuration.
- Improvement: Updated NuGet packages to latest stables.

### Version 5.1.4

- Fixed: Not respecting explicit array based format for `WriteTo`.

### Version 5.1.3

- Improvement: Adding the Application Insights sink will only "force" you to specify a connection string when a environment is added to your appsettings.json that is non-Development. Gives a hassle-free out the box experience when running your application while ensuring that when deploying to a non-Development environment it ensures you specify a connection string.

### Version 5.1.2

- Fixed: Console logging format didn't write out Exceptions with their Stack traces in the Console sink.

### Version 5.1.1

- Improvement: Added `Application Insights` sink option.
- Fixed: Removing sinks will also uninstall their nuget packages. 

### Version 5.1.0

- Improvement: The NuGet package versions now automatically update to match the selected .NET version.
- Improvement: The configuration for the Serilog sink has been relocated from the Program.cs file to the `appsettings.json` file. This change gives developers the flexibility to include custom sinks in their projects, even if they are not directly supported by Intent Architect.
- New Feature: Support for [Graylog](https://github.com/serilog-contrib/serilog-sinks-graylog) and File sink options has been introduced, expanding the logging capabilities.

### Version 5.0.2

- Fixed: Added support for Serilog logging configuration in the `appsettings.json` file.

### Version 5.0.1

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 5.0.0

- Improvement: Updated to support use of top-level statements and minimal hosting model improvements introduced in [`Intent.AspNetCore` version 6.0.0](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.AspNetCore/release-notes.md#version-600).

### Version 4.0.0

- Updated to work with the major update made on the `Intent.AspNetCore` (v 5.0.0) module.

### Version 3.3.9

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.3.8

- Updated supported client version to [3.3.0, 5.0.0).
