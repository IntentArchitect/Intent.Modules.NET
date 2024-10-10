### Version 2.2.2

- Fixed: It will no longer remove nuget packages that it deems to be remnants from previous settings since users may want to introduce those nuget packages themselves and it shouldn't get removed. The will need to remove it themselves.

### Version 2.2.1

- Fixed: Integration with MassTransit is broken due to new CSharpInvocationStatements representable as method chaining statements.

### Version 2.2.0

- New Feature: New Exporter option available `Azure Monitor OpenTelemetry Distro`. Read more about it [here](https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable?tabs=aspnetcore#why-should-i-use-the-azure-monitor-opentelemetry-distro).
  > [!NOTE]
  > 
  > Both `Azure Monitor OpenTelemetry Distro` and `Azure Application Insights` export to Azure Application Insights but the former uses a hybrid connection as opposed to the latter that ONLY uses the Open Telemetry Protocol.
- New Feature: Metrics option added along with new Instrumentation options (including for Tracing).
- Improvement: Service Instance Id can now be (optionally) set.

### Version 2.1.2

- Improvement: Updated module NuGet packages infrastructure.

### Version 2.1.1

- Improvement: Updated NuGet packages to latest stables.

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
