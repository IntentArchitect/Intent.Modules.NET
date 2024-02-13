### Version 2.0.0

- Improvement: Updated to support use of top-level statements and minimal hosting model improvements introduced in [`Intent.AspNetCore` version 6.0.0](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.AspNetCore/release-notes.md#version-600).

### Version 1.1.2

- Improvement: Console exporter added and set as default one so as to allow apps to run without having to configure APM connection strings.
- Improvement: Updated Open Telemetry nuget packages to latest stable versions. 

### Version 1.1.1

- Improvement: Added `application insight appinsights` to tags to make this module more discoverable.

### Version 1.1.0

- Improvement: Export setting from `Application Insights` to `Azure Application Insights`. If you have updated to this version you will need to re-set the Export option on the Module settings page to prevent Software Factory errors.
- New Feature: Optional Log capturing to be published to respective export option, i.e. sending Serilog logs to Azure Application Insights now possible.

### Version 1.0.2

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 1.0.0

- New Feature: Introduces an [open framework](https://opentelemetry.io/) for collecting and exporting telemetry data.