### Version 1.1.1

Improvement: Added `application insight appinsights` to tags to make this module more discoverable.

### Version 1.1.0

- Update: Export setting from `Application Insights` to `Azure Application Insights`. If you have updated to this version you will need to re-set the Export option on the Module settings page to prevent Software Factory errors.
- New: Optional Log capturing to be published to respective export option, i.e. sending Serilog logs to Azure Application Insights now possible.

### Version 1.0.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 1.0.0

- New: Introduces an [open framework](https://opentelemetry.io/) for collecting and exporting telemetry data.