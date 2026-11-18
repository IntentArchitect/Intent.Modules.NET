### Version 1.1.2

- Fixed: `MassTransitMessageBus.SchedulePublishAsync` no longer generates a redundant null-forgiving operator (`!`) on the null-checked `ConsumeContext` that SonarAnalyzer rule S8969 reports, which failed builds with warnings treated as errors.

### Version 1.1.1

- Fixed: Minimum client version.

### Version 1.1.0

- Improvement: Updated to support composite message bus architecture and `IMessageBus` interface.

### Version 1.0.5

- Improvement: Updated module documentation to use centralized documentation site.

### Version 1.0.4

- Improvement: Included module help topic.

### Version 1.0.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.0.2

- Improvement: Updates based on changes made in `Intent.Eventing.MassTransit`.

### Version 1.0.1

- Fixed: Missing using directives.

### Version 1.0.0

- New Feature: Schedule Publishing messages from `IEventBus`.
