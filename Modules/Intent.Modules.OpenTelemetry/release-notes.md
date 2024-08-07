### Version 2.1.0

- Improvement: Service name is now configurable in appsettings.json.

### Version 2.0.4

- Improvement: Bumped `OpenTelemetry` packages to 1.8.0

### Version 2.0.3

- Improvement: Added `OpenTelemetry Protocol` which acts as a generic exporter for most systems including [Jaeger](https://www.jaegertracing.io/docs/1.48/apis/#opentelemetry-protocol-stable) and [Elastic Search](https://www.elastic.co/guide/en/observability/current/open-telemetry.html).
- Improvement: Updated nuget package versions to closest latest stable versions.

### Version 2.0.2

- Improvement: Updated Interoperable dependency versions.

### Version 2.0.1

- Improvement: Upgraded module to support new 4.1 SDK features.

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